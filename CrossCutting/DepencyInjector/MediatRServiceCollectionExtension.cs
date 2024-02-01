using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.DepencyInjector
{
    public static class MediatRServiceCollectionExtension
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            return services;
        }
    }
}