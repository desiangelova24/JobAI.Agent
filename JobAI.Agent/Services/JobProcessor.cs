using JobAI.Agent.Config;
using JobAI.Agent.Models;
using JobAI.Agent.UI;
using JobAI.Core;
using JobAI.Core.Models;
using JobAI.Core.Services;
using JobAI.Core.Settings;
using JobAI.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace JobAI.Agent.Services
{
    public class JobProcessor
    {
        private readonly GeminiClient _geminiClient;
        private readonly JobService _service;
        private readonly VoiceAssistant _voice;
        private readonly Random _random;
        private readonly SeleniumSettings _settings;
        public JobProcessor(GeminiClient geminiClient, JobService db, VoiceAssistant voice, IOptions<SeleniumSettings> options)
        {
            _geminiClient = geminiClient;
            _service = db;
            _voice = voice;
            _random = new();
            _settings = options.Value;  
        }
        public async Task<bool> CheckIfJobExists(string extId)
        {
            return await _service.JobExistsAsync(extId);
        }   
        public async Task ProcessJob(string extId, string title, string company, string description, string jobUrl)
        {
            try
            {
                // Check if the job has already been processed to avoid duplicates  
                var aiResult = await GetAiAnalysis(description);
                if (aiResult == null) return;

                var remoteJob = new RemoteJob
                {
                    ExternalId = extId,
                    Title = title,
                    Company = company,
                    Description = description,
                    JobUrl = jobUrl,
                    MatchScore = aiResult.MatchScore,
                    Technologies = aiResult.Technologies,
                    WorkMode = aiResult.WorkMode,
                    LanguageLevel = aiResult.LanguageLevel,
                    SalaryEUR = aiResult.SalaryEUR,
                    Advice = aiResult.Advice,
                    CompanyOrigin = aiResult.CompanyOrigin
                };  
                await _service.ProcessAndSaveJob(remoteJob);

                //save log for all findings, not only the good ones, to have a complete history of what was processed
                SaveLogJobFinding(title, company, aiResult, jobUrl);

                Console.WriteLine($"✅ Processed: {title} | Score: {aiResult.MatchScore}%");

                //print all findings in the console, not only the good ones, to have a complete overview of what was analyzed   
                PrintJobAnalysis(title, company, aiResult);

                if (aiResult.MatchScore > 80)
                {
                    _voice.Say($"I found a great opportunity for {title} at {company}!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Critical error in ProcessJob: {ex.Message}");
            }
        }
        private async Task<AiResult> GetAiAnalysis(string description)
        {
            string jsonResult = await _geminiClient.AnalyzeJob(description);

            if (string.IsNullOrEmpty(jsonResult)) return null;

            var responseContainer = JsonConvert.DeserializeObject<AiResultResponse>(jsonResult);

            if (responseContainer == null || !responseContainer.Success)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️ AI analysis failed: {responseContainer?.Message}");
                Console.ResetColor();
                return null;
            }

            return JsonConvert.DeserializeObject<AiResult>(responseContainer.Message);
        }
        private void SaveLogJobFinding(string title, string company, AiResult aiResult, string jobUrl)
        {
            try
            {
                string logPath = _settings.LogsFolder;
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                  $"💼 POSITION: {title}\n" +
                                  $"📊 MATCH: {aiResult.MatchScore}% | ORIGIN: {aiResult.CompanyOrigin}\n" +
                                  $"🔗 LINK: {jobUrl}\n" +
                                  $"📝 ANALYSIS: {aiResult.Advice}\n" +
                                  "--------------------------------------------------\n";

                File.AppendAllText(logPath, logEntry);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"❌ Error saving log entry: {ex.Message}");
            }
           
        }
        private void PrintJobAnalysis(string title, string company, AiResult aiResult)
        {
            Console.WriteLine("\n" + new string('═', 60));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" 💼 JOB FOUND: {title.ToUpper()}");
            Console.WriteLine($" 🏢 COMPANY:  {company} - " + aiResult.CompanyOrigin);
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
