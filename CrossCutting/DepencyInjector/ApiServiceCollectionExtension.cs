using Infrastructure.Filter;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.DepencyInjector
{
    public static class ApiServiceCollectionExtension
    {
        public static IServiceCollection AddApiRServices(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<AuthenticationFilter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });
            return services;
        }
    }
}