using JobAI.Agent.Services;
using JobAI.Agent.UI;
using JobAI.Core;
using Microsoft.Extensions.DependencyInjection;

namespace JobAI.Agent.Config
{
    public static class DependencyInjection
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
            var services = new ServiceCollection();
            // Register services with appropriate lifetimes (singleton for shared instances, transient for new instances per request)   
            services.AddSingleton<VoiceAssistant>();
            services.AddSingleton<DatabaseManager>();
            services.AddSingleton<GeminiClient>();
            services.AddTransient<JobProcessor>();
            services.AddTransient<JobScanner>();

            return services.BuildServiceProvider();
        }
    }
}
