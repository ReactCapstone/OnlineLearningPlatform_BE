using Microsoft.Extensions.DependencyInjection;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Application.Services;

namespace NVLearnHub.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register all application-layer services here as they are built
        services.AddScoped<IAuthService, AuthService>();

        // Future services go here — one line each:

        return services;
    }
}