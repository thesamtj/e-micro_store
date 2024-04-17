using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var env = builder.Environment;

// Add services to the container.
builder.Configuration.AddJsonFile($"ocelot.{env.EnvironmentName}.json", true, true);
services.AddOcelot()
            .AddCacheManager(o => o.WithDictionaryHandle());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
var env2 = app.Environment;

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

//app.Run();
