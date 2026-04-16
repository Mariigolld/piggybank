using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY_PREFIX = "Categories_";

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<CategoryDto>> GetUserCategoriesAsync(string userId)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<CategoryDto>? cachedCategories))
            {
                var categories = await _unitOfWork.Categories.GetUserAndSharedCategoriesAsync(userId);
                cachedCategories = _mapper.Map<IEnumerable<CategoryDto>>(categories);

                _cache.Set(cacheKey, cachedCategories, TimeSpan.FromHours(1));
            }

            return cachedCategories!;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, string userId)
        {
            var category = _mapper.Map<Category>(dto);

            if (!dto.IsShared)
            {
                category.UserId = userId;
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove($"{CACHE_KEY_PREFIX}{userId}");

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                throw new KeyNotFoundException("Kategorija nije pronađena");
            }

            if (category.IsShared || category.UserId != userId)
            {
                throw new UnauthorizedAccessException("Ne možete obrisati ovu kategoriju");
            }

            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();

            _cache.Remove($"{CACHE_KEY_PREFIX}{userId}");
        }
    }
}