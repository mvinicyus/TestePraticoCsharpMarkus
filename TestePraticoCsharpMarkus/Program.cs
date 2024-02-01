using Application.HubWebSocket;
using CrossCutting.DepencyInjector;
using Infrastructure.Middleware.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddRepositoryServices();
builder.Services.AddMediatRServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddDomainServices();
builder.Services.AddApiRServices();
builder.Services.AddAuthenticationJwt();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerServices();
builder.Services.AddCorsServices();

var app = builder.Build();

app.UseRepositoryServices();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCorsServices();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");
app.Run();