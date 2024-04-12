using Discount.API.Services;
using Discount.Application.Handlers;
using Discount.Core.IRepositories;
using Discount.Infrastructure.Data;
using Discount.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddGrpc();
// configure strongly typed settings object
services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));

//DI
services.AddAutoMapper(typeof(Program));
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDiscountCommandHandler).Assembly));
services.AddSingleton<DiscountContext>();
services.AddScoped<IDiscountRepository, DiscountRepository>();

var app = builder.Build();

// ensure database and tables exist
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DiscountContext>();
    await context.Init();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// app.UseHttpsRedirection();

app.UseRouting();

app.MapGrpcService<DiscountService>();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync(
        "Communication with gRPC endpoints must be made through a gRPC client.");
});

app.Run();
