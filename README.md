# Cyber Companion – Cybersecurity Chatbot (WPF)

A friendly WPF chatbot that teaches cybersecurity topics like passwords, scams, privacy, and phishing. Built with .NET 6 and C#.

## Features

- **GUI Interface** – clean chat bubbles, ASCII art side panel, memory display, exit button.
- **Keyword Recognition** – detects `password`, `scam`, `privacy`, `phishing` and gives relevant tips.
- **Random Responses** – each topic has 4–5 different tips, chosen randomly for dynamic conversation.
- **Conversation Flow** – handles follow‑ups: `tell me more`, `another tip`, `explain more`.
- **Memory & Recall** – remembers your name (`my name is X`) and favourite topic (`I like Y`), personalises replies.
- **Sentiment Detection** – detects `worried`, `frustrated`, `curious` and adapts responses with empathy.
- **Voice Greeting** – auto‑generates `greeting.wav` on first run using text‑to‑speech; play with a button.
- **Error Handling** – default response for unknown inputs; empty input prompts user to type something.
- **Exit/Quit/Bye** – typing `exit`, `quit`, or `bye` makes the bot say thank you and close after 1.5 seconds.
- **Clean Code** – OOP with separate classes (`ChatbotEngine`, `MemoryStore`, `SentimentAnalyzer`), dictionaries for response pools, full comments.

## How to Run

1. Open the solution in **Visual Studio 2022** with the `.NET desktop development` workload installed.
2. Build the project (Ctrl+Shift+B) – NuGet will restore the `System.Speech` package automatically.
3. Press **F5** to run the application.
4. Type your messages and press Enter or click Send.

## Voice Greeting

- On the first run, the application generates `greeting.wav` in the output folder using `System.Speech.Synthesis`.
- Click the **Play Voice Greeting** button to hear the welcome message.

## Example Interactions

| You type | Bot response |
|----------|---------------|
| `my name is Alex` | "Nice to meet you, Alex! I'm your cybersecurity assistant. Use a strong password..." |
| `I like privacy` | "Got it! I'll remember that you like privacy.  Limit what you share on social media..." |
| `tell me about scams` | Random scam tip |
| `tell me more` | Another random scam tip (same topic) |
| `I'm worried about phishing` | "I understand you're worried.  Phishing emails often create urgency..." |
| `exit` | "Thank you for using Cyber Companion. Goodbye! " (app closes) |
| (empty input) | "Please type a message. I can help with passwords, scams, privacy, or phishing." |

## GitHub Requirements Met

-  Minimum **6 commits** with meaningful messages.
-  At least **2 releases/tags** (e.g., `v1.0`, `v1.1`).
-  Complete source code, README, and auto‑generated `greeting.wav` included.
-  Video presentation (unlisted YouTube) linked below.

## Video Explanation

https://youtu.be/QYv1cY377Uk?si=Z65uxAYDyAPK7zdp

The video demonstrates:
- Project structure and code organisation
- Explanation of each class (`ChatbotEngine`, `MemoryStore`, `SentimentAnalyzer`)
- Live demo of all features (keyword recognition, random responses, conversation flow, memory, sentiment, voice greeting, exit/quit/bye)

## Project Structure
