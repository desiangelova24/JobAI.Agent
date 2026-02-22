using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Infrastructure
{
    /// <summary>
    /// Represents the result of an AI operation, including success status, error categorization, and a descriptive
    /// message.
    /// </summary>
    /// <remarks>Use this class to convey the outcome of AI-related requests. The properties provide
    /// information for error handling and user feedback, allowing clients to determine whether the operation succeeded
    /// and, if not, the nature and context of the failure.</remarks>
    public class AiResultResponse
    {
        // It includes a success flag, an error type for any issues encountered, and a message for additional context.
        public bool Success { get; set; }
        // The 'ErrorType' field can be used to categorize errors (e.g., 'NO_KEYS', 'QUOTA_LIMIT', 'HIGH_DEMAND') for better error handling and user feedback.  
        public required string ErrorType { get; set; }
        // The 'Message' field provides a human-readable explanation of the error or additional information about the response.
        // This can be used to inform the user about what went wrong or what actions to take next. 
        public required string Message { get; set; } 
    }
  
}
