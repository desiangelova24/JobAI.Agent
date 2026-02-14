using System;
using System.IO;
using System.Speech.Synthesis;

namespace JobAI.Agent.UI
{
    public class VoiceAssistant
    {
        private readonly SpeechSynthesizer _synth;

        public VoiceAssistant()
        {
            _synth = new SpeechSynthesizer();

            // We configure the voice to be Female and English (US/UK) 
            // to help you practice your listening skills.
            _synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            _synth.Volume = 100;
            _synth.Rate = 0; // Normal speaking speed
        }

        /// <summary>
        /// Speaks a predefined phrase based on the bot's current activity.
        /// Displays both English and Bulgarian text in the console for learning.
        /// </summary>
        public void Say(string situation, int value = 0)
        {
            (string english, string bulgarian) = situation.ToLower() switch
            {
                "start" => ("System online. Starting the job search.", "Системата е онлайн. Започвам търсенето на работа."),
                "page" => ($"Moving to page {value}.", $"Премествам се на страница {value}."),
                "found" => ($"I found {value} new job opportunities.", $"Намерих {value} нови възможности за работа."),
                "remote" => ("This is a remote position paying in Euros.", "Това е дистанционна позиция с плащане в евро."),
                "error" => ("Attention! Please check for a captcha or security block.", "Внимание! Моля, провери за капча или блокада."),
                "finish" => ("Task completed. You are one step closer to your remote job!", "Задачата е изпълнена. Една стъпка по-близо си до дистанционната работа!"),
                _ => ("Processing data.", "Обработка на данни.")
            };

            // Print translations to help you learn English phrases while the bot works.
            Console.WriteLine($"\n📢 [EN] {english}");
            Console.WriteLine($"   [BG] {bulgarian}");

            // The bot speaks ONLY in English to immerse you in the language.
            _synth.Speak(english);
        }

        /// <summary>
        /// Speaks a custom message, such as AI-generated advice.
        /// </summary>
        public void SayMessage(string message)
        {
            Console.WriteLine($"🗣️ Assistant: {message}");
            _synth.Speak(message);
        }
    }
}
