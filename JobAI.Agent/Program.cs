using JobAI.Agent;

public class Program
{
    static async Task Main(string[] args)
    {
        // Ensure the console can display Cyrillic symbols if any job titles are in Bulgarian
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string secretsFile = "secrets.txt";
        var voice = new VoiceAssistant();
        bool wasError = false;
        if (!File.Exists(secretsFile) || File.ReadAllLines(secretsFile).Length < 4)
        {
            wasError = true;
            if (!File.Exists(secretsFile))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("====================================================");
                Console.WriteLine("❌ CRITICAL ERROR: CONFIGURATION FILE MISSING");
                Console.WriteLine("====================================================");
                Console.ResetColor();

                string alertMsg = "Critical error. Configuration file missing. Please create secrets dot text.";
                voice.SayMessage(alertMsg);

                Console.WriteLine("\nTo fix this, create 'secrets.txt' with these 4 lines:");
                Console.WriteLine("1. Primary Gemini API Key");
                Console.WriteLine("2. Secondary Gemini API Key");
                Console.WriteLine("3. LinkedIn Email Address");
                Console.WriteLine("4. LinkedIn Password");
            }
            while (true)
            {
                if (File.Exists(secretsFile))
                {
                    var lines = File.ReadAllLines(secretsFile);
                    if (lines.Length >= 4) break;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n⚠️ File found, but it's incomplete (less than 4 lines).");
                    Console.WriteLine("1. Primary Gemini API Key");
                    Console.WriteLine("2. Secondary Gemini API Key");
                    Console.WriteLine("3. LinkedIn Email Address");
                    Console.WriteLine("4. LinkedIn Password");
                    Console.ResetColor();
                    voice.SayMessage("The file is incomplete.");
                }

                Console.WriteLine("\n[R] - Retry scanning | [E] - Exit");
                Console.Write("Action: ");
                string choice = Console.ReadLine()?.ToUpper();

                if (choice == "E")
                {
                    voice.SayMessage("Goodbye.");
                    return;
                }
                else if (choice == "R")
                {
                    Console.WriteLine("🔄 Re-scanning...");
                    await Task.Delay(800);
                    continue;
                }
            }
        }
            
        if (wasError)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Success! Configuration fixed and loaded.");
            Console.ResetColor();
            voice.SayMessage("Configuration loaded. Starting the system. Let's find some remote jobs!");
            await Task.Delay(1000);
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        voice.SayMessage("Welcome. Initializing Job AI Hunter Pro. Ready to scan for remote opportunities.");
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
        }
        finally
        {
            Console.WriteLine("\n--- Execution Finished ---");
            Console.WriteLine("⌨️ Press any key to exit...");
            Console.ReadKey();
        }
    }
}