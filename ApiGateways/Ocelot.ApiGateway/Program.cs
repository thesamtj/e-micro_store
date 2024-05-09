using Common.Logging;
using Common.Logging.Correlation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.ApiGateway.Config;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;
var services = builder.Services;
var host = builder.Host;
var routes = "Routes";

// Configure Serilog
host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
//var authScheme = "E-MicroStoreGatewayAuthScheme";
//services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//        .AddJwtBearer(authScheme, options =>
//        {
//            options.Authority = "https://localhost:9009";
//            options.Audience = "E-MicroStoreGateway";
//        });
//.AddJwtBearer(options =>
//{
//    options.Authority = "https://localhost:9009";
//    options.Audience = "E-MicroStoreGateway";
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// configuration.AddJsonFile($"ocelot.{environment.EnvironmentName}.json", true, true);
configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = routes;
});
configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddOcelot(routes, environment)
    .AddEnvironmentVariables();

// services.AddOcelot(); only enable if the next line is not present
services.AddOcelot(configuration)
            .AddCacheManager(o => o.WithDictionaryHandle());
services.AddEndpointsApiExplorer();
services.AddSwaggerForOcelot(configuration);
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
        opt.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
    });
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddCorrelationIdMiddleware();

app.UseRouting();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync(
        "Hello Ocelot");
});

await app.UseOcelot();

app.Run();
