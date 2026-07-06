using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Domain.Entities.Identity;
using NVLearnHub.Infrastructure.Data;

namespace NVLearnHub.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(LearnHubDbContext context) : base(context) { }

    public async Task<Role?> GetByNameAsync(string name)
        => await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
}