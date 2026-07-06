using NVLearnHub.Domain.Entities.Catalog;

namespace NVLearnHub.Application.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithSubCategoriesAsync();
    Task<Category?> GetWithSubCategoriesAsync(int id);
}