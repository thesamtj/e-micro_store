using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
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

services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    //Enable when required
    options.ApiVersionReader = ApiVersionReader.Combine(
            new HeaderApiVersionReader("x-version"),
            new QueryStringApiVersionReader("api-version", "ver"),
            new MediaTypeApiVersionReader("ver"),
            new UrlSegmentApiVersionReader()
        );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c => 
{ 
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog API Microservice", Version = "1" }); 
});

var app = builder.Build();
// GetRequiredService<T>() throws an InvalidOperationException if it can't find the service
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
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
