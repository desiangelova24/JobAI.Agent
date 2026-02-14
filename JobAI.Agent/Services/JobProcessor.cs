using JobAI.Agent.Config;
using JobAI.Agent.Models;
using JobAI.Agent.UI;
using JobAI.Core;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Agent.Services
{
    public class JobProcessor
    {
        private readonly GeminiClient _geminiClient;
        private readonly DatabaseManager _db;
        private readonly VoiceAssistant _voice;
        private readonly Random _random;

        public JobProcessor(GeminiClient geminiClient, DatabaseManager db, VoiceAssistant voice)
        {
            _geminiClient = geminiClient;
            _db = db;
            _voice = voice;
            _random = new();
        }
        public bool CheckIfJobExists(string extId)
        {
            return _db.IsAlreadySaved(extId);
        }   
        public async Task ProcessJob(string extId, string title, string company, string description)
        {
            try
            {
                var apiKeys = ConfigValidator.LoadConfigFiles()?.GeminiApiKeys;
                string jsonResult = await _geminiClient.AnalyzeJob(description, apiKeys);

                if (string.IsNullOrEmpty(jsonResult)) return;

                var responseContainer = JsonConvert.DeserializeObject<AiResultResponse>(jsonResult);
                if (responseContainer == null) return;
                if (!responseContainer.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ AI analysis failed for {title}: {responseContainer.ErrorType} - {responseContainer.Message}");
                    Console.ResetColor();
                    return;
                }
                var aiResult = JsonConvert.DeserializeObject<AiResult>(responseContainer.Message);

                _db.SaveJobAsync(extId, title, company, description, aiResult);

                Console.WriteLine($"✅ Saved: {title} | Score: {aiResult?.MatchScore}%");
                PrintJobAnalysis(title, company, aiResult);
                if (aiResult.MatchScore > 80)
                {
                    _voice.Say($" I found a great ad for {title} at {company}!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing the ad: {ex.Message}");
            }
        }
        private void PrintJobAnalysis(string title, string company, AiResult aiResult)
        {
            Console.WriteLine("\n" + new string('═', 60));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" 💼 JOB FOUND: {title.ToUpper()}");
            Console.WriteLine($" 🏢 COMPANY:  {company}");
            Console.ResetColor();
            Console.WriteLine(new string('─', 60));

            Console.Write(" 🛠️  TECH STACK: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(aiResult.Technologies);
            Console.ResetColor();

            Console.WriteLine($" 🌍 MODE: {aiResult.WorkMode} | 🇬🇧 ENG: {aiResult.LanguageLevel}");

            Console.Write(" 💶 SALARY: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{aiResult.SalaryEUR:N2} EUR"); // Форматира се като валута
            Console.ResetColor();

            int score = aiResult.MatchScore;
            Console.Write(" 📈 MATCH: [");
            Console.ForegroundColor = score > 70 ? ConsoleColor.Green : ConsoleColor.Yellow;
            Console.Write(new string('█', score / 5) + new string('░', 20 - (score / 5)));
            Console.ResetColor();
            Console.WriteLine($"] {score}%");
            Console.WriteLine(new string('─', 60));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" 🤖 AI ADVICE:");
            Console.ResetColor();
            Console.WriteLine($" \"{aiResult.Advice}\"");
            Console.WriteLine(new string('═', 60) + "\n");
        }
    }
}
