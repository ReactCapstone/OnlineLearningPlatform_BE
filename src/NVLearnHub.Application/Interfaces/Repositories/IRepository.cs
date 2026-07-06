using NVLearnHub.Domain.Common;
using System.Linq.Expressions;

namespace NVLearnHub.Application.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    // Basic CRUD
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);

    // Extras
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}