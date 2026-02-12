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
1.  **Initialization:** The system greets the user via voice and checks for `secrets.txt`.
2.  **Validation:** If the config is missing, it enters a retry loop until the file is created.
3.  **Operation:** The bot logs into LinkedIn, scrapes jobs, and sends data to Gemini.
4.  **Quota Handling:** If an API limit is reached, it automatically switches keys and waits for 35 seconds.

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
## 🛡️ License
Private project for personal use and remote work career development.

## 📌 Current Status
**Version:** 1.0.0 (Stable Release)
**Last Update:** February 2026


### 📬 Contact & Support
If you have any questions or suggestions regarding the **JobAI Hunter Pro**, feel free to reach out via GitHub issues or email. 

*Let's automate the future of work together!* 🚀
