using Google.GenAI.Types;
using JobAI.Agent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Timers;

public class Program
{
    static async Task Main(string[] args)
    {
       
        var voice = new VoiceAssistant();
        // Ensure the console can display Cyrillic symbols if any job titles are in Bulgarian
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        UIHelper.StartClock();
        Console.Title = "🚀 JobAI Hunter Pro v1.1.0 | Remote Search Mode (EUR)"; 
        UIHelper.ShowWelcomeScreen();
        voice.SayMessage("Welcome to Job AI Hunter. Checking configuration...");
        ConfigValidator.CheckSystemRequirements();
        PathsConfig.InitializeWorkspace();
        ConfigValidator.RunFullSetup(voice);
        bool online = ConfigValidator.IsInternetAvailable();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[NETWORK]  ");
        if (online)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Online (Stable)");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Offline (Check Connection)");
        }
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Cyan;
        voice.SayMessage("Ready to scan for remote opportunities.");
        Console.ResetColor();
        Console.WriteLine("📅 System Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        try
        {
            // Initialize the main engine
            var agent = new JobScanner();

            // Start the automated scraping process
            await agent.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ FATAL ERROR: {ex.Message}");
            Console.ResetColor();

            PathsConfig.DeleteWorkspace();
        }
        finally
        {
            Console.WriteLine("\n--- Execution Finished ---");
            Console.WriteLine("⌨️ Press any key to exit...");
            Console.ReadKey();
        }
    }
}