using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Infrastructure
{
    /// <summary>
    /// Provides factory methods for creating standardized AI result response messages in JSON format.
    /// </summary>
    /// <remarks>This static class is intended to simplify the generation of consistent response payloads for
    /// AI-related operations, including success and various error scenarios. All methods return a JSON-serialized
    /// string representing an AI result response, which can be used for API responses or logging. The returned JSON
    /// structure is based on the AiResultResponse type.</remarks>
    public static class AiResultResponseFactory
    {
        public static string CreateNoKeysResponse()
        {
            var response = new AiResultResponse
            {
                Success = false,
                ErrorType = "NO_KEYS",
                Message = "Keys are missing!"
            };
            return System.Text.Json.JsonSerializer.Serialize(response);
        }
        public static string CreateUnknownErrorResponse(string errorMessage)
        {
            var response = new AiResultResponse
            {
                Success = false,
                ErrorType = "UNKNOWN_ERROR",
                Message = $"An unknown error occurred: {errorMessage}"
            };
            return System.Text.Json.JsonSerializer.Serialize(response);
        }
        public static string CreateMaxRetriesResponse()
        {
            var response = new AiResultResponse
            {
                Success = false,
                ErrorType = "MAX_RETRIES",
                Message = "Job analysis failed after maximum number of attempts."
            };
            return System.Text.Json.JsonSerializer.Serialize(response);
        }
        public static string CreateSuccessResponse(string aiJson)
        {
            var response = new AiResultResponse
            {
                Success = true,
                ErrorType = string.Empty,
                Message = aiJson
            };
            return System.Text.Json.JsonSerializer.Serialize(response);
        }
        //AI response content is empty or malformed
        public static string CreateEmptyContentResponse()
        {
            var response = new AiResultResponse
            {
                Success = false,
                ErrorType = "EMPTY_CONTENT",
                Message = "AI response content is empty or malformed."
            };
            return System.Text.Json.JsonSerializer.Serialize(response);
        }
        //No response candidates received from AI
        public static string CreateNoCandidatesResponse()
        {
            var response = new AiResultResponse
            {
                Success = false,
                ErrorType = "NO_CANDIDATES",
                Message = "No response candidates received from AI."
            };
            return System.Text.Json.JsonSerializer.Serialize(response);
        }
    }
}
