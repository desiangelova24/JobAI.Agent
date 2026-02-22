using Google.GenAI;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace JobAI.Infrastructure
{
    public class GeminiClient
    {
        /// <summary>
        /// Sends the job description to Gemini AI for analysis.
        /// Implements API key rotation and retry logic to handle rate limits and server demand.
        /// </summary>
        public async Task<string> AnalyzeJob(string description, string[] apiKeys)
        {
            if (apiKeys == null || apiKeys.Length == 0)
            {
                return AiResultResponseFactory.CreateNoKeysResponse();
            }
            var model = "gemini-3-flash-preview"; // Core model
            int currentKeyIndex = 0;
            int totalRetries = 0;
            while (totalRetries < 5)
            {
                try
                {
                    // Initialize the AI client with the current active key
                    var client = new Client(apiKey: apiKeys[currentKeyIndex]);
                    string fullPrompt = PromptFactory.GetJobAnalysisPrompt(description);

                    var aiTask =  client.Models.GenerateContentAsync(model, fullPrompt);
                    var response = await WrapWithProgressBar(aiTask, "🤖 AI thinks about the ad...");
                    if (response.Candidates == null || response.Candidates.Count == 0)
                    {
                        return AiResultResponseFactory.CreateNoCandidatesResponse();
                    }
                    if (response.Candidates[0].Content == null)
                    {
                        return AiResultResponseFactory.CreateEmptyContentResponse();
                    }
                    if (response.Candidates[0].Content.Parts == null || response.Candidates[0].Content.Parts.Count == 0)
                    {
                        return AiResultResponseFactory.CreateEmptyContentResponse();
                    }
                    string aiJson = response.Candidates[0].Content.Parts[0].Text;

                    // Clean the JSON output from any potential markdown formatting
                    aiJson = aiJson.Replace("```json", "").Replace("```", "").Trim();

                    if (!string.IsNullOrEmpty(aiJson))
                    {
                        return AiResultResponseFactory.CreateSuccessResponse(aiJson);
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("quota") || ex.Message.Contains("429"))
                {
                    if (totalRetries >= apiKeys.Length)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        string fatalMsg = "❌ CRITICAL: All API keys have reached their quota limits.";
                        Console.WriteLine($"\n{fatalMsg}");
                        Console.ResetColor();

                        Console.WriteLine("All keys exhausted. Shutting down system. Please try again later.");

                        Console.WriteLine("\n[Press any key to exit the program]");

                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                    // SWITCH KEY: Handle Rate Limits (Free Tier Quota)
                    currentKeyIndex = (currentKeyIndex + 1) % apiKeys.Length;
                    Console.WriteLine($"🛑 Quota Limit Reached! Switching to API Key {currentKeyIndex + 1} and waiting...");
                    await Task.Delay(35000); // Wait 35 seconds to allow the quota to reset
                }
                catch (Exception ex) when (ex.Message.Contains("high demand"))
                {
                    // WAIT: Handle server overload
                    Console.WriteLine("⏳ Server is under high demand. Pausing for 20 seconds...");
                    await Task.Delay(20000);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Unexpected AI Error: {ex.Message}");
                    // Crucial: Always reset the color back to normal!
                    Console.ResetColor();
                    return AiResultResponseFactory.CreateUnknownErrorResponse(ex.Message);
                }

                totalRetries++;
            }

            Console.WriteLine("❌ Job analysis failed after maximum retry attempts.");
            return AiResultResponseFactory.CreateMaxRetriesResponse();
        }

        // A simple console-based progress bar to indicate that the AI is processing the job description.   
        private async Task<T> WrapWithProgressBar<T>(Task<T> task, string message)
        {
            Console.Write($"{message}\n[");
            int position = 0;
            int width = 30;
            bool forward = true;

            while (!task.IsCompleted)
            {
                Console.SetCursorPosition(1, Console.CursorTop);

                Console.Write(new string(' ', width));
                Console.SetCursorPosition(position + 1, Console.CursorTop);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("===>");
                Console.ResetColor();

                if (forward) position++; else position--;
                if (position >= width - 5) forward = false;
                if (position <= 0) forward = true;

                await Task.Delay(80);
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine(new string(' ', width + 2)); 
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine($"{message} Done! ✅");

            return await task;
        }
    }
}