using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Exceptions;

namespace Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
            (context, loggerConfiguration) =>
            {
                var env = context.HostingEnvironment;
                loggerConfiguration.MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                    .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .WriteTo.Console();
                if (env.IsDevelopment())
                {
                    loggerConfiguration.MinimumLevel.Override("Catalog", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Basket", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Discount", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Ordering", LogEventLevel.Debug);
                }

                //var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
                //if (!string.IsNullOrEmpty(elasticUrl))
                //{
                //    loggerConfiguration.WriteTo.Elasticsearch(
                //        new ElasticsearchSinkOptions(new Uri(elasticUrl))
                //        {
                //            AutoRegisterTemplate = true,
                //            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                //            IndexFormat = "EShopping-Logs-{0:yyyy.MM.dd}",
                //            MinimumLogEventLevel = LogEventLevel.Debug
                //        });
                //}
            };
    }
}
