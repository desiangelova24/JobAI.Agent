using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JobAI.Agent.Config
{
    public static class PathsConfig
    {
        private static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string TempFolder = Path.Combine(RootPath, "temp");
        public static readonly string BrowserProfile = Path.Combine(TempFolder, "JobAI_Profile");
        public static readonly string DatabaseFolder = Path.Combine(TempFolder, "JobAI-DB");
        public static readonly string BrowserScreenshotsPath = Path.Combine(TempFolder, "JobAI_Screenshots");
        public static readonly string LogsFolder = Path.Combine(TempFolder, "Logs");
        public static readonly string ConfigFileName = "appsettings.json";
        public static string FullConfigPath => Path.Combine(TempFolder, ConfigFileName);
        public static string DatabaseName => "jobs_history.db"; 
        public static string DatabaseFile => Path.Combine(DatabaseFolder, DatabaseName);

        /// <summary>
        /// 
        /// </summary>
        public static void InitializeWorkspace()
        {
            string[] foldersToCreate = {
                            TempFolder,
                            BrowserProfile,
                            DatabaseFolder,
                            LogsFolder,
                            BrowserScreenshotsPath
             };

            foreach (var folder in foldersToCreate)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    Console.WriteLine($"[INIT] Created directory: {folder}");
                }
            }
            if (!File.Exists(FullConfigPath))
            {
                var template = new
                {
                    Gemini = new { ApiKeys = new[] { "YOUR_KEY_HERE", "YOUR_SECOND_KEY_HERE" } },
                    Credentials = new { Email = "your-email@example.com", Password = "your-password" },
                    Settings = new { Currency = "EUR", MaxApplicationsPerDay = 10 }
                };
                string jsonString = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FullConfigPath, jsonString);

                Console.WriteLine($"[INIT] Created template config at: {FullConfigPath}");
            }
        }

        /// <summary>
        /// </summary>
        public static void DeleteWorkspace()
        {
            if (Directory.Exists(TempFolder))
            {
                Console.WriteLine($"⚠️  WARNING: You are about to delete EVERYTHING in: {TempFolder}");
                Console.Write("Are you sure? (type 'YES' to confirm): ");

                string confirmation = Console.ReadLine() ?? "";

                if (confirmation.ToUpper() == "YES")
                {
                    try
                    {
                        Directory.Delete(TempFolder, true);
                        Console.WriteLine("✅ Workspace deleted successfully. The app will now close.");
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error during deletion: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Deletion cancelled.");
                }
            }
        }
    }
}
