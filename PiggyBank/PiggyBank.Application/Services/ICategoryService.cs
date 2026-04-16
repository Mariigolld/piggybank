using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetUserCategoriesAsync(string userId);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto, string userId);
        Task DeleteAsync(int id, string userId);
    }
}