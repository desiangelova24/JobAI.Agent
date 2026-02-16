using JobAI.Agent.Models;
using JobAI.Agent.UI;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace JobAI.Agent.Config
{
    public class ConfigValidator
    {
        /// <summary>
        /// Determines whether the system has an active internet connection by attempting to reach a well-known external
        /// server.
        /// </summary>
        /// <remarks>This method performs a network ping to a public DNS server (1.1.1.1) to check
        /// connectivity. A successful response indicates that the system is likely connected to the internet. Network
        /// conditions, firewalls, or restricted environments may affect the result.</remarks>
        /// <returns>true if the system can successfully contact the external server; otherwise, false.</returns>
        public static bool IsInternetAvailable()
        {
            try
            {
                using var ping = new Ping();
                // We check Cloudflare DNS (1.1.1.1) - one of the fastest in the world
                var reply = ping.Send("1.1.1.1", 3000);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
        private static string ReadSensitiveInput(string prompt)
        {
            Console.Write(prompt);
            string input = "";
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(true); // true означава, че символът няма да се появи в конзолата
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Remove(input.Length - 1);
                    Console.Write("\b \b"); // Изтрива последната звездичка от екрана
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    input += keyInfo.KeyChar;
                    Console.Write("*"); // Пише звезда вместо истинския символ
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine(); // Нов ред след натискане на Enter
            return input;
        }
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
        public static void CheckSystemRequirements()
        {
            Console.WriteLine("🔍 Checking system requirements...");
            // Check .NET version
            var version = Environment.Version;
            if (version.Major < 8)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ SYSTEM ERROR: This agent requires at least .NET 8.0 to run.");
                Console.WriteLine("👉 Please download the latest SDK from: https://dotnet.microsoft.com/download");
                Console.ResetColor();
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://dotnet.microsoft.com/download/dotnet/8.0",
                    UseShellExecute = true
                });
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine($"✅ .NET Version {version} detected. System requirements met.");
            }
        }
        public static AppSettings? LoadConfigFiles() 
        {
            try { 
                string jsonString = File.ReadAllText(PathsConfig.FullConfigPath); 
                using JsonDocument doc = JsonDocument.Parse(jsonString); 
                JsonElement root = doc.RootElement;
                string apiKey = root.GetProperty("Gemini").GetProperty("ApiKeys")[0].GetString() ?? ""; 
                string password = root.GetProperty("Credentials").GetProperty("Password").GetString() ?? "";
                string email = root.GetProperty("Credentials").GetProperty("Email").GetString() ?? "";
                return new AppSettings { GeminiApiKeys = [apiKey], LinkedInEmail = email, LinkedInPassword = password };
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"❌ Error loading config: {ex.Message}");
                return null;
            }
        }
        public static bool IsConfigValid()
        {
            if (!IsConfigFilePresent())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️  Config file '{PathsConfig.ConfigFileName}' not found! A template will be created.");
                Console.ResetColor();
                return false;
            }
            if (!IsConfigFileValid())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Config file '{PathsConfig.ConfigFileName}' is malformed! Please fix the JSON structure.");
                Console.ResetColor();
                return false;
            }
            if (!AreConfigFieldsPopulated())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Config file '{PathsConfig.ConfigFileName}' contains empty or placeholder fields! Please update it with your credentials.");
                Console.ResetColor();
                return false;
            }
            return true;
        }
        public static bool AreFieldsValid(string? apiKey, string? password, string? email)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("YOUR_"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ API Key cannot be empty or contain placeholder text! Please update it in the config file.");
                Console.ResetColor();
                return false;
            }
            if (string.IsNullOrWhiteSpace(email) || email.Contains("your-email"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Email cannot be empty or contain placeholder text! Please update it in the config file.");
                Console.ResetColor();
                return false;
            }
            if (string.IsNullOrWhiteSpace(password) || password.Contains("your-password"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Password cannot be empty or contain placeholder text! Please update it in the config file.");
                Console.ResetColor();
                return false;
            }
            if (!IsValidEmail(email))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Invalid email format! Please enter a valid email address in the config file.");
                Console.ResetColor();
                return false;
            }
            return true;
        }   
        public static void SaveConfig(string? apiKey, string? password, string? email)
        {
            var configData = new
            {
                Gemini = new { ApiKeys = new[] { apiKey, apiKey } },
                Credentials = new { Email = email, Password = password },
                Settings = new { Currency = "EUR", MaxApplicationsPerDay = 10 }
            };
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(configData, options);
                File.WriteAllText(PathsConfig.FullConfigPath, json);

                Console.WriteLine("\n✅ Configuration saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving config: {ex.Message}");
            }
        } 
        private static bool CreateDefaultConfig()
        {
            try
            {
                var defaultConfig = new
                {
                    Gemini = new { ApiKeys = new[] { "YOUR_KEY_HERE", "YOUR_SECOND_KEY_HERE" } },
                    Credentials = new { Email = "your-email@example.com", Password = "your-password" },
                    Settings = new { Currency = "EUR", MaxApplicationsPerDay = 10 }
                };

                string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(PathsConfig.FullConfigPath, json);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"✨ Created a new {PathsConfig.FullConfigPath} template for you!");
                Console.ResetColor();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("❌ Error: Access denied! Try running the app as Administrator.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"❌ Error: Could not write to disk. Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An unexpected error occurred: {ex.Message}");
            }
            return false;
        }
        public static bool IsConfigFilePresent()
        {
            return File.Exists(PathsConfig.ConfigFileName);
        }   
        public static bool IsConfigFileValid()
        {
            try
            {
                string jsonString = File.ReadAllText(PathsConfig.FullConfigPath);
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;
                if (!root.TryGetProperty("Gemini", out JsonElement gemini) ||
                    !gemini.TryGetProperty("ApiKeys", out JsonElement apiKeys) ||
                    apiKeys.GetArrayLength() < 2 ||
                    !root.TryGetProperty("Credentials", out JsonElement credentials) ||
                    !credentials.TryGetProperty("Email", out JsonElement email) ||
                    !credentials.TryGetProperty("Password", out JsonElement password))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool AreConfigFieldsPopulated()
        {
            try
            {
                string jsonString = File.ReadAllText(PathsConfig.FullConfigPath);
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;
                string primaryKey = root.GetProperty("Gemini").GetProperty("ApiKeys")[0].GetString() ?? "";
                string secondKey = root.GetProperty("Gemini").GetProperty("ApiKeys")[1].GetString() ?? "";
                string email = root.GetProperty("Credentials").GetProperty("Email").GetString() ?? "";
                string password = root.GetProperty("Credentials").GetProperty("Password").GetString() ?? "";
                if (string.IsNullOrWhiteSpace(primaryKey) || primaryKey.Contains("YOUR_") ||
                    string.IsNullOrWhiteSpace(email) || email.Contains("your-email") ||
                    string.IsNullOrWhiteSpace(secondKey) || secondKey.Contains("YOUR_") ||
                    string.IsNullOrWhiteSpace(password) || password.Contains("your-password"))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }   
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        internal static void RunFullSetup(VoiceAssistant voice)
        {
            if (!IsConfigFileValid())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ ERROR: 'appsettings.json' is not a valid JSON format!");
                Console.ResetColor();
                voice.SayMessage("Configuration format is invalid.");
                voice.SayMessage("Exiting application. Goodbye.");
                Environment.Exit(0);
            }
            if (!AreConfigFieldsPopulated())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ Configuration file exists but is EMPTY or has placeholders.");
                Console.WriteLine("👉 Please add your Gemini API keys and LinkedIn credentials.");
                Console.ResetColor();
                try
                {
                    string? email = string.Empty;
                    string? apiKey = string.Empty;

                    while (true)
                    {
                        apiKey = ReadSensitiveInput("🔑 Enter your Gemini API Key: ");
                        //Console.Write("🔑 Enter your Gemini API Key: ");
                        //apiKey = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(apiKey))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ API Key cannot be empty! Try again.");
                            voice.SayMessage("API Key cannot be empty. Please enter a valid key.");
                            Console.ResetColor();
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true)
                    {
                        email = ReadSensitiveInput("📧 Enter your LinkedIn Email: ");
                        //Console.Write("📧 Enter your LinkedIn Email: ");
                        //email = Console.ReadLine()?.Trim();
                        if (IsValidEmail(email))
                        {
                            Console.WriteLine("✅ Email looks good.");
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ Invalid email format! Try again.");
                            voice.SayMessage("That email doesn't look right. Please check it.");
                            Console.ResetColor();
                        }
                    }
                    Console.Write("🔒 Enter your LinkedIn Password: ");
                    //string? password = Console.ReadLine()?.Trim();
                    string password = ReadPassword();
                    SaveConfig(apiKey, password, email);
                }
                catch (Exception ex)
                {
                    voice.SayMessage("Configuration is invalid.");
                    Console.WriteLine($"👉 Please open it manually to enter your keys.");
                }
                //UIHelper.OpenGithubInstructions();
                return;
            }
        }
    }
}
