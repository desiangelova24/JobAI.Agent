# 🚀 JobAI Hunter Pro

**JobAI Hunter Pro** is an automated C# application designed to scan LinkedIn for remote job opportunities, analyze them using Gemini AI, and store the results in a local database.

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

## 🚦 How it Works

The agent follows a structured execution flow to ensure data integrity and security:

1. **Initialization:** The system greets the user via voice alerts and verifies the presence of the `appsettings.json` configuration file.
2. **Validation:** If the configuration is missing or invalid, the bot enters a smart retry loop, waiting for the user to provide the necessary credentials before proceeding.
3. **Operation:** * The bot launches the browser and performs a secure LinkedIn login.
    * It scrapes job postings based on "Remote" and "EUR" salary filters.
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

## 🚀 How to Run

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/desiangelova24/JobAI.Agent.git](https://github.com/desiangelova24/JobAI.Agent.git)

2. Initial Launch: Run the app once to let it create the folder structure.

3. Configure Credentials: Open the newly created secrets.txt or appsettings.json and enter your LinkedIn details and API keys.

Build & Run:

dotnet run

## 🛡️ License
Private project for personal use and remote work career development.

## 📌 Current Status
**Version:** 1.0.0 (Stable Release)
**Last Update:** February 2026


### 📬 Contact & Support
If you have any questions or suggestions regarding the **JobAI Hunter Pro**, feel free to reach out via GitHub issues or email. 

*Let's automate the future of work together!* 🚀
