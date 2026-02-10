# 🚀 JobAI Hunter Pro

**JobAI Hunter Pro** is an automated C# application designed to scan LinkedIn for remote job opportunities, analyze them using Gemini AI, and store the results in a local database.

---

## ✨ Key Features
* **Automated Scraping:** Uses Selenium with Edge to find relevant job postings.
* **AI Analysis:** Integrates Google Gemini AI to evaluate job descriptions.
* **Voice Notifications:** Includes a `VoiceAssistant` that provides real-time audio feedback.
* **Smart Security:** Validates configuration files and handles API rotation for the Free Tier.
* **Database Integration:** Saves all leads in a structured format for easy tracking.

---

## 🛠️ Setup & Configuration

### 1. Prerequisites
* .NET 8.0 SDK or later
* Microsoft Edge Browser
* Gemini API Keys (Google AI Studio)

### 2. Secrets Configuration
Create a file named `secrets.txt` in the project root directory. The application expects exactly **4 lines** in the following order:
1. Primary Gemini API Key
2. Secondary Gemini API Key
3. LinkedIn Email Address
4. LinkedIn Password

> **Note:** Ensure the file properties in Visual Studio are set to **"Copy to Output Directory: Copy always"**.

---

## 🚦 How it Works
1.  **Initialization:** The system greets the user via voice and checks for `secrets.txt`.
2.  **Validation:** If the config is missing, it enters a retry loop until the file is created.
3.  **Operation:** The bot logs into LinkedIn, scrapes jobs, and sends data to Gemini.
4.  **Quota Handling:** If an API limit is reached, it automatically switches keys and waits for 35 seconds.

---

## 🌍 Language & Currency
* **Analysis:** Optimized for remote jobs globally.
* **Currency:** Default compensation tracking is in **EUR** (€).
* **UI:** English console interface for professional standards.

---
## 🛠️ Troubleshooting & Common Issues

### 1. "Request headers must contain only ASCII characters"
* **Cause:** This happens when the job description or metadata contains special characters (like Cyrillic or emojis) that the API request cannot handle.
* **Solution:** The system now includes a `CleanToAscii` helper. Ensure all string data passed to headers is processed through this method.

### 2. "Quota Limit Reached (429)"
* **Cause:** You have exceeded the free tier requests for Gemini.
* **Solution:** The bot will automatically switch between your two API keys and wait for 35 seconds. If both are exhausted, it will shut down to protect your account.

### 3. "secrets.txt not found"
* **Cause:** The file is not in the execution folder.
* **Solution:** 1. Check if the file is in the project root.
    2. Ensure "Copy to Output Directory" is set to "Copy Always" in Visual Studio properties.
    3. Check `bin/Debug/net8.0/` to see if the file is actually there.

### 4. LinkedIn Login Fails
* **Cause:** Incorrect credentials or two-factor authentication (2FA).
* **Solution:** Verify lines 3 and 4 in `secrets.txt`. If 2FA is active, you may need to manually approve the login the first time.
  
## 📌 Current Status
**Version:** 1.0.0 (Stable Release)
**Last Update:** February 2026
  
## 🛡️ License
Private project for personal use and remote work career development.
*Developed with a focus on efficiency and career growth.*
