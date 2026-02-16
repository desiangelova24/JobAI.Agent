

using JobAI.Agent.Config;
using JobAI.Agent.Models;
using JobAI.Agent.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace JobAI.Agent.Services
{
    public class JobScanner
    {
        private readonly Random _random;
        private readonly VoiceAssistant _voice;
        private readonly JobProcessor _processor;   
        public JobScanner(JobProcessor processor, VoiceAssistant voice)
        {
            _voice = voice;
            _random = new Random();
            _processor = processor;
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
            // HEADLESS MODE (Optional):
            // Uncomment these lines to run the browser in the background without a window.
            // options.AddArgument("--headless=new");
            // options.AddArgument("--disable-gpu");

            // STABILITY SETTINGS:
            // These arguments help prevent crashes and handle memory limits in automated environments.
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument($"user-data-dir={PathsConfig.BrowserProfile}");
            options.AddArgument("profile-directory=Default");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-blink-features=AutomationControlled");

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
                    //driver.Navigate().GoToUrl(PathsConfig.Pathurl);
                    driver.Navigate().GoToUrl("https://www.linkedin.com/jobs");

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
                    driver.Navigate().GoToUrl(PathsConfig.Pathurl);
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
                        //hasMorePages = await EdgeManager.TryGoToNextPage(driver);

                        //if (hasMorePages)
                        //{
                        //    currentPage++;
                        //    _voice.Say("page", currentPage); // Voice: "Moving to page X..."

                        //    // Visual and audio feedback for tracking progress
                        //    NotifyPageProcess(driver, currentPage);
                        //    Console.WriteLine($"🔄 Page {currentPage} loaded. Continuing scan...");
                        //}
                        break;
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


        //private bool IsUserLoggedIn(IWebDriver driver) => driver.FindElements(By.ClassName("global-nav__me-photo")).Count > 0;
        private bool IsUserLoggedIn(IWebDriver driver)
        {
            try
            {
                var homeIcon = driver.FindElements(By.CssSelector("[data-view-name='navigation-homepage']"));

                var navContainer = driver.FindElements(By.ClassName("global-nav__content"));

                return homeIcon.Count > 0 || navContainer.Count > 0;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Orchestrates the scraping process: dynamically loads job cards, 
        /// extracts data, and performs AI analysis for each unique job post.
        /// </summary>

        private async Task StartScraping(IWebDriver driver)
        {
            Console.WriteLine("🚀 Initiating data extraction...");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                var cards = await LoadJobCards(driver, wait);
  
                foreach (var card in cards)
                {
                    string extId = card.GetAttribute("data-job-id");

                    var exitsJobId = _processor.CheckIfJobExists(extId);
                    if (exitsJobId)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"⚠️ Skipping already processed job ID: {extId}");
                        Console.ResetColor();
                        continue;
                    }
                    // Click the card via JavaScript to display job details in the right pane
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", card);
                    // Wait for the job details pane to load
                    await Task.Delay(_random.Next(2500, 4000));

                    // Extract job information using precise CSS classes and IDs
                    string title = driver.FindElement(By.ClassName("job-details-jobs-unified-top-card__job-title")).Text;
                    string company = driver.FindElement(By.ClassName("job-details-jobs-unified-top-card__company-name")).Text;
                    string description = driver.FindElement(By.Id("job-details")).Text;
                
                    // 2. CHECK: Is there anything empty or too short
                    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] Failed to retrieve data from LinkedIn. Skipping this ad..");
                        Console.ResetColor();
                        return;
                    }
                    string jobUrl = "Unknown Link";
                    try
                    {
                        var linkElement = driver.FindElement(By.CssSelector(".jobs-search-results-list__list-item--active a.job-card-list__title, .jobs-search-results-list__list-item--active a"));
                        jobUrl = linkElement.GetAttribute("href");

                        if (jobUrl.Contains("?"))
                        {
                            jobUrl = jobUrl.Split('?')[0];
                        }
                    }
                    catch
                    {
                        jobUrl = driver.Url.Split('?')[0];
                    }
                    
                    await _processor.ProcessJob(extId, title, company, description, jobUrl);

                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during scraping sequence: {ex.Message}");
            }
        }

       /// <summary>
       /// Asynchronously loads job card elements from the search results panel on the current page.    
       /// </summary>
       /// <remarks>This method attempts to load up to 25 job card elements by scrolling and waiting for
       /// them to appear. It may return fewer cards if the page does not contain enough job cards or if loading is
       /// incomplete after several attempts.</remarks>
       /// <param name="driver">The web driver instance used to interact with the browser and locate elements.</param>
       /// <param name="wait">The WebDriverWait instance used to wait for the search results panel to become available.</param>
       /// <returns>A read-only collection of IWebElement objects representing the loaded job cards. The collection may contain
       /// fewer than 25 elements if not all cards are available within the allowed attempts.</returns>
        private async Task<ReadOnlyCollection<IWebElement>> LoadJobCards(IWebDriver driver, WebDriverWait wait)
        {
            // 1. LOCATE THE RESULTS PANEL
            // We look for the scrollable container that holds all the job cards.
            var resultsPanel = wait.Until(d => d.FindElement(By.XPath(
                  "//div[contains(@class, 'jobs-search-results-list') or contains(@class, 'scaffold-layout__list')]"
              )));

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
            return cards;
        }
    }
}