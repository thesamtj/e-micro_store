using Basket.Application.GrpcService;
using Basket.Application.Handlers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Data;
using Basket.Infrastructure.Repositories;
using Discount.Grpc.Protos;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers();
services.AddApiVersioning();
services.AddDbContext<BasketContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
services.AddHealthChecks().Services.AddDbContext<BasketContext>();

//DI
services.AddAutoMapper(typeof(Program));
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateShoppingCartCommandHandler).Assembly));
// services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
services.AddScoped<IBasketRepository, BasketRepository>();
services.AddScoped<DiscountGrpcService>();
services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
            (o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});
//services.AddMassTransitHostedService(); Remove, it is automatically registered
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" }); });

// Identity Server changes
 var userPolicy = new AuthorizationPolicyBuilder()
     .RequireAuthenticatedUser()
     .Build();

services.AddControllers(config =>
{
    config.Filters.Add(new AuthorizeFilter(userPolicy));
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:9009";
        options.Audience = "Basket";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
