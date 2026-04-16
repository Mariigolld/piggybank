using Microsoft.EntityFrameworkCore;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSharedCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsShared)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetUserAndSharedCategoriesAsync(string userId)
        {
            return await _dbSet
                .Where(c => c.IsShared || c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
