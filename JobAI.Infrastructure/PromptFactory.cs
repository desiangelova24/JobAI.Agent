using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Infrastructure
{
    /// <summary>
    /// Provides factory methods for generating prompt strings used in language model interactions, such as job
    /// description analysis and English language explanations.
    /// </summary>
    /// <remarks>This static class centralizes the creation of prompt templates to ensure consistency and
    /// maintainability when interacting with AI services. The generated prompts are tailored for specific scenarios,
    /// such as analyzing .NET Developer job descriptions or explaining English phrases to Bulgarian software
    /// developers.</remarks>
    public static class PromptFactory
    {
     
        public static string GetJobAnalysisPrompt(string description)
        {
            description = description.Replace("\"", "'");
            // We use a more detailed instruction to ensure Gemini acts as a professional Career Coach.
            string prompt = $@"
            Task: Analyze the following job description and my profile. Provide a professional 'AI ADVICE' in English.
            Return ONLY a raw JSON object. Do not include markdown blocks like ```json.

            Requirements for the JSON fields:
            1. 'Technologies':Compare this job description with my core skills: .NET, C#, SQL. Tell me how well I fit on a scale from 1 to 100.
            2. 'LanguageLevel': Assess the required English level (e.g., 'B1 - Intermediate'). If German/French required, set MatchScore to 0.
            3. 'WorkMode': Identify if it is '100% Remote', 'Hybrid', or 'On-site'.
            4. 'SalaryEUR': Extract the salary. If in BGN or other currency, convert it to EUR (1 EUR = 1.95583 BGN). If not found, return 0.
            5. 'MatchScore': Calculate a percentage (0-100) based on how well my .NET, C#, and SQL skills match the specific job requirements. Be strict: give points only for skills explicitly mentioned or strongly implied. Subtract points if a critical skill is missing (like a specific framework or cloud experience).
            6. 'Advice': A 'Market Intelligence' report in English. Analyze the hidden needs (speed, scaling, or fixing chaos). Explain how my skill in 'organizing complex logic' is the solution.
            7. 'CompanyOrigin': Identify if the company is 'English' or 'International' based on the name, description, or language of the ad.

            JSON Structure:
            {{
                ""Technologies"": ""string"",
                ""LanguageLevel"": ""string"",
                ""WorkMode"": ""string"",
                ""SalaryEUR"": number,
                ""MatchScore"": number,
                ""Advice"": ""string"",
                ""CompanyOrigin"": ""string""
            }}

            Description to analyze:
            {description}";

            return prompt;
        }

        public static string GetEnglishTeacherPrompt(string text)
        {
            // This helps you learn English while working
            return $@"
                You are a friendly English teacher for a Bulgarian software developer. 
                Explain the following English phrase or text in Bulgarian: '{text}'.
                Provide the meaning, a literal translation, and a simple example of how to use it.";
        }
    }
}
