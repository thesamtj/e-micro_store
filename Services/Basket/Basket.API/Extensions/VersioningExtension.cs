using Asp.Versioning;

namespace Basket.API.Extensions
{
    public static class VersioningExtension
    {
        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
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

            return services;
        }
    }
}