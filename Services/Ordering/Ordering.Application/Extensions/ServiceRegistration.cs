using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Behaviour;
using Ordering.Application.Handlers;
using System.Reflection;

namespace Ordering.Application.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CheckoutOrderCommandHandler).Assembly));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddValidatorsFromAssemblyContaining<CheckoutOrderCommandValidator>();            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            return services;
        }
    }
}
