using System;

namespace CyberCompanion
{
    /// <summary>
    /// Represents the detected emotional state of the user based on input.
    /// </summary>
    public enum Sentiment
    {
        Neutral,    // No strong emotion detected
        Worried,    // User expresses fear or anxiety
        Frustrated, // User is annoyed or confused
        Curious     // User is interested and wants to learn more
    }

    /// <summary>
    /// Static class to analyze user input and detect sentiment.
    /// Simple keyword-based detection for demonstration.
    /// </summary>
    public static class SentimentAnalyzer
    {
        /// <summary>Analyze the input string and return a Sentiment value.</summary>
        public static Sentiment Detect(string input)
        {
            string lower = input.ToLower();

            // Check for worried-related words
            if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("nervous"))
                return Sentiment.Worried;

            // Check for frustration indicators
            if (lower.Contains("frustrated") || lower.Contains("annoying") || lower.Contains("confusing"))
                return Sentiment.Frustrated;

            // Check for curiosity
            if (lower.Contains("curious") || lower.Contains("interesting") || lower.Contains("tell me more"))
                return Sentiment.Curious;

            // Default neutral
            return Sentiment.Neutral;
        }
    }
}