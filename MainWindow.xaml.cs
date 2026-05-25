using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace cyberCompanion   // Change if your namespace is different
{
    public partial class MainWindow : Window
    {
        private readonly ChatbotEngine _chatbot;
        private readonly MemoryStore _memory;
        private readonly ObservableCollection<ChatMessage> _messages;
        private readonly string _greetingWavPath;
        private MediaPlayer _mediaPlayer = new();

        public MainWindow()
        {
            InitializeComponent();

            _greetingWavPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            _memory = new MemoryStore();
            _chatbot = new ChatbotEngine(_memory);

            _messages = new ObservableCollection<ChatMessage>();
            ChatListBox.ItemsSource = _messages;

            EnsureGreetingWav();
            AddBotMessage("Hello! I'm your cybersecurity assistant. Ask me about passwords, scams, or privacy.");
            UpdateMemoryDisplay();
        }

        private void EnsureGreetingWav()
        {
            if (!System.IO.File.Exists(_greetingWavPath))
            {
                using (var synth = new SpeechSynthesizer())
                {
                    synth.SetOutputToWaveFile(_greetingWavPath);
                    synth.Speak("Welcome to the cybersecurity chatbot. I am here to help you stay safe online.");
                }
            }
        }

        private void PlayGreeting()
        {
            if (System.IO.File.Exists(_greetingWavPath))
            {
                _mediaPlayer?.Close();
                _mediaPlayer = new MediaPlayer();
                _mediaPlayer.Open(new Uri(_greetingWavPath));
                _mediaPlayer.Play();
            }
        }

        private void PlayGreetingButton_Click(object sender, RoutedEventArgs e) => PlayGreeting();

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessUserInput();

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ProcessUserInput();
        }

        private void ProcessUserInput()
        {
            string input = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                AddBotMessage("Please type a message. I can help with passwords, scams, privacy, or phishing.");
                return;
            }

            AddUserMessage(input);
            InputTextBox.Clear();

            // Check for exit/quit/bye commands
            string lowerInput = input.ToLower();
            if (lowerInput == "exit" || lowerInput == "quit" || lowerInput == "bye")
            {
                AddBotMessage("Thank you for using Cyber Companion. Goodbye! 👋");
                // Wait 1.5 seconds so user sees the goodbye message, then close
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    Application.Current.Shutdown();
                };
                timer.Start();
                return;
            }

            string response = _chatbot.ProcessInput(input, out _);
            AddBotMessage(response);

            UpdateMemoryDisplay();
        }

        private void AddUserMessage(string text)
        {
            _messages.Add(new ChatMessage
            {
                Text = text,
                Alignment = HorizontalAlignment.Right,
                Background = new SolidColorBrush(Color.FromRgb(52, 152, 219))
            });
            ScrollToBottom();
        }

        private void AddBotMessage(string text)
        {
            _messages.Add(new ChatMessage
            {
                Text = text,
                Alignment = HorizontalAlignment.Left,
                Background = new SolidColorBrush(Color.FromRgb(236, 240, 241))
            });
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                if (ChatListBox.Items.Count > 0)
                    ChatListBox.ScrollIntoView(ChatListBox.Items[^1]);
            }));
        }

        private void UpdateMemoryDisplay()
        {
            string name = _memory.Get("name") ?? "Not set";
            string favTopic = _memory.Get("favouriteTopic") ?? "None";
            MemoryDisplay.Text = $"Name: {name}\nFavourite topic: {favTopic}";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        private string _text = "";
        private HorizontalAlignment _alignment;
        private Brush _background = Brushes.Transparent;

        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public HorizontalAlignment Alignment { get => _alignment; set { _alignment = value; OnPropertyChanged(nameof(Alignment)); } }
        public Brush Background { get => _background; set { _background = value; OnPropertyChanged(nameof(Background)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
