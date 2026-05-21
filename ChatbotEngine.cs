using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberCompanion
{
    /// <summary>
    /// Core logic of the chatbot: keyword recognition, random responses,
    /// conversation flow (follow-ups), memory integration, sentiment adaptation.
    /// </summary>
    public class ChatbotEngine
    {
        // Reference to the memory store (user details)
        private readonly MemoryStore _memory;

        // Stores the last cybersecurity topic discussed (e.g., "password") to handle follow-ups
        private string? _lastTopic;

        // Dictionary mapping topic keywords to lists of possible responses.
        // Allows random selection for dynamic conversation.
        private readonly Dictionary<string, List<string>> _responses;

        /// <summary>Constructor initializes the response database and connects to memory.</summary>
        public ChatbotEngine(MemoryStore memory)
        {
            _memory = memory;

            // Initialize response pools for each cybersecurity keyword.
            // Using case-insensitive key comparison for keyword matching.
            _responses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new()
                {
                    "🔐 Use a strong password with a mix of letters, numbers, and symbols.",
                    "🗝️ Never reuse passwords across different sites!",
                    "🛡️ Enable two‑factor authentication whenever possible.",
                    "🤖 Consider using a password manager to generate and store complex passwords."
                },
                ["scam"] = new()
                {
                    "📧 Don’t click on links in unsolicited emails – verify the sender first.",
                    "📞 Hang up on calls asking for personal information – legitimate companies don’t do that.",
                    "🛑 If an offer sounds too good to be true, it probably is a scam.",
                    "🔍 Always check the URL before entering any login details."
                },
                ["privacy"] = new()
                {
                    "👤 Limit what you share on social media – cybercriminals use that info.",
                    "🔒 Use privacy settings on all your accounts.",
                    "🌐 Consider using a VPN when on public Wi‑Fi.",
                    "📄 Read privacy policies – you’d be surprised what data is collected."
                },
                ["phishing"] = new()
                {
                    "🎣 Phishing emails often create urgency – 'Your account will be closed!'",
                    "🔗 Hover over links before clicking to see the real destination.",
                    "📎 Never open unexpected attachments, even if they look legitimate.",
                    "📞 If unsure, contact the company directly using a known number."
                }
            };
        }

        /// <summary>
        /// Main entry point for processing user input.
        /// Returns the chatbot's response and provides the detected sentiment.
        /// </summary>
        public string ProcessInput(string input, out Sentiment sentiment)
        {
            // First, detect the user's mood from the input
            sentiment = SentimentAnalyzer.Detect(input);
            string lower = input.Trim().ToLower();

            // --- 1. Handle follow-up requests (conversation flow) ---
            if (IsFollowUpRequest(lower) && !string.IsNullOrEmpty(_lastTopic))
            {
                // Continue discussing the same topic
                return GetPersonalisedResponse(_lastTopic, sentiment);
            }

            // --- 2. Remember user's name: "my name is X" or "call me X" ---
            if (TryExtractName(input, out string? name))
            {
                _memory.Set("name", name!);
                // Personalised greeting + a random tip
                return GetPersonalisedGreeting(name!) + " " + GetRandomTip();
            }

            // --- 3. Remember favourite topic: "I like password / scam / privacy" ---
            if (TryExtractFavouriteTopic(lower, out string? topic))
            {
                _memory.Set("favouriteTopic", topic!);
                return $"Got it! I'll remember that you like {topic}. " + GetRandomResponse(topic!);
            }

            // --- 4. Keyword recognition (core cybersecurity topics) ---
            foreach (var keyword in _responses.Keys)
            {
                if (lower.Contains(keyword))
                {
                    _lastTopic = keyword;   // store for potential follow-up
                    return GetPersonalisedResponse(keyword, sentiment);
                }
            }

            // --- 5. Default / fallback for unknown inputs (error handling) ---
            _lastTopic = null;
            return GetDefaultResponse(sentiment);
        }

        // ----- Helper methods for conversation flow -----

        /// <summary>Checks if the user wants to continue the current topic.</summary>
        private bool IsFollowUpRequest(string lower)
        {
            return lower.Contains("tell me more") ||
                   lower.Contains("another tip") ||
                   lower.Contains("explain more") ||
                   lower == "more" ||
                   lower == "continue";
        }

        // ----- Name extraction -----

        /// <summary>Extracts name from patterns like "my name is John" or "call me John".</summary>
        private bool TryExtractName(string input, out string? name)
        {
            name = null;
            string lower = input.ToLower();

            int idx = lower.IndexOf("my name is ");
            if (idx >= 0)
            {
                name = input.Substring(idx + 11).Trim();
                return true;
            }

            idx = lower.IndexOf("call me ");
            if (idx >= 0)
            {
                name = input.Substring(idx + 8).Trim();
                return true;
            }

            return false;
        }

        // ----- Favourite topic extraction -----

        /// <summary>Detects "I like X" where X is one of the known topics.</summary>
        private bool TryExtractFavouriteTopic(string lower, out string? topic)
        {
            topic = null;
            if (lower.Contains("i like "))
            {
                foreach (var key in _responses.Keys)
                {
                    if (lower.Contains(key))
                    {
                        topic = key;
                        return true;
                    }
                }
            }
            return false;
        }

        // ----- Response selection (randomised) -----

        /// <summary>Returns a random response from the pool for a given topic.</summary>
        private string GetRandomResponse(string topic)
        {
            if (_responses.TryGetValue(topic, out var list))
                return list[new Random().Next(list.Count)];
            return GetDefaultResponse(Sentiment.Neutral);
        }

        /// <summary>Returns a random tip from any topic (for the welcome or generic use).</summary>
        private string GetRandomTip()
        {
            var allTips = _responses.Values.SelectMany(v => v).ToList();
            return allTips[new Random().Next(allTips.Count)];
        }

        // ----- Personalisation and sentiment adaptation -----

        /// <summary>
        /// Builds a response that includes the user's name (if known) and adapts to sentiment.
        /// </summary>
        private string GetPersonalisedResponse(string topic, Sentiment sentiment)
        {
            string baseResponse = GetRandomResponse(topic);
            string userTopic = _memory.Get("favouriteTopic") ?? "";
            string name = _memory.Get("name") ?? "";

            string personalisation = "";
            if (!string.IsNullOrEmpty(name))
                personalisation = $"{name}, ";
            else if (!string.IsNullOrEmpty(userTopic) && userTopic.Equals(topic, StringComparison.OrdinalIgnoreCase))
                personalisation = $"Since you like {userTopic}, ";

            // Add sentiment prefix to show empathy
            return personalisation + ApplySentimentPrefix(sentiment) + baseResponse;
        }

        /// <summary>Personalised greeting when user first gives name.</summary>
        private string GetPersonalisedGreeting(string name)
        {
            return $"Nice to meet you, {name}! I'm your cybersecurity assistant.";
        }

        /// <summary>Returns an empathetic prefix based on detected sentiment.</summary>
        private string ApplySentimentPrefix(Sentiment sentiment)
        {
            return sentiment switch
            {
                Sentiment.Worried => "I understand you're worried. ",
                Sentiment.Frustrated => "I know this can be frustrating. Let me help you: ",
                Sentiment.Curious => "That's a great curiosity! ",
                _ => ""
            };
        }

        /// <summary>Fallback response when no keyword matches.</summary>
        private string GetDefaultResponse(Sentiment sentiment)
        {
            string prefix = ApplySentimentPrefix(sentiment);
            return prefix + "I can help with passwords, scams, privacy, or phishing. Just ask!";
        }
    }
}