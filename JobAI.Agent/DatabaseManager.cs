using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;

namespace JobAI.Agent
{
    public class DatabaseManager
    {
        // The database is stored in C:\Temp to ensure persistent storage across sessions.
        private readonly string _connectionString = "Data Source=C:\\Temp\\JobAI-DB\\Jobs.db";

        public DatabaseManager()
        {
            // 1. Ensure the directory exists before initializing the database.
            string folderPath = "C:\\Temp\\JobAI-DB";
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

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
                LanguageLevel TEXT,            -- Required English proficiency [cite: 2026-01-14]
                WorkMode TEXT,                 -- Remote, Hybrid, or On-site [cite: 2026-01-08]
                SalaryEUR REAL,                -- Salary converted to EUR [cite: 2026-01-14]
                AI_Advice TEXT,                -- Personalized career tip: 'Hey girl...'
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
        public void SaveToDb(string extId, string title, string company, string desc, AiResult ai)
        {
            using var conn = new SqliteConnection(_connectionString);

            // Mapping properties to SQL parameters for safe data insertion.
            var jobParams = new
            {
                id = extId,
                t = title,
                c = company,
                d = desc,
                tech = ai.Technologies,
                lang = ai.LanguageLevel,
                mode = ai.WorkMode,
                salary = ai.SalaryEUR,
                advice = ai.Advice,
                date = DateTime.Now
            };

            string sql = @"INSERT OR IGNORE INTO RemoteJobs 
              (ExternalId, Title, Company, Description, Technologies, 
               LanguageLevel, WorkMode, SalaryEUR, AI_Advice, DateSaved) 
              VALUES 
              (@id, @t, @c, @d, @tech, @lang, @mode, @salary, @advice, @date)";

            conn.Execute(sql, jobParams);

            Console.WriteLine($"✅ Job saved with full AI analysis in Jobs.db: {title} at {company}");
        }
    }
}
