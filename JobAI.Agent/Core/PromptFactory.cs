using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Agent.Core
{
    public static class PromptFactory
    {
        public static string GetJobAnalysisPrompt(string description)
        {
            // We use a more detailed instruction to ensure Gemini acts as a professional Career Coach.
            string prompt = $@"
            Task: Analyze the following job description for a .NET Developer.
            Return ONLY a raw JSON object. Do not include markdown blocks like ```json.

            Requirements for the JSON fields:
            1. 'Technologies': A comma-separated list of the tech stack (e.g., 'C#, .NET 8, SQL, Azure').
            2. 'LanguageLevel': Assess the required English level (e.g., 'B1 - Intermediate').
            3. 'WorkMode': Identify if it is '100% Remote', 'Hybrid', or 'On-site'. [cite: 2026-01-08]
            4. 'SalaryEUR': Extract the salary. If in BGN or other currency, convert it to EUR (1 EUR = 1.95583 BGN). If not found, return 0. [cite: 2026-01-14]
            5. 'MatchScore': A score from 1 to 100 based on how well it fits a Remote .NET Developer profile.
            6. 'Advice': A friendly, encouraging message in English. Mention the benefits of working from home, staying cozy, and not having to catch the bus!

            JSON Structure:
            {{
                ""Technologies"": ""string"",
                ""LanguageLevel"": ""string"",
                ""WorkMode"": ""string"",
                ""SalaryEUR"": number,
                ""MatchScore"": number,
                ""Advice"": ""string""
            }}

            Description to analyze:
            {description}";

            return prompt;
        }

        public static string GetEnglishTeacherPrompt(string text)
        {
            // This helps you learn English while working [cite: 2026-01-14]
            return $@"
                You are a friendly English teacher for a Bulgarian software developer. 
                Explain the following English phrase or text in Bulgarian: '{text}'.
                Provide the meaning, a literal translation, and a simple example of how to use it.";
        }
    }
}
