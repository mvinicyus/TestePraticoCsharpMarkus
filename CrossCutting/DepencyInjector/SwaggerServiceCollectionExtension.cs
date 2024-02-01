using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CrossCutting.DepencyInjector
{
    public static class SwaggerServiceCollectionExtension
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog teste", Version = "v1" });
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization using the bearer scheme." +
                    "\r\n\r\n Enter 'Bearer' [space] and then your token the text input below" +
                    "\r\n\r\n Example: \"Bearer 12345abcdef\""
                });
                x.AddSecurityDefinition("Refresh", new OpenApiSecurityScheme
                {
                    Name = "RefreshToken",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "RefreshToken is a token for renewed the token automatically"
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Refresh"
                            }
                        },
                        new string[]{ }
                    }
                });
            });
            return services;
        }
    }
}