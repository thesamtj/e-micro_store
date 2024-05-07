using Asp.Versioning.ApiExplorer;
using Basket.API.Extensions;
using Basket.API.Swagger;
using Basket.Application.GrpcService;
using Basket.Application.Handlers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Data;
using Basket.Infrastructure.Repositories;
using Common.Logging;
using Common.Logging.Correlation;
using Discount.Grpc.Protos;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var host = builder.Host;

// Configure Serilog
host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
services.AddDbContext<BasketContext>(opt =>
{
   opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
});
services.AddHealthChecks().Services.AddDbContext<BasketContext>();

//DI
services.AddAutoMapper(typeof(Program));
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateShoppingCartCommandHandler).Assembly));
services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
services.AddScoped<IBasketRepository, BasketRepository>();
services.AddScoped<DiscountGrpcService>();
services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
            (o => o.Address = new Uri(configuration["GrpcSettings:DiscountUrl"]));
services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration["EventBusSettings:HostAddress"]);
    });
});
//services.AddMassTransitHostedService(); Remove, it is automatically registered

services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddVersioning();
services.AddEndpointsApiExplorer();
services.AddSwagger();

// Identity Server changes
//var userPolicy = new AuthorizationPolicyBuilder()
//     .RequireAuthenticatedUser()
//     .Build();
//services.AddControllers(config =>
//{
//    config.Filters.Add(new AuthorizeFilter(userPolicy));
//});
//services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Authority = "https://localhost:9009";
//        options.Audience = "Basket";
//    });

var app = builder.Build();
// GetRequiredService<T>() throws an InvalidOperationException if it can't find the service
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
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
