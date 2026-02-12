using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Text;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using System.Speech.Synthesis;
using SeleniumExtras.WaitHelpers;

namespace JobAI.Agent
{
    public class JobScanner
    {
        private readonly Random _random;
        private readonly DatabaseManager _db;
        private readonly VoiceAssistant _voice;
        public JobScanner()
        {
            _db = new DatabaseManager();
            _voice = new VoiceAssistant();
            _random = new();
        }

        public async Task Run()
        {
            Console.WriteLine("=== JobAI Agent 2026 - Start ===");

            // AUTOMATIC DRIVER MANAGEMENT:
            // This resolves version mismatch between Edge browser and EdgeDriver automatically.
            var driverPath = new DriverManager().SetUpDriver(new EdgeConfig(), VersionResolveStrategy.MatchingBrowser);

            // Validate versions by passing the path returned by DriverManager
            EdgeManager.CheckRealVersionMatch(driverPath);

            // BROWSER OPTIONS CONFIGURATION
            var options = new EdgeOptions();
            string profilePath = @"C:\Temp\JobAI_Profile"; // Persistent profile to stay logged in
            // DIRECTORY CHECK:
            // If the profile folder doesn't exist, create it to prevent errors.
            if (!Directory.Exists(profilePath))
            {
                Directory.CreateDirectory(profilePath);
                Console.WriteLine($"📂 Created new profile directory at: {profilePath}");
            }
            // HEADLESS MODE (Optional):
            // Uncomment these lines to run the browser in the background without a window.
            // options.AddArgument("--headless=new");
            // options.AddArgument("--disable-gpu");

            // STABILITY SETTINGS:
            // These arguments help prevent crashes and handle memory limits in automated environments.
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument($"user-data-dir={profilePath}");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            // PROCESS CLEANUP:
            // Close existing Edge instances to avoid "Profile in use" errors.
            EdgeManager.AskToCloseEdge();

            IWebDriver driver = new EdgeDriver(options);
            driver.Manage().Window.Maximize();

            try
            {
                try
                {
                    // NAVIGATION:
                    // Target URL: Remote .NET Developer positions in Bulgaria.
                    driver.Navigate().GoToUrl("https://www.linkedin.com/jobs/search/?keywords=.net%20developer&location=Bulgaria&f_WT=2");

                    // Wait for elements and handle the cookie consent banner
                    await Task.Delay(3000);
                    EdgeManager.HandleCookies(driver);

                    // LOGIN VERIFICATION:
                    // Check if session is active; if not, attempt automatic or manual login.
                    if (!IsUserLoggedIn(driver))
                    {
                        // 1. Attempt automated login
                        await LoginWithRetry(driver);

                        // 2. Secondary check
                        if (!IsUserLoggedIn(driver))
                        {
                            // 3. Failure - capture screenshot for debugging (Captcha or UI change)
                            EdgeManager.TakeErrorScreenshot(driver, "LoginFailed");

                            // 4. Alert user via audio and console
                            EdgeManager.AlertUserForAction("Automated login failed. Please sign in manually to continue.");
                            Console.WriteLine("⌨️ Waiting for manual login. Press ENTER when finished...");
                            Console.ReadLine();
                        }
                    }

                    _voice.Say("start"); // Voice notification: "System online..."

                    bool hasMorePages = true;
                    int currentPage = 1;

                    // Initial notification and screenshot for the first page
                    NotifyPageProcess(driver, currentPage);

                    // MAIN SCRAPING LOOP:
                    // Iterate through pagination until no more pages are found.
                    while (hasMorePages)
                    {
                        // Start processing job cards on the current page
                        await StartScraping(driver);

                        // Attempt to navigate to the next page
                        hasMorePages = await EdgeManager.TryGoToNextPage(driver);

                        if (hasMorePages)
                        {
                            currentPage++;
                            _voice.Say("page", currentPage); // Voice: "Moving to page X..."

                            // Visual and audio feedback for tracking progress
                            NotifyPageProcess(driver, currentPage);
                            Console.WriteLine($"🔄 Page {currentPage} loaded. Continuing scan...");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Runtime Error: {ex.Message}");
                    EdgeManager.TakeErrorScreenshot(driver, "Runtime_Error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Critical System Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("🏁 Process finished. You can now close the console.");
                // driver?.Quit(); // Kept commented to allow manual inspection of results
            }

            // FINAL NOTIFICATION:
            // Signal completion with a beep and voice message.
            Console.Beep(1200, 1000);
            Console.WriteLine("🏁 Program completed! Check your screenshots folder.");
            _voice.Say("finish"); // Voice: "Task completed!"
        }
        public async Task LoginWithRetry(IWebDriver driver)
        {
            int maxAttempts = 2; 
            int currentAttempt = 0;
            bool success = false;

            while (currentAttempt < maxAttempts && !success)
            {
                try
                {
                    currentAttempt++;
                    await TryAutomaticLogin(driver);

                    if (driver.Url.Contains("feed") || driver.Url.Contains("jobs"))
                    {
                        success = true;
                        Console.WriteLine("✅ Login successful on attempt " + currentAttempt);
                    }
                    else
                    {
                        throw new Exception("Still on login page.");
                    }
                }
                catch (Exception)
                {
                    if (currentAttempt < maxAttempts)
                    {
                        Console.WriteLine($"⚠️ Attempt {currentAttempt} failed. Refreshing and retrying...");
                        driver.Navigate().Refresh();
                        await Task.Delay(3000);
                    }
                    else
                    {
                        Console.WriteLine("❌ All login attempts failed.");
                        _voice.SayMessage("I couldn't log in. Please check if there is a CAPTCHA on the screen.");
                        EdgeManager.AlertUserForAction("Please complete the login manually.");
                    }
                }
            }
        }
        /// <summary>
        /// Attempts to perform an automated login using credentials from the secrets file.
        /// Uses randomized delays to mimic human behavior and avoid detection.
        /// </summary>
        private async Task TryAutomaticLogin(IWebDriver driver)
        {
            try
            {
                Console.WriteLine("🔑 Attempting automatic login...");
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // 1. Handling the initial 'Sign in' button
                var signInButtons = driver.FindElements(By.LinkText("Sign in"));
                if (signInButtons.Count > 0)
                {
                    Console.WriteLine("🌐 Clicking the 'Sign in' button...");
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", signInButtons[0]);

                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("username")));
                }

                // 2. Loading credentials
                var configFile = ConfigValidator.LoadConfigFiles();
                if (configFile == null)
                {
                    Console.WriteLine("⚠️ Error: Credentials missing in configuration!");
                    return;
                }

                // 3. Entering Email and Password
                var emailField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("username")));
                emailField.Clear();
                emailField.SendKeys(configFile.LinkedInEmail);

                var passwordField = driver.FindElement(By.Id("password"));
                passwordField.Clear();
                passwordField.SendKeys(configFile.LinkedInPassword);

                var loginButton = driver.FindElement(By.XPath("//button[@type='submit']"));
                loginButton.Click();

                await Task.Delay(3000);

                // 4. Handling 2FA Challenge (Two-Factor Authentication)
                if (driver.Url.Contains("/checkpoint/challenge/"))
                {
                    Console.WriteLine("🛡️ LinkedIn security: 2FA challenge detected!");
                    _voice.SayMessage("LinkedIn has sent a verification code. You have two minutes to enter it.");
                    EdgeManager.AlertUserForAction("Please enter the PIN code in the browser. The program will exit in 2 minutes if no action is taken.");

                    try
                    {
                        // Wait up to 2 minutes for the user to enter the PIN and the URL to change
                        var wait2fa = new WebDriverWait(driver, TimeSpan.FromMinutes(2));
                        wait2fa.Until(d => !d.Url.Contains("/checkpoint/challenge/"));
                        Console.WriteLine("✅ 2FA verification successful!");
                    }
                    catch (WebDriverTimeoutException)
                    {
                        // Action on Timeout
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("⏰ Security timeout: Verification time expired. Closing the browser...");
                        _voice.SayMessage("Security timeout. Closing the browser.");

                        driver.Quit();
                        Environment.Exit(0);
                    }
                }

                Console.WriteLine("⏳ Waiting for post-login redirection...");
                await Task.Delay(5000);
                Console.WriteLine("✅ Login process completed.");
            }
            catch (Exception ex)
            {
                EdgeManager.AlertUserForAction($"Automatic login failed: {ex.Message}");
            }
        }
        private void NotifyPageProcess(IWebDriver driver, int pageNumber)
        {
            string message = $"Processing page {pageNumber}. Taking screenshot.";
            _voice.SayMessage(message);
            EdgeManager.SavePageScreenshot(driver, pageNumber);

            Console.WriteLine($"🗣️ Processing page {pageNumber}. Notification sent.");
        }
        /// <summary>
        /// Orchestrates the scraping process: dynamically loads job cards, 
        /// extracts data, and performs AI analysis for each unique job post.
        /// </summary>
        private async Task StartScraping(IWebDriver driver)
        {
            Console.WriteLine("🚀 Initiating data extraction for job listings...");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                // 1. LOCATE THE RESULTS PANEL
                // We look for the scrollable container that holds all the job cards.
                var resultsPanel = wait.Until(d => d.FindElement(By.XPath(
                    "//div[contains(@class, 'jobs-search-results-list') or contains(@class, 'scaffold-layout__list')]"
                )));

                // 2. DYNAMIC LOADING (Lazy Loading Management)
                // LinkedIn renders cards as you scroll. We must reach our target count before processing.
                int targetCount = 25;
                int attempts = 0;
                var cards = resultsPanel.FindElements(By.ClassName("job-card-container"));

                Console.WriteLine($"✅ Dynamic loading started. Initial cards found: {cards.Count}");

                while (cards.Count < targetCount && attempts < 15)
                {
                    attempts++;
                    if (cards.Count > 0)
                    {
                        // Scroll to the last card found so far to trigger the next batch of results
                        IWebElement lastCard = cards.Last();
                        ((IJavaScriptExecutor)driver).ExecuteScript(
                            "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", lastCard);
                    }

                    // Randomized delay to mimic human scrolling and allow data fetching
                    await Task.Delay(_random.Next(2000, 3500));

                    // Refresh the list from the results panel
                    cards = resultsPanel.FindElements(By.ClassName("job-card-container"));
                    Console.WriteLine($"🔄 Attempt {attempts}: Loaded {cards.Count} of {targetCount}...");
                }

                Console.WriteLine($"🚀 Final card count for processing: {cards.Count}");

                // 3. JOB DATA EXTRACTION & AI ANALYSIS
                foreach (var card in cards)
                {
                    try
                    {
                        // Retrieve the unique LinkedIn ID to check for duplicates in the database
                        string extId = card.GetAttribute("data-job-id");
                        if (_db.IsAlreadySaved(extId)) continue;

                        // Click the card via JavaScript to display job details in the right pane
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", card);

                        // Wait for the job details pane to load
                        await Task.Delay(_random.Next(2500, 4000));

                        // Extract job information using precise CSS classes and IDs
                        string title = driver.FindElement(By.ClassName("job-details-jobs-unified-top-card__job-title")).Text;
                        string company = driver.FindElement(By.ClassName("job-details-jobs-unified-top-card__company-name")).Text;
                        string description = driver.FindElement(By.Id("job-details")).Text;

                        // AI ANALYSIS: Pass the description to Gemini for evaluation [cite: 2026-02-07]
                        var aiClient = new GeminiClient();
                        AiResult analysis = await aiClient.AnalyzeJob(description);

                        if (analysis == null)
                        {
                            Console.WriteLine($"⚠️ AI Analysis returned empty for: {title}");
                        }

                        // PERSISTENCE: Save to SQLite database with salary converted to EUR [cite: 2026-01-14]
                        _db.SaveToDb(extId, title, company, description, analysis);

                        Console.WriteLine($"✅ Saved to DB: {title} @ {company} | Score: {analysis?.MatchScore}%");
                    }
                    catch (Exception)
                    {
                        // If one card fails (e.g., ad or stale element), skip and continue with the next
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during scraping sequence: {ex.Message}");
            }
        }

        private bool IsUserLoggedIn(IWebDriver driver) => driver.FindElements(By.ClassName("global-nav__me-photo")).Count > 0;

    }
}