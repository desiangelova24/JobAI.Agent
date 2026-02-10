using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Agent
{
    public class AiResult
    {
        // Technologies identified by AI (e.g., "C#, .NET, SQL Server, Docker")
        public string Technologies { get; set; }

        // Required English proficiency (e.g., "B2 - Upper Intermediate")
        public string LanguageLevel { get; set; }

        // Work setting: Remote, Hybrid, or On-site [cite: 2026-01-08]
        public string WorkMode { get; set; }

        // Extracted salary converted to EUR [cite: 2026-01-14]
        public double SalaryEUR { get; set; }

        // Compatibility score from 1 to 100 based on your profile
        public int MatchScore { get; set; }

        // AI's personalized career coaching advice
        public string Advice { get; set; }
    }
}
