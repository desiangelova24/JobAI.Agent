
using JobAI.Agent;
using JobAI.Agent.Config;
using JobAI.Agent.Services;
using JobAI.Agent.UI;
using JobAI.Core.Helpers;
using JobAI.Data.Extensions;
using JobAI.Data.Initializers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Timers;
public class Program
{
    static async Task Main(string[] args)
    {
        //PathsConfig.DeleteWorkspace (); // Clean up any previous workspace on startup
        var serviceProvider = ServiceRegistration.ConfigureServices();
        var voice = serviceProvider.GetRequiredService<VoiceAssistant>();
      
        // Ensure the console can display Cyrillic symbols if any job titles are in Bulgarian
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        UIHelper.StartClock();
        Console.Title = "🚀 JobAI Hunter Pro v1.1.0 | Remote Search Mode (EUR)"; 
        UIHelper.ShowWelcomeScreen();

        voice.SayMessage("Welcome to Job AI Hunter. Checking configuration...");

        bool online = NetworkHelper.IsInternetAvailable();
        ResilienceHelper.ExecuteWithRetry(() =>
        {
            if (!NetworkHelper.IsInternetAvailable())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Offline (Check Connection)");
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("✅ Internet connected. Starting Agent...");
            Console.ResetColor();
        });
      
     
        Console.ForegroundColor = ConsoleColor.Cyan;
        voice.SayMessage("Ready to scan for remote opportunities.");
        Console.ResetColor();
        Console.WriteLine("📅 System Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        try
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IHostedService>() as DatabaseInitializer;
                if (initializer != null)
                {
                    await initializer.StartAsync(CancellationToken.None);
                }
            }
            var agent = serviceProvider.GetRequiredService<JobScanner>();
            await agent.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ FATAL ERROR: {ex.Message}");
            Console.ResetColor();

            //PathsConfig.DeleteWorkspace();
        }
        finally
        {
            Console.WriteLine("\n--- Execution Finished ---");
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("⌨️ Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}