using Common.Logging;
using Common.Logging.Correlation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
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

// Configure Serilog
host.UseSerilog(Logging.ConfigureLogger);
configuration.AddJsonFile($"ocelot.{environment.EnvironmentName}.json", true, true);

// Add services to the container.
services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
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
//services.AddOcelot();
services.AddOcelot()
            .AddCacheManager(o => o.WithDictionaryHandle());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddSwaggerForOcelot(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
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

app.Run();
