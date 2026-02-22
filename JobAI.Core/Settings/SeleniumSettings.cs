using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Core.Settings
{
    public class SeleniumSettings
    {
        public string ChromeDriverPath { get; set; } = string.Empty;
        public string BrowserScreenshotsPath { get; set; } = string.Empty;
        public string LogsFolder { get; set; } = string.Empty;
        public string BrowserProfile { get; set; } = string.Empty;
        public bool IsVoiceEnabled { get; set; }
        public bool Headless { get; set; }
    }
}
