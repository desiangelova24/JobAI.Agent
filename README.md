# 🚀 JobAI Hunter Pro

**JobAI Hunter Pro** is an automated C# application designed to scan LinkedIn for job opportunities, analyze them using Gemini AI, and store the results in a local database.

---

## ✨ Key Features
* **Automated Scraping:** Uses Selenium with Edge to find relevant job postings.
* **AI Analysis:** Integrates Google Gemini AI to evaluate job descriptions.
* **Voice Notifications:** Includes a `VoiceAssistant` that provides real-time audio feedback.
* **Smart Security:** Validates configuration files and handles API rotation for the Free Tier.
* **Database Integration:** Saves all leads in a structured format for easy tracking.
 **Smart Login & 2FA:** Automated LinkedIn authentication with built-in Two-Factor Authentication (2FA) support and a 2-minute security timeout.
---

## 🛠 Tech Stack

* **Language:** C# (.NET 8/9)
* **Automation:** Selenium WebDriver (Microsoft Edge)
* **AI Engine:** Google Gemini API
* **Database:** SQLite
* **Configuration:** JSON-based secrets management (`appsettings.json`)

---

## 📦 Core Dependencies (NuGet Packages)

* **Selenium.WebDriver:** For browser automation.
* **Google.GenerativeAI:** For Gemini AI integration.
* **Newtonsoft.Json:** For robust configuration parsing.
* **Microsoft.Data.Sqlite:** For efficient local data storage.

---

## ⚙️ How It Works

The **JobAI Hunter Pro** is designed with a "plug-and-play" architecture, focusing on automation, security, and ease of use.

---
### 📁 Core Logic
The core business logic is provided as a compiled library (**JobAI.Core.dll**) located in the `/Libs` folder. 
This ensures the agent can function while keeping the core implementation private.

### 📂 1. Automated Infrastructure
Upon the first launch, the system utilizes a centralized `PathsConfig` engine to:
* **Generate Workspaces**: Automatically creates a secure `temp` directory and a dedicated database folder (`JobAI-DB`).
* **Configuration Scaffolding**: Detects if `appsettings.json` is missing and generates a fresh template, ensuring the app never crashes due to missing files.

### 🛡️ 2. Interactive Setup & Security
To protect your privacy, especially during remote screen-sharing sessions:
* **Masked Input**: LinkedIn passwords are collected through a secure console buffer that displays asterisks (`****`) instead of plain text.
* **Validation Layer**: The `ConfigValidator` ensures all entered data (Emails, API Keys) is logically correct before the scanning engine starts.

## 💶 3. Smart Search & Currency

* **Multi-Currency Support:** Automatically detects and converts job salaries between **BGN** and **EUR** using 2026 fixed exchange rates.
* **Smart Salary Filtering:** Gemini AI analyzes job descriptions to extract "hidden" salary ranges and translates them into a standardized European format.
* **Remote-First Intelligence:** Specifically targets high-paying remote positions within the Eurozone, calculating the net value based on Bulgarian tax logic.
* **Future-Proof Logic:** Built-in support for the Euro transition, making the tool ready for the official currency change.

### ⌨️ 4. Advanced Control (CLI)
For power users, the app supports command-line arguments to manage the environment quickly:
* `dotnet run -- -clean`: Resets the entire environment and deletes all local data (temp folder).
* `dotnet run -- -help` : Displays the interactive help menu with all available commands.

---
## 🚦 How it Works

The agent follows a structured execution flow to ensure data integrity and security:

1. **Initialization:** 
* **Environment Check:** Automatically verifies the existence of the `temp` configuration folder.
* **Auto-Recovery:** If the `temp` folder or required config files are missing, the system prompts for secure setup.
2. **Validation:** If the configuration is missing or invalid, the bot enters a smart retry loop, waiting for the user to provide the necessary credentials before proceeding.
3. **Operation:** * The bot launches the browser and performs a secure LinkedIn login.
    * It scrapes job postings based on ".NET & C#" and "Bulgaria" and "Remote".
    * Data is sent to the **Gemini AI** engine for deep requirement analysis.
4. **Quota & Error Handling:** * If an AI API limit is reached, the system automatically cycles through backup keys.
    * It includes a built-in 35-second cooldown period to respect rate limits and ensure continuous operation.

---
### 📂 Automatic Environment Setup
To simplify the first-time setup, the agent features an automated file system manager:
* **Auto-Configuration:** On the first run, the system automatically generates the necessary `appsettings.json` files if they are missing.
* **Workspace Management:** All operational and temporary files are organized within a dedicated `temp` folder to keep your project directory clean.
* **Persistence:** The system ensures that the **SQLite** database and log files are correctly initialized before starting any automation tasks.

---
## 🛠️ Troubleshooting & Common Issues

### 🌐 1. Browser version mismatch
**Issue:** The bot fails to launch or gives a "Driver version" error.
**Solution:** Ensure your Microsoft Edge is up to date. The `EdgeDriver` should match your browser version. The program is designed to use the latest stable release.

### 🛡️ 2. LinkedIn 2FA Pop-up
**Issue:** The bot stops and asks for a code.
**Solution:** This is a security feature. Look at the console log; you will hear a voice alert. You have 120 seconds to manually enter the code from your email before the session terminates for security.

### 🔑 3. Login Failed (Invalid Credentials)
**Issue:** Automation stops at the login screen.
**Solution:** Check your `appsettings.json`. Make sure your email and password are correct and that there are no extra spaces.

### 🕒 4. Timeout Exceptions
**Issue:** The program crashes with a "TimeoutException".
**Solution:** This usually happens due to a slow internet connection. The bot is set to wait for 10 seconds for elements to appear. If your connection is slow, try increasing the wait time in `TryAutomaticLogin`.

---


## 🚀 Future Roadmap

I am constantly working to make **JobAI Hunter Pro** even smarter. Planned features include:

* **📧 Real-time Email Alerts**: Automatically send an email notification as soon as a high-match job is found.
* **📊 Salary Benchmarking**: Compare found salaries in **EUR** with market averages to help during negotiations.
* **📄 AI Cover Letter Generator**: Use Gemini AI to draft a tailored cover letter based on the specific job description and your CV.
* **🌐 Multi-platform Support**: Expand beyond LinkedIn to include platforms like Indeed and remote-specific boards (WeWorkRemotely, RemoteOK).

## 🛡️ License
Private project for personal use and work career development.

## 📌 Current Status
**Version:** 1.1.0 (Stable Release)
**Last Update:** February 2026


### 📬 Contact & Support
If you have any questions or suggestions regarding the **JobAI Hunter Pro**, feel free to reach out via GitHub issues or email. 

*Let's automate the future of work together!* 🚀
