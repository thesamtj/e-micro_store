using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;
using Ordering.API.Extensions;
using Microsoft.EntityFrameworkCore;
using Ordering.API.EventBusConsumer;
using MassTransit;
using EventBus.Messages.Common;
using Common.Logging;
using Serilog;
using Common.Logging.Correlation;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var host = builder.Host;

// Configure Serilog
host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
services.AddApplicationServices();
services.AddInfraServices(configuration);
services.AddHealthChecks().Services.AddDbContext<OrderContext>();
services.AddAutoMapper(typeof(Program));
services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
services.AddScoped<BasketOrderingConsumer>();
services.AddScoped<BasketOrderingConsumerV2>();
services.AddMassTransit(config =>
{
    //Mark this as consumer
    config.AddConsumer<BasketOrderingConsumer>();
    config.AddConsumer<BasketOrderingConsumerV2>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration["EventBusSettings:HostAddress"]);
        //provide the queue name with consumer settings
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });
        //V2 endpoint will pick items from here 
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueueV2, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumerV2>(ctx);
        });
    });
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering API Microservice", Version = "1" });
});

var app = builder.Build();

app.MigrateDatabase<OrderContext>((context, services) =>
{
    // GetService<T>() treturns null if it can't find the service
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait();
});

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
