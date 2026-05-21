using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CyberCompanion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// Handles GUI events, voice greeting, chat message display, and memory UI update.
    /// </summary>
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

            // Set path for the voice greeting WAV file (saved in output directory)
            _greetingWavPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            // Initialise memory and chatbot engine
            _memory = new MemoryStore();
            _chatbot = new ChatbotEngine(_memory);

            // Collection for chat messages (supports dynamic UI updates)
            _messages = new ObservableCollection<ChatMessage>();
            ChatListBox.ItemsSource = _messages;

            // Generate the WAV file if it doesn't exist (first run)
            EnsureGreetingWav();

            // Welcome message from the bot
            AddBotMessage("Hello! I'm your cybersecurity assistant. Ask me about passwords, scams, or privacy.");

            // Update the side panel to show current memory contents
            UpdateMemoryDisplay();
        }

        /// <summary>Generate greeting.wav using System.Speech if not already present.</summary>
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

        /// <summary>Play the voice greeting using MediaPlayer (supports WAV).</summary>
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

        // Event handler for the Play Greeting button
        private void PlayGreetingButton_Click(object sender, RoutedEventArgs e) => PlayGreeting();

        // Send button click event
        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessUserInput();

        // Allow pressing Enter key to send message
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ProcessUserInput();
        }

        /// <summary>Read user input, get chatbot response, update UI.</summary>
        private void ProcessUserInput()
        {
            string input = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            // Show user's message in chat
            AddUserMessage(input);
            InputTextBox.Clear();

            // Get response from chatbot engine (sentiment is used internally)
            string response = _chatbot.ProcessInput(input, out _);
            AddBotMessage(response);

            // Refresh memory display in side panel (name, favourite topic)
            UpdateMemoryDisplay();
        }

        /// <summary>Add a user message to the chat list (right-aligned, blue bubble).</summary>
        private void AddUserMessage(string text)
        {
            _messages.Add(new ChatMessage
            {
                Text = text,
                Alignment = HorizontalAlignment.Right,
                Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)) // #3498DB
            });
            ScrollToBottom();
        }

        /// <summary>Add a bot message (left-aligned, light grey bubble).</summary>
        private void AddBotMessage(string text)
        {
            _messages.Add(new ChatMessage
            {
                Text = text,
                Alignment = HorizontalAlignment.Left,
                Background = new SolidColorBrush(Color.FromRgb(236, 240, 241)) // #ECF0F1
            });
            ScrollToBottom();
        }

        /// <summary>Auto-scroll to the most recent message.</summary>
        private void ScrollToBottom()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                if (ChatListBox.Items.Count > 0)
                    ChatListBox.ScrollIntoView(ChatListBox.Items[^1]);
            }));
        }

        /// <summary>Update the side panel with currently stored name and favourite topic.</summary>
        private void UpdateMemoryDisplay()
        {
            string name = _memory.Get("name") ?? "Not set";
            string favTopic = _memory.Get("favouriteTopic") ?? "None";
            MemoryDisplay.Text = $"Name: {name}\nFavourite topic: {favTopic}";
        }
    }

    /// <summary>
    /// Model for a single chat message. Implements INotifyPropertyChanged to update UI when properties change.
    /// </summary>
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