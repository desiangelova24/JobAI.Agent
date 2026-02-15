
using Dapper;
using JobAI.Agent.Config;
using JobAI.Agent.Models;
using Microsoft.Data.Sqlite;

namespace JobAI.Agent.Services
{
    public class DatabaseManager
    {
        // The database is stored in Temp to ensure persistent storage across sessions.
        private string _connectionString;
        public DatabaseManager()
        {
            _connectionString = $"Data Source={PathsConfig.DatabaseFile}";
            // 2. Initialize the table with specific fields for AI analysis and job requirements.
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
            CREATE TABLE IF NOT EXISTS RemoteJobs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ExternalId TEXT UNIQUE,        -- Unique LinkedIn Job ID
                Title TEXT,                    -- Job Title
                Company TEXT,                  -- Company Name
                Description TEXT,              -- Raw job description for AI re-processing
                Technologies TEXT,             -- Extracted stack (C#, SQL, Docker...)
                LanguageLevel TEXT,            -- Required English proficiency
                WorkMode TEXT,                 -- Remote, Hybrid, or On-site
                SalaryEUR REAL,                -- Salary converted to EUR
                AI_Advice TEXT,                -- Personalized career tip: 'Hey girl...'
                CompanyOrigin TEXT,            -- 'English' or 'International'
                JobUrl TEXT,                   -- URL of the job posting for reference    
                DateSaved DATETIME             -- Timestamp of the scan
            )");

            Console.WriteLine("✅ Database initialized and ready for job entries.");
        }

        /// <summary>
        /// Checks if a job has already been processed to avoid duplicates.
        /// </summary>
        public bool IsAlreadySaved(string externalId)
        {
            using var connection = new SqliteConnection(_connectionString);

            // Using Dapper to count existing entries with the same ExternalId.
            string sql = "SELECT COUNT(1) FROM RemoteJobs WHERE ExternalId = @externalId";
            int count = connection.ExecuteScalar<int>(sql, new { externalId });

            return count > 0;
        }

        /// <summary>
        /// Saves the processed job along with its AI-generated analysis.
        /// </summary>
        public async Task SaveJobAsync(string extId, string title, string company, string desc, AiResult ai, string jobUrl)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                                    INSERT INTO RemoteJobs (ExternalId, Title, Company, Description, Technologies, LanguageLevel, WorkMode, SalaryEUR, AI_Advice,CompanyOrigin,JobUrl, DateSaved)
                                    VALUES (@extId, @title, @company, @desc, @tech, @lang, @mode, @salary, @advice,@companyOrigin,@jobUrl, @date)";

                    command.Parameters.AddWithValue("@extId", extId);
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@company", company);
                    command.Parameters.AddWithValue("@desc", desc);
                    command.Parameters.AddWithValue("@tech", ai.Technologies);
                    command.Parameters.AddWithValue("@lang", ai.LanguageLevel);
                    command.Parameters.AddWithValue("@mode", ai.WorkMode);
                    command.Parameters.AddWithValue("@salary", ai.SalaryEUR);
                    command.Parameters.AddWithValue("@advice", ai.Advice);
                    command.Parameters.AddWithValue("@companyOrigin", ai.CompanyOrigin);
                    command.Parameters.AddWithValue("@jobUrl", jobUrl);
                    command.Parameters.AddWithValue("@date", DateTime.Now);
                  
                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("✅ Success: Job saved to database.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("❌ Database Error: " + ex.Message);
                    }
                }
               
            }
        }
        public async Task<bool> IsAlreadySavedAsync(string extId)
        {
            return await Task.Run(() => IsAlreadySaved(extId)); 
        }
    }
}
