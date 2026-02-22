using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Core.Interfaces
{
    public interface IGeminiClient
    {
        Task<string> AnalyzeJobAsync(string description);
    }
}
