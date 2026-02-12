using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Agent
{
    public class AppSettings
    {

        public required string LinkedInEmail { get; set; } 
        public required string LinkedInPassword { get; set; } 
        public int MaxPagesToScan { get; set; } = 5;
        public string [] GeminiApiKeys { get; set; } = [""]; 
    }
}
