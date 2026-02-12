🚀 JobAI Hunter Pro

Automated Backend Agent for the 2026 Job Market



JobAI Hunter Pro is an intelligent C# agent designed to automate job search and description analysis. The project is fully optimized for the new economic landscape in Bulgaria (following the EUR adoption on January 1st, 2026) and focuses exclusively on Remote positions.



✨ Key Features

🤖 Smart Login \& 2FA: Automated LinkedIn authentication with built-in Two-Factor Authentication (2FA) support and a 2-minute security timeout.



🎯 AI-Powered Analysis: Deep integration with Gemini AI for intelligent parsing of job requirements and skill matching.



💶 EUR Integration: Full support for salary tracking and conversion in Euro (Standard for Bulgaria since 2026).



🏠 Remote-First Focus: Specialized filters for remote work to save time, commuting costs, and office-related expenses.



📊 Robust English Logging: Comprehensive real-time logging in English for tracking every stage of the bot's execution.



🗄️ Data Management: SQLite integration for persistent storage of application history and job status.



🛠 Tech Stack

Language: C# (.NET 8/9)



Automation: Selenium WebDriver (Microsoft Edge)



AI Engine: Google Gemini API



Database: SQLite



Configuration: JSON-based secrets management (appsettings.json)



\## 📦 Core Dependencies (NuGet Packages)



To ensure high performance and reliability, this project utilizes the following industry-standard libraries:



\* \*\*Selenium.WebDriver (v4.x):\*\* For browser automation and handling web elements.

\* \*\*Selenium.Support:\*\* Provides additional utilities like `ExpectedConditions` for robust waits.

\* \*\*Google.GenerativeAI:\*\* The official SDK for integrating the \*\*Gemini AI\*\* engine.

\* \*\*Newtonsoft.Json:\*\* For high-speed JSON parsing of configuration and data files.

\* \*\*Microsoft.Data.Sqlite:\*\* A lightweight ADO.NET provider for the \*\*SQLite\*\* database.

\* \*\*System.Speech:\*\* Powers the voice assistant for real-time audio notifications.



🚀 How to Run



1. Clone the repository:

git clone https://github.com/desiangelova24/JobAI.Agent.git



2\. Configure Credentials: Update appsettings.json or secrets.txt with your LinkedIn credentials and API keys.



3\. Build \& Run:



&nbsp;	dotnet run

