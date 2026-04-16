using PiggyBank.Core.Entities;

namespace PiggyBank.Core.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Category>> GetSharedCategoriesAsync();
        Task<IEnumerable<Category>> GetUserAndSharedCategoriesAsync(string userId);
    }
}