using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NVLearnHub.API.Extensions
{
    public static class CorsServiceExtensions
    {
        public static readonly string PolicyName = "NVLearnHubCorsPolicy";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services,
                                                        IConfiguration configuration)
        {
            var allowedOrigins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}