using JobAI.Agent.Services;
using JobAI.Agent.UI;
using JobAI.Core;
using JobAI.Core.Interfaces;
using JobAI.Core.Services;
using JobAI.Core.Settings;
using JobAI.Data.Data;
using JobAI.Data.Initializers;
using JobAI.Data.Repositories;
using JobAI.Infrastructure;
using JobAI.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JobAI.Agent.Config
{
    public static class ServiceRegistration
    {
        /// <summary>
        /// Configures and builds the application's service provider with required dependencies.    
        /// </summary>
        /// <remarks>The returned service provider includes singleton and transient registrations for core
        /// application components. Use this method to initialize dependency injection before resolving services in the
        /// application.</remarks>
        /// <returns>An <see cref="IServiceProvider"/> instance containing the configured services for dependency injection.</returns>
        public static IServiceProvider ConfigureServices()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var configuration = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env}.json", optional: true)
                          .AddEnvironmentVariables()
                          .Build();

            var services = new ServiceCollection();

            services.AddJobAiLogging(configuration, configuration.GetConnectionString("ApplicationInsights"));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            var settings = configuration.GetSection("SeleniumSettings").Get<SeleniumSettings>();

            if (settings != null)
            {
                string[] paths = { settings.BrowserScreenshotsPath, settings.LogsFolder, settings.BrowserProfile };
                foreach (var path in paths)
                {
                    if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
           
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure()));
            services.AddHostedService<DatabaseInitializer>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<JobService>(); 
            services.AddScoped<EdgeManager>();
            services.Configure<GeminiSettings>(configuration.GetSection("Gemini"));
            services.Configure<SeleniumSettings>(configuration.GetSection("SeleniumSettings"));
            services.Configure<LinkedInSettings>(configuration.GetSection("LinkedInSettings"));
            // Register services with appropriate lifetimes (singleton for shared instances, transient for new instances per request)   
            services.AddSingleton<VoiceAssistant>();
            //services.AddSingleton<DatabaseManager>();
            services.AddSingleton<GeminiClient>();
            services.AddSingleton<IGeminiClient, GeminiClient>();

            services.AddTransient<EdgeManager>();
            services.AddTransient<JobProcessor>();
            services.AddTransient<JobScanner>();

            return services.BuildServiceProvider();
        }
    }
}
