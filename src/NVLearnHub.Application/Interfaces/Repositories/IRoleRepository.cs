using NVLearnHub.Domain.Entities.Identity;

namespace NVLearnHub.Application.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}