using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Core.Models
{
    public class RemoteJob
    {
        [Key]
        public int Id { get; set; }

        public string ExternalId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Technologies { get; set; }
        public string LanguageLevel { get; set; }
        public string WorkMode { get; set; }
        public double SalaryEUR { get; set; }
        public string Advice { get; set; }
        public int MatchScore { get; set; }
        public string CompanyOrigin { get; set; }
        public string JobUrl { get; set; }
        public DateTime DateSaved { get; set; } = DateTime.Now;
    }
}
               