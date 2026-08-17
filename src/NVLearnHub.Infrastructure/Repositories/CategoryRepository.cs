using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Catalog;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(LearnHubDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetAllWithSubCategoriesAsync()
            => await _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null) // top-level only
                .ToListAsync();

        public async Task<Category?> GetWithSubCategoriesAsync(int id)
            => await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
    }
}