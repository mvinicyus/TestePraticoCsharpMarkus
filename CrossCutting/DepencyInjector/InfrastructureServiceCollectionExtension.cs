using Domain.Interface.Cryptography;
using Infrastructure.Message;
using Infrastructure.Message.Interface;
using Infrastructure.Middleware.Cryptography;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.DepencyInjector
{
    public static class InfrastructureServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddTransient<ISha, Sha>();
            services.AddScoped<IMessagesHandler, MessagesHandler>();
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            return services;
        }
    }
}