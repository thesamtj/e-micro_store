using Common.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;
var services = builder.Services;
var host = builder.Host;

// Configure Serilog
host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
configuration.AddJsonFile($"ocelot.{environment.EnvironmentName}.json", true, true);
var authScheme = "E-MicroStoreGatewayAuthScheme";
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(authScheme, options =>
        {
            options.Authority = "https://localhost:9009";
            options.Audience = "E-MicroStoreGateway";
        });
//.AddJwtBearer(options =>
//{
//    options.Authority = "https://localhost:9009";
//    options.Audience = "E-MicroStoreGateway";
//});
services.AddOcelot()
            .AddCacheManager(o => o.WithDictionaryHandle());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync(
        "Hello Ocelot");
});

await app.UseOcelot();

app.Run();
