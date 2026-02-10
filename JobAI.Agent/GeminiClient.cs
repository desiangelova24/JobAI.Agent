using JobAI.Agent;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Google.GenAI;
using System.Linq;
public class GeminiClient
{
    private string[] _apiKeys;
    public GeminiClient()
    {
        LoadKeys();
    }
    private void LoadKeys()
    {
        try
        {
            if (File.Exists("secrets.txt"))
            {
                string[] allLines = File.ReadAllLines("secrets.txt");

                _apiKeys = allLines.Take(2).ToArray();

                Console.WriteLine($"✅ Loaded {_apiKeys.Length} API keys for Gemini.");
            }
            else
            {
                _apiKeys = new string[] { "" };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading keys: {ex.Message}");
        }
    }
    /// <summary>
    /// Sends the job description to Gemini AI for analysis.
    /// Implements API key rotation and retry logic to handle rate limits and server demand.
    /// </summary>
    public async Task<AiResult> AnalyzeJob(string description)
    {
        var model = "gemini-3-flash-preview"; // Core model
        int currentKeyIndex = 0;
        int totalRetries = 0;

        while (totalRetries < 5)
        {
            try
            {
                // Initialize the AI client with the current active key
                var client = new Client(apiKey: _apiKeys[currentKeyIndex]);
                string fullPrompt = PromptFactory.GetJobAnalysisPrompt(description);

                var response = await client.Models.GenerateContentAsync(model, fullPrompt);
                string aiJson = response.Candidates[0].Content.Parts[0].Text;

                // Clean the JSON output from any potential markdown formatting
                aiJson = aiJson.Replace("```json", "").Replace("```", "").Trim();

                if (!string.IsNullOrEmpty(aiJson))
                {
                    return JsonConvert.DeserializeObject<AiResult>(aiJson);
                }
            }
            catch (Exception ex) when (ex.Message.Contains("quota") || ex.Message.Contains("429"))
            {
                if (totalRetries >= _apiKeys.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    string fatalMsg = "❌ CRITICAL: All API keys have reached their quota limits.";
                    Console.WriteLine($"\n{fatalMsg}");
                    Console.ResetColor();

                    var voice = new VoiceAssistant();
                    voice.SayMessage("All keys exhausted. Shutting down system. Please try again later.");

                    Console.WriteLine("\n[Press any key to exit the program]");
                    
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                // SWITCH KEY: Handle Rate Limits (Free Tier Quota)
                currentKeyIndex = (currentKeyIndex + 1) % _apiKeys.Length;
                Console.WriteLine($"🛑 Quota Limit Reached! Switching to API Key {currentKeyIndex - 1} and waiting...");
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
                return null;
            }

            totalRetries++;
        }

        Console.WriteLine("❌ Job analysis failed after maximum retry attempts.");
        return null;
    }
}