# JobAI - AzureCloudAgent 🚀

**AzureCloudAgent** is a specialized automation tool designed to discover and analyze specific job opportunities on LinkedIn. The agent is currently pre-configured to target .NET and C# developer roles located in Sofia, Bulgaria.

By leveraging Selenium for targeted scraping and Google Gemini AI for semantic evaluation, it identifies the most relevant positions and assesses how well they match the user's professional skill set.


## 🔍 Search Parameters
To ensure high-quality results, the agent uses the following automated filters:
* **Keywords:** `.NET`, `C#`
* **Location:** `Sofia, Bulgaria` (including Remote options)
* **Platform:** LinkedIn
* **AI Analysis:** Evaluation of job requirements against a .NET Developer profile.

## 🌟 Key Features

- **Automated Discovery:** Crawls job boards using **Selenium WebDriver** with Edge support.

- **AI-Powered Analysis:** Leverages **Google Gemini AI** to evaluate job descriptions, match skills, and summarize key requirements.

- **Enterprise Logging:** Implementation of a dedicated **Serilog** logging module integrated with **Azure Application Insights**.

- **Resilient Infrastructure:** Uses **Polly** policies for handling transient faults during API calls and web navigation.

- **Clean Architecture:** Organized into decoupled projects for better maintainability and scalability.

## 🛠 Tech Stack

- **Framework:** .NET 8

- **ORM:** Entity Framework Core (SQLite for local / SQL Server for Azure)

- **AI Integration:** Google GenAI (Gemini)

- **Automation:** Selenium WebDriver & WebDriverManager

- **Reliability:** Polly (Retry & Circuit Breaker patterns)

- **Mapping:** AutoMapper

- **Logging:** Serilog (Console & Azure Sinks)

## 🏗 Project Architecture & Namespaces

The solution is organized into several **Namespaces**, each representing a logical layer of the application. This ensures a clean separation of concerns and maintainability.

### 🧩 Core Modules (Namespaces)

* **JobAI.Agent** (Main Entry Point)

  - The orchestration layer. It manages the Selenium lifecycle, coordinates the scraping flow, and initiates the AI analysis.

  - *Contains:* Program.cs, background services, and job scheduling logic.

* **JobAI.Core** (Domain Layer)

  - The "heart" of the system. It defines the shared data models, database entities, and interfaces used across the entire solution.

  - *Contains:* JobOffer.cs, MappingProfiles, and repository interfaces.

* **JobAI.Logging** (Cross-Cutting Concern)

  - A dedicated namespace for structured logging. It integrates **Serilog** with **Azure Application Insights** to provide unified monitoring.

  - *Contains:* LoggingExtensions, ServiceRegistration.

* **JobAI.Infrastructure** (Service Layer - Private)

  - Handles external communications. It contains the implementation for the **Google Gemini AI** client and database persistence.

  - *Contains:* GeminiClient, DbContext, and external API wrappers.

## ⚙️ Local Configuration

To run this project locally, you must provide your own configuration settings.

1. **Create a development settings file:**

  In the JobAI.Agent project, create an appsettings.Development.json file.

2. **Add the following configuration:**

  ```json

  {

  "GeminiSettings": {

  "ApiKey": "YOUR_GEMINI_API_KEY_HERE"

  },

  "ConnectionStrings":

3. **Dependencies:**

Ensure that the private assembly JobAI.Infrastructure.dll is placed in the /Libs folder of the main project.

### 📦 Key Libraries & Dependencies

To ensure high performance and reliability, the following enterprise-grade libraries are used:

* **Google.GenAI**

  * **The "brain" of the AI. Facilitates communication with Google's Gemini API for deep analysis of job descriptions.

* **Data & Persistence:**

  * **Entity Framework Core:** Modern Object-Database Mapper for seamless data access.

  * **Newtonsoft.Json:** High-performance JSON framework for parsing AI responses and web data.

* **Resilience & Stability:**

  * **Polly:** Implementation of transient-fault-handling policies like **Retry** and **Circuit Breaker** for robust API communications.

* **Configuration & Mapping:**

  * **AutoMapper:** Simplifies object-to-object mapping, reducing manual boilerplate code.

  * **Microsoft.Extensions.Options:** Enables strongly-typed access to configuration settings from appsettings.json.

* **Automation & Scraper:**

  * **Selenium WebDriver:** Core engine for browser automation.

  * **WebDriverManager:** Automatically manages the lifecycle and versioning of the Edge driver.

### 🚀 Future Roadmap (Next Steps)

Azure Integration: Move the local SQLite database to Azure SQL Database and deploy the agent as an Azure WebJob or Azure Function.
Web Dashboard: Develop a Blazor or React frontend to visualize the job analysis results in real-time.
Notifications: Integrate Azure Communication Services to send SMS or Email alerts when a "High Match" job is found by Gemini.
Containerization: Add a Dockerfile to run the entire agent inside a Docker container for easier deployment to Azure Container Instances (ACI).