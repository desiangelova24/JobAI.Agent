using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Agent
{
    public class AppSettings
    {
        /// <summary>
        /// User email address for LinkedIn authentication.
        /// </summary>
        public required string LinkedInEmail { get; set; }

        /// <summary>
        /// Password for LinkedIn. Note: Keep this secure and never push to GitHub.
        /// </summary>
        public required string LinkedInPassword { get; set; }

        /// <summary>
        /// The maximum number of search result pages the bot will scrape per run.
        /// </summary>
        public int MaxPagesToScan { get; set; } = 5;

        /// <summary>
        /// Array of Google Gemini API keys to handle quota limits and rotating requests.
        /// </summary>
        public string[] GeminiApiKeys { get; set; } = [""];
    }
}
