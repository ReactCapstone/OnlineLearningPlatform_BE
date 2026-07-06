using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Repositories;
using NVLearnHub.Infrastructure.Data;
using NVLearnHub.Infrastructure.Repositories;
using NVLearnHub.Infrastructure.Services;

namespace NVLearnHub.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<LearnHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Current User
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}