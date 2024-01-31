using Application.Boudary.User;
using Application.Command.Authentication;
using Application.Command.User;
using Application.Handler.Authentication;
using Application.Handler.User;
using Domain.Interface.Cryptography;
using Infrastructure.Filter;
using Infrastructure.Message;
using Infrastructure.Message.Interface;
using Infrastructure.Middleware.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Context;
using Infrastructure.Middleware.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddScoped<IMessagesHandler, MessagesHandler>();
builder.Services.AddTransient(typeof(GenericRepository<,>));
builder.Services.AddTransient<ISha, Sha>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
builder.Services.AddTransient<IRequestHandler<CreateUserCommand, CreateUserOutput>, UserHandler>();
builder.Services.AddTransient<IRequestHandler<AuthenticationCommand, AuthenticationOutput>, AuthenticationHandler>();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connection = Environment.GetEnvironmentVariable("MySqlConnectionString");
    options
    .UseMySql(connection, ServerVersion.AutoDetect(connection), a => { });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthenticationFilter>();
});
builder.Services.AddAuthenticationJwt();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
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
        Description = "RefreshToken is a token for renewed ir token automatically"
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<DatabaseContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();