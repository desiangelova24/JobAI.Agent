using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JobAI.Agent
{
    public static class UIHelper
    {
        public static void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            string rocket = @"
           ^
          / \
         /   \
        /_____\
       |       |
       | JobAI |
       |  PRO  |
       |_______|
        /|   |\
       /_|___|_\
         v   v
        (_____)
";
            Console.WriteLine(rocket);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("=========================================");
            Console.WriteLine("       JOB-AI HUNTER PRO v1.0.0          ");
            Console.WriteLine("=========================================");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[STATUS]  System Online");
            Console.WriteLine($"[TARGET]  Remote C# Roles");
            Console.WriteLine($"[CURRENCY] EUR (Bulgaria 2026)");
            Console.WriteLine("=========================================");
            Console.WriteLine();

            Console.ResetColor();
        }
        public static void StartClock()
        {
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                string time = DateTime.Now.ToString("HH:mm:ss");
                string date = DateTime.Now.ToString("dd.MM.yyyy");

                Console.Title = $"🚀 JobAI Hunter Pro | 🕒 {time} | 📅 {date} | EUR Mode";
            };
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        public static void OpenGithubInstructions()
        {
            string githubUrl = "https://github.com/desiangelova24/JobAI.Agent#installation";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = githubUrl,
                    UseShellExecute = true
                });

                Console.WriteLine("\n🌐 Opening documentation in your browser https://github.com/desiangelova24/JobAI.Agent#installation");
            }
            catch (Exception)
            {
                Console.WriteLine("\n⚠️ Could not open browser automatically. Please visit the link above.");
            }
        }
    }
}
