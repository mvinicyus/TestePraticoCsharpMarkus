using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.DepencyInjector
{
    public static class CorsServiceCollectionExtension
    {
        public static IServiceCollection AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "Cors",
                                  policy =>
                                  {
                                      policy.AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .SetIsOriginAllowed((host) => true)
                                            .AllowCredentials();
                                  });
            });
            return services;
        }

        public static WebApplication UseCorsServices(this WebApplication app)
        {
            app.UseCors("Cors");
            return app;
        }
    }
}