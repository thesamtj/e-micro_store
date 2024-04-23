using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Common.Logging;
using Common.Logging.Correlation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var host = builder.Host;

// Configure Serilog
host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
services.AddApiVersioning();
services.AddHealthChecks()
            .AddMongoDb(configuration["DatabaseSettings:ConnectionString"], "Catalog  Mongo Db Health Check",
                HealthStatus.Degraded);

//DI
services.AddAutoMapper(typeof(Program));
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductHandler).Assembly));
services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
services.AddScoped<ICatalogContext, CatalogContext>();
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IBrandRepository, ProductRepository>();
services.AddScoped<ITypesRepository, ProductRepository>();
services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" }); });

//Identity Server changes
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
            options.Audience = "Catalog";
        });
services.AddAuthorization(options =>
{
    options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "catalogapi.read"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
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
