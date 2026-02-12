using Microsoft.Win32;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Agent
{
    public class EdgeManager
    {
        /// <summary>
        /// Performs a progressive scroll on the job results list to trigger lazy loading.
        /// This ensures all job cards are rendered in the DOM before scraping begins.
        /// </summary>
        public static async Task ScrollResultsList(IWebDriver driver)
        {
            Console.WriteLine("⏳ Starting 'Senior Scroll' to load job results...");

            // 12 steps are usually enough to load around 25-100 job postings via lazy loading.
            for (int i = 0; i < 12; i++)
            {
                try
                {
                    // We use JavaScript to find the specific scrollable container used by LinkedIn's UI.
                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                var scrollContainer = document.querySelector('.jobs-search-results-list') || 
                                     document.querySelector('.scaffold-layout__list') ||
                                     document.querySelector('.jobs-search-results-display__list');
                
                if (scrollContainer) {
                    scrollContainer.scrollTop += 1500; // Scroll down by 1500 pixels
                } else {
                    window.scrollBy(0, 1000); // Fallback for different UI layouts
                }
            ");

                    // Wait for the browser to fetch and render new remote job entries.
                    await Task.Delay(2000);
                    Console.WriteLine($"⏬ Scroll step {i + 1}/12 completed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Scroll Error at step {i}: {ex.Message}");
                }
            }

            // Final short delay to ensure the DOM is fully updated.
            await Task.Delay(1000);

            Console.WriteLine("✅ Results list fully loaded.");
        }

        /// <summary>
        /// Compares the installed Microsoft Edge browser version with the EdgeDriver version.
        /// Ensures the major versions match to prevent Selenium initialization errors.
        /// </summary>
        /// <param name="actualDriverPath">The file path to the downloaded msedgedriver.exe.</param>
        public static void CheckRealVersionMatch(string actualDriverPath)
        {
            try
            {
                // 1. Locate the Edge browser executable via the Windows Registry
                string edgePath = (string)Microsoft.Win32.Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe", "", null);

                if (string.IsNullOrEmpty(edgePath))
                {
                    Console.WriteLine("❌ Error: Microsoft Edge browser path not found in Registry.");
                    return;
                }

                // 2. Extract version information from the browser and the driver files
                string browserVersion = FileVersionInfo.GetVersionInfo(edgePath).FileVersion;
                string driverVersion = FileVersionInfo.GetVersionInfo(actualDriverPath).FileVersion;

                Console.WriteLine($"🌐 Browser Version: {browserVersion}");
                Console.WriteLine($"⚙️ Driver Version:  {driverVersion}");

                // 3. Compare the Major version (the first part of the version string)
                if (browserVersion.Split('.')[0] == driverVersion.Split('.')[0])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ VERSION MATCH! The system is ready for automation.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️ VERSION MISMATCH: Major versions do not match. Stability may be affected.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during version verification: {ex.Message}");
            }
        }
      
        public static void TakeErrorScreenshot(IWebDriver driver, string actionName)
        {
            try
            {
                string fileName = $"Error_{actionName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                ITakesScreenshot screenshotDriver = (ITakesScreenshot)driver;
                screenshotDriver.GetScreenshot().SaveAsFile(path);
                Console.WriteLine($"📸 Screenshot saved: {fileName}");
            }
            catch (Exception ex) { Console.WriteLine($"⚠️ Screenshot failed: {ex.Message}"); }
        }

        /// <summary>
        /// Attempts to navigate to the next page of job results.
        /// Scrolls to the bottom of the list to reveal the pagination controls.
        /// </summary>
        /// <param name="driver">The IWebDriver instance.</param>
        /// <returns>True if navigation was successful; false if no more pages are found.</returns>
        public static async Task<bool> TryGoToNextPage(IWebDriver driver)
        {
            try
            {
                Console.WriteLine("📄 Attempting to navigate to the next page...");

                // 1. Scroll to the bottom of the sidebar to render the pagination buttons.
                // LinkedIn uses lazy loading for the pagination footer.
                var scrollablePanel = driver.FindElement(By.XPath("//div[contains(@class, 'jobs-search-results-list')] | //section[contains(@class, 'scaffold-layout__list')]"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollTop = arguments[0].scrollHeight;", scrollablePanel);

                // Wait for the UI to update
                await Task.Delay(2000);

                // 2. Search for the 'Next' button specifically by its aria-label as seen in the LinkedIn DOM.
                var nextButtons = driver.FindElements(By.XPath("//button[@aria-label='View next page']"));

                if (nextButtons.Count > 0 && nextButtons[0].Displayed)
                {
                    var nextButton = nextButtons[0];
                    Console.WriteLine("✅ 'Next' button found. Executing click...");

                    // Use JavaScript to scroll the button into the center of the view.
                    // This prevents overlapping elements (like chat popups) from blocking the click.
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", nextButton);
                    await Task.Delay(1000);

                    // Perform a JavaScript click for maximum reliability.
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", nextButton);

                    // 3. Wait for the new batch of remote job postings to load. [cite: 2026-01-08]
                    await Task.Delay(5000);
                    return true;
                }
                else
                {
                    Console.WriteLine("ℹ️ 'View next page' button not found. You have likely reached the last page.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Pagination Error: {ex.Message}");
            }

            return false;
        }
      
        public static void SavePageScreenshot(IWebDriver driver, int pageNumber)
        {
            try
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                string fileName = Path.Combine(PathsConfig.BrowserScreenshotsPath, $"Page_{pageNumber}_{DateTime.Now:HH-mm-ss}.png");
                ss.SaveAsFile(fileName);
                Console.WriteLine($"📸 Page {pageNumber} saved to disk.");
            }
            catch (Exception ex) { Console.WriteLine($"⚠️ Screenshot error: {ex.Message}"); }
        }
        /// <summary>
        /// Detects and dismisses cookie consent banners to clear the view for scraping.
        /// Supports multiple languages and button types (Reject/Deny).
        /// </summary>
        public static void HandleCookies(IWebDriver driver)
        {
            try
            {
                Console.WriteLine("🍪 Searching for cookie consent banner...");

                // We use a short wait (3 seconds) because the banner usually appears immediately.
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(3));

                var rejectButton = wait.Until(d => {
                    try
                    {
                        // XPath logic: Finds buttons with 'DENY' action or specific text in EN/BG.
                        return d.FindElement(By.XPath(
                            "//button[@action-type='DENY'] | " +
                            "//button[contains(text(), 'Reject')] | " +
                            "//button[contains(text(), 'Отказвам')] | " +
                            "//button[contains(text(), 'Decline')]"
                        ));
                    }
                    catch { return null; }
                });

                if (rejectButton != null && rejectButton.Displayed)
                {
                    rejectButton.Click();
                    Console.WriteLine("✅ Cookie banner dismissed (Rejected).");
                }
            }
            catch (Exception)
            {
                // If the banner doesn't appear within 3 seconds, we assume it's already handled or not present.
                Console.WriteLine("ℹ️ No cookie banner detected. Proceeding...");
            }
        }
       
        /// <summary>
        /// Checks for running Microsoft Edge processes and offers to terminate them.
        /// This is necessary to release the user profile lock for Selenium.
        /// </summary>
        public static void AskToCloseEdge()
        {
            // Search for all active Microsoft Edge processes
            var edgeProcesses = Process.GetProcessesByName("msedge");

            if (edgeProcesses.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"❓ Active Edge instances detected ({edgeProcesses.Length}). Close them automatically? (Y/N): ");
                Console.ResetColor();

                string response = Console.ReadLine()?.ToUpper();

                if (response == "Y")
                {
                    Console.WriteLine("System: Closing Edge processes...");
                    foreach (var p in edgeProcesses)
                    {
                        try
                        {
                            p.Kill();
                            // Wait up to 2 seconds for the process to exit cleanly
                            p.WaitForExit(2000);
                        }
                        catch (Exception ex)
                        {
                            // Silent catch if a process is already closed or access is denied
                            Debug.WriteLine($"Could not kill process: {ex.Message}");
                        }
                    }
                    Console.WriteLine("✅ All Edge processes terminated.");
                }
                else
                {
                    Console.WriteLine("⚠️ Warning: If Edge remains open, the automation might fail.");
                }
            }
        }
        /// <summary>
        /// Alerts the user when manual intervention is required.
        /// Triggers a visual console message and an audible beep sequence.
        /// </summary>
        /// <param name="message">The warning message to display.</param>
        public static void AlertUserForAction(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n🔔 ACTION REQUIRED: {message}");
            Console.ResetColor();

            // Emits 3 short audible beeps to catch the user's attention.
            for (int i = 0; i < 3; i++)
            {
                // Parameters: Frequency in Hertz (800Hz), Duration in milliseconds (500ms)
                Console.Beep(800, 500);

                // Short pause between beeps for clarity
                Thread.Sleep(100);
            }
        }
    }
}
