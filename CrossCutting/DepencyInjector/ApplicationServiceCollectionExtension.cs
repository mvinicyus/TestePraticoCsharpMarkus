using Application.Boudary.Post;
using Application.Boudary.User;
using Application.Command.Authentication;
using Application.Command.Post;
using Application.Command.User;
using Application.Handler.Authentication;
using Application.Handler.Post;
using Application.Handler.User;
using Application.HubWebSocket;
using Domain.Interface.HubWebSocket;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.DepencyInjector
{
    public static class ApplicationServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<CreateUserCommand, CreateUserOutput>, UserHandler>();
            services.AddTransient<IRequestHandler<CreatePostCommand, CreatePostOutput>, CreatePostHandler>();
            services.AddTransient<IRequestHandler<UpdatePostCommand, UpdatePostOutput>, UpdatePostHandler>();
            services.AddTransient<IRequestHandler<DeletePostCommand, DeletePostOutput>, DeletePostHandler>();
            services.AddTransient<IRequestHandler<GetPostsCommand, GetPostsOutput>, GetPostsHandler>();
            services.AddTransient<IRequestHandler<GetPostCommand, GetPostOutput>, GetPostHandler>();
            services.AddTransient<IRequestHandler<AuthenticationCommand, AuthenticationOutput>, AuthenticationHandler>();
            services.AddScoped<INotificationRealTime, NotificationRealTime>();
            return services;
        }
    }
}