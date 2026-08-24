using Microsoft.EntityFrameworkCore;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Infrastructure.Data;
using NVLearnHub.Infrastructure.Repositories;
using NVLearnHub.Infrastructure.Services;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Application.Services;

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
        services.AddScoped<IEmailService, GmailEmailService>();

        // Wishlist (in-memory for now)
        services.AddScoped<IWishlistService, WishlistService>();

        return services;
    }
}