using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace JobAI.Logging
{
    public static class LoggingExtensions
    {
        public static void AddJobAiLogging(this IServiceCollection services, IConfiguration configuration, string azureConnString = null)
        {
            var loggerConfig = new LoggerConfiguration()
                             .MinimumLevel.Information()
                             .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                             .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                             .ReadFrom.Configuration(configuration)
                             .Enrich.FromLogContext()
                             .WriteTo.Console();
            loggerConfig.ReadFrom.Configuration(configuration);

            if (!string.IsNullOrEmpty(azureConnString))
            {
                loggerConfig.WriteTo.ApplicationInsights(azureConnString, TelemetryConverter.Traces);
            }

            Log.Logger = loggerConfig.CreateLogger();
            services.AddLogging(x => x.AddSerilog());
        }
    }
}
