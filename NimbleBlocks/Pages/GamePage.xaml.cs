// GamePage.xaml.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.IO;
using System.Reflection;

namespace NimbleBlocks
{
    public class GameState
    {
        public int[] Counts { get; set; }
        public bool IsUserTurn { get; set; }
        public int CurrentPlayer { get; set; }
        public int TimeLeftSeconds { get; set; }
    }

    public class DeviceTimer : IDisposable
    {
        private readonly TimeSpan _interval;
        private readonly Func<bool> _callback;
        private readonly bool _repeat;
        private System.Threading.Timer _timer;
        private bool _disposed = false;

        public DeviceTimer(TimeSpan interval, Func<bool> callback, bool repeat = true)
        {
            _interval = interval;
            _callback = callback;
            _repeat = repeat;
            _timer = new System.Threading.Timer(TimerCallback, null, (int)_interval.TotalMilliseconds, repeat ? (int)_interval.TotalMilliseconds : System.Threading.Timeout.Infinite);
        }

        private void TimerCallback(object state)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!_disposed && _callback != null)
                {
                    bool shouldContinue = _callback();
                    if (!shouldContinue && _repeat)
                    {
                        _timer?.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                }
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _timer?.Dispose();
                _timer = null;
            }
        }
    }

    public class ShadowEffect : RoutingEffect
    {
        public ShadowEffect() : base("NimbleBlocks.ShadowEffect")
        {
        }

        public static readonly BindableProperty RadiusProperty =
            BindableProperty.CreateAttached("Radius", typeof(float), typeof(ShadowEffect), 5f);

        public static readonly BindableProperty DistanceXProperty =
            BindableProperty.CreateAttached("DistanceX", typeof(float), typeof(ShadowEffect), 1f);

        public static readonly BindableProperty DistanceYProperty =
            BindableProperty.CreateAttached("DistanceY", typeof(float), typeof(ShadowEffect), 2f);

        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached("Color", typeof(Color), typeof(ShadowEffect), Color.Black);

        public static float GetRadius(BindableObject view)
        {
            return (float)view.GetValue(RadiusProperty);
        }

        public static void SetRadius(BindableObject view, float value)
        {
            view.SetValue(RadiusProperty, value);
        }

        public static float GetDistanceX(BindableObject view)
        {
            return (float)view.GetValue(DistanceXProperty);
        }

        public static void SetDistanceX(BindableObject view, float value)
        {
            view.SetValue(DistanceXProperty, value);
        }

        public static float GetDistanceY(BindableObject view)
        {
            return (float)view.GetValue(DistanceYProperty);
        }

        public static void SetDistanceY(BindableObject view, float value)
        {
            view.SetValue(DistanceYProperty, value);
        }

        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

        public static void SetColor(BindableObject view, Color value)
        {
            view.SetValue(ColorProperty, value);
        }
    }

    public class GameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<object> Row1Blocks { get; } = new ObservableCollection<object>();
        public ObservableCollection<object> Row2Blocks { get; } = new ObservableCollection<object>();
        public ObservableCollection<object> Row3Blocks { get; } = new ObservableCollection<object>();

        private int[] _counts = new int[3];
        public int[] Counts => _counts;

        private string _difficulty;
        public string Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Difficulty)));
            }
        }

        private int _timeLeftSeconds;
        public int TimeLeftSeconds
        {
            get => _timeLeftSeconds;
            set
            {
                _timeLeftSeconds = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeLeftSeconds)));
            }
        }

        private bool _isUserTurn = true;
        public bool IsUserTurn
        {
            get => _isUserTurn;
            set
            {
                _isUserTurn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUserTurn)));
            }
        }

        private bool _gameActive = true;
        public bool GameActive
        {
            get => _gameActive;
            set
            {
                _gameActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameActive)));
            }
        }

        public GameViewModel(string difficulty)
        {
            Difficulty = difficulty;
            ResetBoard();
        }

        public void ResetBoard()
        {
            Row1Blocks.Clear();
            for (int i = 0; i < 3; i++) Row1Blocks.Add(new object());
            _counts[0] = 3;

            Row2Blocks.Clear();
            for (int i = 0; i < 4; i++) Row2Blocks.Add(new object());
            _counts[1] = 4;

            Row3Blocks.Clear();
            for (int i = 0; i < 5; i++) Row3Blocks.Add(new object());
            _counts[2] = 5;

            IsUserTurn = true;
            GameActive = true;
        }

        public bool RemoveBlocks(int row, int num)
        {
            if (num < 1 || num > _counts[row]) return false;

            _counts[row] -= num;
            return true;
        }

        public ObservableCollection<object> GetRowCollection(int row)
        {
            switch (row)
            {
                case 0: return Row1Blocks;
                case 1: return Row2Blocks;
                case 2: return Row3Blocks;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public int TotalBlocks => _counts[0] + _counts[1] + _counts[2];
    }

    public partial class GamePage : ContentPage, IDisposable
    {
        private GameViewModel _vm;
        private GameLogic _gameLogic;
        private string _difficulty;
        private int _timeLimit;
        private DeviceTimer _timer;
        private bool _isPlayerVsPlayer = false;
        private int _currentPlayer = 1; // 1 for Player 1, 2 for Player 2
        private int _lastMoverPlayer = 1; // Tracks who made the last move
        private bool _disposed = false;


        public GamePage(string difficulty, int timeLimit, CharacterData characterData = null)
        {
            InitializeComponent();
            _difficulty = difficulty;
            _timeLimit = timeLimit;

            // Initialize game logic
            _gameLogic = new GameLogic();
            _gameLogic.GameStateChanged += OnGameStateChanged;
            _gameLogic.GameEnded += OnGameEnded;

            _vm = new GameViewModel(difficulty);
            BindingContext = _vm;

            _vm.TimeLeftSeconds = _timeLimit;

            TimeLabel.Text = $"0:{_vm.TimeLeftSeconds:D2}";
            DifficultyLabel.Text = $"{_difficulty} ({_timeLimit}s)";
            TurnIndicator.Text = "●";
            TurnIndicator.TextColor = Color.FromHex("#00FF00");
            UpdateScoresLabel();

            // Initialize progress bar
            TimeProgressBar.Progress = 1.0;

            // Start star animation
            StartStarAnimation();
            
            // Setup audio settings
            SetupAudioSettings();

            // Set up character data
            if (characterData != null)
            {
                _isPlayerVsPlayer = characterData.IsPlayerVsPlayer;
                _currentPlayer = 1;

                // Reset scores when switching modes
                var app = Application.Current as App;
                app.PlayerWins = 0;
                app.AppWins = 0;

                // Update player labels with avatars and names
                Player1WinsLabel.Text = $"0";
                Player2WinsLabel.Text = $"0";
                Player1AvatarLabel.Text = characterData.Player1Avatar;
                Player1NameLabel.Text = characterData.Player1Name;
                Player2AvatarLabel.Text = characterData.Player2Avatar;
                Player2NameLabel.Text = characterData.Player2Name;

                if (_isPlayerVsPlayer)
                {
                    PromptLabel.Text = $"{characterData.Player1Name}'s turn! ";
                }
                else
                {
                    PromptLabel.Text = $"{characterData.Player1Name}'s turn!";
                }
            }
            else
            {
                // Fallback for backward compatibility
                _isPlayerVsPlayer = false; // Default to computer mode
                _currentPlayer = 1;

                // Set default avatars and names
                Player1AvatarLabel.Text = "🤴";
                Player1NameLabel.Text = "Player 1";
                Player2AvatarLabel.Text = _isPlayerVsPlayer ? "👸" : "🤖";
                Player2NameLabel.Text = _isPlayerVsPlayer ? "Player 2" : "AI";

                if (_isPlayerVsPlayer)
                {
                    PromptLabel.Text = "Player 1's turn!";
                }
                else
                {
                    PromptLabel.Text = "Your turn!";
                }
            }

            // Ensure GameLogic and ViewModel are synchronized
            _gameLogic.IsUserTurn = _vm.IsUserTurn;
            _gameLogic.GameActive = _vm.GameActive;

            UpdateBlockDisplay();
            StartTimer();
            StartSparkleAnimations();
        }

        private void UpdateBlockDisplay()
        {
            // Update visibility of blocks based on current counts from GameLogic
            var blockCounts = _gameLogic.GetBlockCountsCopy();
            
            // Synchronize ViewModel counts with GameLogic counts
            _vm.Counts[0] = blockCounts[0];
            _vm.Counts[1] = blockCounts[1];
            _vm.Counts[2] = blockCounts[2];
            
            UpdateRowVisibility(Row1, blockCounts[0], GameConstants.ROW_1_BLOCKS);
            UpdateRowVisibility(Row2, blockCounts[1], GameConstants.ROW_2_BLOCKS);
            UpdateRowVisibility(Row3, blockCounts[2], GameConstants.ROW_3_BLOCKS);

            // Clear any existing selections
            ClearAllSelections();
            UpdateSelectionControlsVisibility();
        }

        private void UpdateBlockDisplayWithoutResettingStyles()
        {
            // Update visibility of blocks based on current counts from GameLogic
            var blockCounts = _gameLogic.GetBlockCountsCopy();
            
            // Synchronize ViewModel counts with GameLogic counts
            _vm.Counts[0] = blockCounts[0];
            _vm.Counts[1] = blockCounts[1];
            _vm.Counts[2] = blockCounts[2];
            
            UpdateRowVisibilityWithoutResettingStyles(Row1, blockCounts[0], GameConstants.ROW_1_BLOCKS);
            UpdateRowVisibilityWithoutResettingStyles(Row2, blockCounts[1], GameConstants.ROW_2_BLOCKS);
            UpdateRowVisibilityWithoutResettingStyles(Row3, blockCounts[2], GameConstants.ROW_3_BLOCKS);

            // Clear any existing selections
            ClearAllSelections();
            UpdateSelectionControlsVisibility();
        }

        private void UpdateRowVisibility(StackLayout row, int count, int maxCount)
        {
            for (int i = 0; i < maxCount; i++)
            {
                if (i < row.Children.Count)
                {
                    var button = row.Children[i] as Button;
                    if (button != null)
                    {
                        button.IsVisible = i < count;
                        button.IsEnabled = i < count;

                        // Reset button appearance using UIHelpers
                        if (button.IsVisible)
                        {
                            UIHelpers.ApplyNormalStyle(button);
                            button.Rotation = 0;
                            button.Opacity = 1.0;
                        }
                    }
                }
            }
        }

        private void UpdateRowVisibilityWithoutResettingStyles(StackLayout row, int count, int maxCount)
        {
            for (int i = 0; i < maxCount; i++)
            {
                if (i < row.Children.Count)
                {
                    var button = row.Children[i] as Button;
                    if (button != null)
                    {
                        button.IsVisible = i < count;
                        button.IsEnabled = i < count;
                        // Don't reset button appearance - keep current styling
                    }
                }
            }
        }


        private StackLayout GetRowStack(int row)
        {
            switch (row)
            {
                case 0: return Row1;
                case 1: return Row2;
                case 2: return Row3;
                default: return null;
            }
        }

   

        private void SaveGameState()
        {
            var state = new GameState
            {
                Counts = new int[] { _vm.Counts[0], _vm.Counts[1], _vm.Counts[2] },
                IsUserTurn = _vm.IsUserTurn,
                CurrentPlayer = _currentPlayer,
                TimeLeftSeconds = _vm.TimeLeftSeconds
            };
        }

        private async Task AddSparkleEffect(Button button)
        {
            // Use UIHelpers for sparkle effect
            await UIHelpers.AnimateSparkleEffect(button);
        }

        private void StartSparkleAnimations()
        {
            // Background sparkle animations removed - now using simple black background
            // Stars are still animated in StartStarAnimation method
        }

        private void StartTimer()
        {
            _timer = new DeviceTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (!_vm.GameActive) return false;

                _vm.TimeLeftSeconds--;
                TimeLabel.Text = $"0:{_vm.TimeLeftSeconds:D2}";

                // Update progress bar
                double progress = (double)_vm.TimeLeftSeconds / _timeLimit;
                TimeProgressBar.Progress = Math.Max(0, progress);

                if (_vm.TimeLeftSeconds <= 0)
                {
                    EndGame(null);
                    return false;
                }
                return true;
            }, true);
        }


        private async Task HighlightTargetRow(int row)
        {
            var rowStack = GetRowStack(row);
            if (rowStack == null) return;

            // Scale up the entire row briefly
            await rowStack.ScaleTo(1.1, 300, Easing.SinInOut);
            await rowStack.ScaleTo(1.0, 300, Easing.SinInOut);
        }

        private async Task HighlightComputerMove(int row, int numBlocks)
        {
            var rowStack = GetRowStack(row);
            if (rowStack == null) return;

            // Find the last 'numBlocks' visible buttons in the row
            var visibleButtons = new List<Button>();
            foreach (var child in rowStack.Children)
            {
                if (child is Button button && button.IsVisible)
                {
                    visibleButtons.Add(button);
                }
            }

            // Highlight the last 'numBlocks' buttons
            int startIndex = Math.Max(0, visibleButtons.Count - numBlocks);
            var buttonsToHighlight = new List<Button>();
            for (int i = startIndex; i < visibleButtons.Count; i++)
            {
                buttonsToHighlight.Add(visibleButtons[i]);
                UIHelpers.ApplyComputerHighlightStyle(visibleButtons[i]);
            }

            // Wait for a moment to show the highlight, re-applying style periodically
            for (int i = 0; i < 15; i++) // 15 iterations of 100ms = 1.5 seconds
            {
                await Task.Delay(100);
                // Re-apply highlighting in case it was overwritten
                foreach (var button in buttonsToHighlight)
                {
                    if (button != null && button.IsVisible)
                    {
                        UIHelpers.ApplyComputerHighlightStyle(button);
                    }
                }
            }
        }

        private int CountSelectedBlocksInRow(StackLayout rowStack)
        {
            int count = 0;
            foreach (var child in rowStack.Children)
            {
                if (child is Button button && button.IsVisible)
                {
                    // Check if button is selected
                    if (button.BackgroundColor == GameConstants.Colors.SelectedBlock)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void ClearAllSelections()
        {
            // Clear selections from all rows
            ClearRowSelections(Row1);
            ClearRowSelections(Row2);
            ClearRowSelections(Row3);
            UpdateSelectionControlsVisibility();
        }

        private void ClearRowSelections(StackLayout row)
        {
            foreach (var child in row.Children)
            {
                if (child is Button button && button.IsVisible)
                {
                    UIHelpers.ApplyNormalStyle(button);
                }
            }
        }

        private (int row, int num) GetComputerMove()
        {
            var availableRows = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (_vm.Counts[i] > 0) availableRows.Add(i);
            }

            if (availableRows.Count == 0) return (0, 0);

            // "For the app's turn, the app picks a random number of blocks to remove from a row"
            var random = new Random();
            int row = availableRows[random.Next(availableRows.Count)];
            int num = random.Next(1, _vm.Counts[row] + 1);

            return (row, num);
        }

        private (int row, int num) GetStrategicMove()
        {
            // Enhanced Nim game strategy with multiple winning conditions
            var availableRows = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (_vm.Counts[i] > 0) availableRows.Add(i);
            }

            if (availableRows.Count == 0) return (0, 0);

            // Calculate current XOR
            int currentXor = _vm.Counts[0] ^ _vm.Counts[1] ^ _vm.Counts[2];

            // Check for immediate winning moves
            for (int i = 0; i < 3; i++)
            {
                if (_vm.Counts[i] > 0)
                {
                    // If we can take all blocks from a row and win
                    if (_vm.Counts[i] == _vm.TotalBlocks)
                    {
                        return (i, _vm.Counts[i]);
                    }
                }
            }

            if (currentXor == 0)
            {
                // We're in a losing position, try to minimize damage
                return GetMinimalDamageMove();
            }
            else
            {
                // We can win - find the move that makes XOR = 0
                for (int i = 0; i < 3; i++)
                {
                    if (_vm.Counts[i] > 0)
                    {
                        int targetCount = _vm.Counts[i] ^ currentXor;
                        if (targetCount < _vm.Counts[i])
                        {
                            return (i, _vm.Counts[i] - targetCount);
                        }
                    }
                }
            }

            // Fallback to smart random move
            return GetSmartRandomMove();
        }

        private (int row, int num) GetPsychologicalMove()
        {
            // Psychological moves to confuse the player
            var availableRows = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (_vm.Counts[i] > 0) availableRows.Add(i);
            }

            if (availableRows.Count == 0) return (0, 0);

            var random = new Random();

            // Strategy 1: Take from the row with most blocks (intimidating)
            if (random.Next(100) < 40)
            {
                int maxRow = 0;
                int maxCount = _vm.Counts[0];
                for (int i = 1; i < 3; i++)
                {
                    if (_vm.Counts[i] > maxCount)
                    {
                        maxCount = _vm.Counts[i];
                        maxRow = i;
                    }
                }
                int num = random.Next(1, Math.Min(maxCount, 3) + 1);
                return (maxRow, num);
            }
            // Strategy 2: Take from the row with least blocks (surprising)
            else if (random.Next(100) < 70)
            {
                int minRow = 0;
                int minCount = _vm.Counts[0];
                for (int i = 1; i < 3; i++)
                {
                    if (_vm.Counts[i] < minCount && _vm.Counts[i] > 0)
                    {
                        minCount = _vm.Counts[i];
                        minRow = i;
                    }
                }
                return (minRow, minCount); // Take all from smallest row
            }
            // Strategy 3: Take exactly 1 block (conservative)
            else
            {
                int row = availableRows[random.Next(availableRows.Count)];
                return (row, 1);
            }
        }

        private (int row, int num) GetMinimalDamageMove()
        {
            // When in losing position, minimize the advantage we give to opponent
            var availableRows = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (_vm.Counts[i] > 0) availableRows.Add(i);
            }

            var random = new Random();

            // Prefer taking from rows with fewer blocks
            int minRow = 0;
            int minCount = _vm.Counts[0];
            for (int i = 1; i < 3; i++)
            {
                if (_vm.Counts[i] < minCount && _vm.Counts[i] > 0)
                {
                    minCount = _vm.Counts[i];
                    minRow = i;
                }
            }

            // Take 1 block to minimize damage
            return (minRow, 1);
        }

        private (int row, int num) GetSmartRandomMove()
        {
            // Smart random move that considers game state
            var availableRows = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (_vm.Counts[i] > 0) availableRows.Add(i);
            }

            var random = new Random();
            int row = availableRows[random.Next(availableRows.Count)];

            // Take 1-3 blocks based on remaining total
            int maxBlocks = Math.Min(_vm.Counts[row], _vm.TotalBlocks > 6 ? 3 : 2);
            int num = random.Next(1, maxBlocks + 1);

            return (row, num);
        }

        private bool DetermineTimeoutWinner()
        {
            // Simple heuristic: if it's the current player's turn when time runs out,
            // they were in a better position (had the opportunity to make a move)
            // In a more sophisticated implementation, you could analyze the board state
            // to determine who has a better strategic position

            if (_isPlayerVsPlayer)
            {
                // In player vs player, the current player was about to make a move
                // so they were in a better position
                return true;
            }
            else
            {
                // In vs AI mode, if it's the user's turn when time runs out,
                // they were in a better position
                return _vm.IsUserTurn;
            }
        }

        private async void EndGame(bool? userWon)
        {
            _vm.GameActive = false;
            _gameLogic.GameActive = false;
            _timer?.Dispose();

            string title = "";
            string message = "";
            string emoji = "";

            // Handle timeout scenario
            if (userWon == null)
            {
                title = "⏰ Time's Up! ⏰";
                message = "No one wins this round!";
                emoji = "⏰";
            }
            else if (_isPlayerVsPlayer)
            {
                // In player vs player mode, the last mover wins
                var app = Application.Current as App;
                if (_lastMoverPlayer == 1)
                {
                    app.PlayerWins++;
                    var winnerName = Player1NameLabel.Text;
                    title = $"🎉 {winnerName} Wins! 🎉";
                    message = $"{winnerName} took the last block and won the game!";
                    }
                    else
                    {
                    app.AppWins++;
                    var winnerName = Player2NameLabel.Text;
                    title = $"🎉 {winnerName} Wins! 🎉";
                    message = $"{winnerName} took the last block and won the game!";
                }
                emoji = "🏆";
                try { Vibration.Vibrate(TimeSpan.FromMilliseconds(500)); } catch { }
                }
                else
                {
                // Versus AI mode uses userWon to determine winner
                var app = Application.Current as App;
                if (userWon == true)
                {
                    app.PlayerWins++;
                    var winnerName = Player1NameLabel.Text;
                    title = "🎉 You Win! 🎉";
                    message = $"Congratulations! {winnerName} took the last block and won the game!";
                emoji = "🏆";
                try { Vibration.Vibrate(TimeSpan.FromMilliseconds(500)); } catch { }
                    }
                    else
                    {
                    app.AppWins++;
                    var winnerName = Player2NameLabel.Text;
                    title = "🤖 Computer Wins! 🤖";
                    message = $"The computer ({winnerName}) took the last block. Better luck next time!";
                emoji = "💻";
            }
            }

            // Reflect updated scores immediately in header
            UpdateScoresLabel();

            // Show game over modal
            await ShowGameOverModal(title, message, emoji);
        }

        private void UpdateScoresLabel()
        {
            var app = Application.Current as App;
            Player1WinsLabel.Text = app.PlayerWins.ToString();
            Player2WinsLabel.Text = app.AppWins.ToString();
        }

        private async Task ShowGameOverModal(string title, string message, string emoji)
        {
            // Update the modal content with dynamic values
            var titleLabel = TimesUpOverlay.FindByName<Label>("TitleLabel");
            var trophyLabel = TimesUpOverlay.FindByName<Label>("TrophyLabel");
            var messageLabel = TimesUpOverlay.FindByName<Label>("MessageLabel");
            
            if (titleLabel != null) titleLabel.Text = title;
            if (trophyLabel != null) trophyLabel.Text = emoji;
            if (messageLabel != null) messageLabel.Text = message;
            
            // Update player statistics
            var statsPlayer1Avatar = TimesUpOverlay.FindByName<Label>("StatsPlayer1Avatar");
            var statsPlayer1Name = TimesUpOverlay.FindByName<Label>("StatsPlayer1Name");
            var statsPlayer1Wins = TimesUpOverlay.FindByName<Label>("StatsPlayer1Wins");
            var statsPlayer2Avatar = TimesUpOverlay.FindByName<Label>("StatsPlayer2Avatar");
            var statsPlayer2Name = TimesUpOverlay.FindByName<Label>("StatsPlayer2Name");
            var statsPlayer2Wins = TimesUpOverlay.FindByName<Label>("StatsPlayer2Wins");
            
            var app = Application.Current as App;
            
            // Update Player 1 stats
            if (statsPlayer1Avatar != null) statsPlayer1Avatar.Text = Player1AvatarLabel.Text;
            if (statsPlayer1Name != null) statsPlayer1Name.Text = Player1NameLabel.Text;
            if (statsPlayer1Wins != null) 
            {
                int player1Wins = app.PlayerWins;
                statsPlayer1Wins.Text = $"{player1Wins} {(player1Wins == 1 ? "win" : "wins")}";
            }
            
            // Update Player 2 stats
            if (statsPlayer2Avatar != null) statsPlayer2Avatar.Text = Player2AvatarLabel.Text;
            if (statsPlayer2Name != null) statsPlayer2Name.Text = Player2NameLabel.Text;
            if (statsPlayer2Wins != null) 
            {
                int player2Wins = app.AppWins;
                statsPlayer2Wins.Text = $"{player2Wins} {(player2Wins == 1 ? "win" : "wins")}";
            }
            
            // Show the background overlay
            TimesUpBackgroundOverlay.IsVisible = true;
            
            // Show the XAML modal
            TimesUpOverlay.IsVisible = true;
            TimesUpOverlay.Opacity = 0;
            
            // Animate modal appearance
            await TimesUpOverlay.FadeTo(1, 300, Easing.CubicInOut);
        }

        private async Task CloseModal(Grid modalOverlay)
        {
            await modalOverlay.FadeTo(0, 200, Easing.CubicInOut);
            // Remove modal from the page
            if (this.Content is Grid grid && grid.Children.Count > 1)
            {
                grid.Children.Remove(modalOverlay);
            }
        }

        private async Task RestartGame()
        {
            // Reset GameLogic state
            _gameLogic.ResetGame();
            
            // Reset ViewModel state
            _vm.ResetBoard();
            _vm.TimeLeftSeconds = _timeLimit;
            _vm.IsUserTurn = true;
            _vm.GameActive = true;
            
            // Update UI
            TimeLabel.Text = $"0:{_vm.TimeLeftSeconds:D2}";
            DifficultyLabel.Text = $"{_difficulty} ({_timeLimit}s)";
            TurnIndicator.Text = "●";
            TurnIndicator.TextColor = GameConstants.Colors.Success;

            // Reset progress bar
            TimeProgressBar.Progress = 1.0;

            // Reset current player to player 1
            _currentPlayer = 1;

            if (_isPlayerVsPlayer)
            {
                PromptLabel.Text = $"{Player1NameLabel.Text}'s turn!";
            }
            else
            {
                PromptLabel.Text = $"{Player1NameLabel.Text}'s turn!";
            }


            UpdateScoresLabel();
            UpdateBlockDisplay();
            StartTimer();
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
			// Reset scores before returning home (only in Player vs Player mode)
			if (_isPlayerVsPlayer)
			{
				var app = Application.Current as App;
				if (app != null)
				{
					app.PlayerWins = 0;
					app.AppWins = 0;
				}
			}
            await Navigation.PushAsync(new MainPage());
        }

        private void OnNewGameClicked(object sender, EventArgs e)
        {
            // Reset GameLogic state
            _gameLogic.ResetGame();
            
            // Reset ViewModel state
            _vm.ResetBoard();
            _vm.TimeLeftSeconds = _timeLimit;
            _vm.IsUserTurn = true;
            _vm.GameActive = true;
            
            // Update UI
            TimeLabel.Text = $"0:{_vm.TimeLeftSeconds:D2}";
            DifficultyLabel.Text = $"{_difficulty} ({_timeLimit}s)";
            TurnIndicator.Text = "●";
            TurnIndicator.TextColor = GameConstants.Colors.Success;

            // Reset progress bar
            TimeProgressBar.Progress = 1.0;

            // Reset player vs player mode
            _isPlayerVsPlayer = false;
            _currentPlayer = 1;

            if (_isPlayerVsPlayer)
            {
                PromptLabel.Text = "Player 1's turn! ";
                Player2NameLabel.Text = "PLAYER 2";
            }
            else
            {
                PromptLabel.Text = "Your turn! ";
                Player2NameLabel.Text = "AI";
            }

            UpdateBlockDisplay();
            if (_timer != null) _timer.Dispose();
            StartTimer();
        }

        private async void OnResetScoresClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Reset Scores", "Are you sure you want to reset the win counts for both players?", "Yes", "No");
            if (confirm)
            {
                var app = Application.Current as App;
                app.PlayerWins = 0;
                app.AppWins = 0;
                UpdateScoresLabel();

                // Show confirmation message
                await DisplayAlert("Scores Reset", "All scores have been reset to 0.", "OK");
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            // Prevent multiple clicks if modal is already open
            if (AudioSettingsModal.IsVisible)
                return;

            // Play sound effect using AudioManager
            AudioManager.Instance.PlaySoundEffect();

            // Button press animation
            await SettingsButton.ScaleTo(0.9, 100, Easing.SinInOut);
            await SettingsButton.ScaleTo(1.0, 100, Easing.SinInOut);

            // Show modal dialog with fade animation
            AudioSettingsModal.IsVisible = true;
            AudioSettingsModal.Opacity = 0.0;

            // Fade in animation
            await AudioSettingsModal.FadeTo(1.0, 300, Easing.CubicOut);
        }

        private async void OnCloseAudioSettingsClicked(object sender, EventArgs e)
        {
            // Play sound effect using AudioManager
            AudioManager.Instance.PlaySoundEffect();

            // Fade out animation
            await AudioSettingsModal.FadeTo(0.0, 300, Easing.CubicIn);

            AudioSettingsModal.IsVisible = false;
        }

        private void SetupAudioSettings()
        {
            // Setup sound slider - get initial value from AudioManager
            SoundSlider.Value = AudioManager.Instance.SoundVolume;
            SoundSlider.ValueChanged += OnSoundSliderChanged;
            UpdateSoundLabel();

            // Setup music slider - get initial value from AudioManager
            MusicSlider.Value = AudioManager.Instance.MusicVolume;
            MusicSlider.ValueChanged += OnMusicSliderChanged;
            UpdateMusicLabel();
        }

        private void OnSoundSliderChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                // Update AudioManager volume
                AudioManager.Instance.SoundVolume = e.NewValue;

                // Play preview sound if volume is not zero
                if (e.NewValue > 0)
                {
                    AudioManager.Instance.PlaySoundEffect();
                }

                System.Diagnostics.Debug.WriteLine($"Sound volume changed to: {e.NewValue:F2}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in sound slider change: {ex.Message}");
            }
            finally
            {
                UpdateSoundLabel();
            }
        }

        private void OnMusicSliderChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                // Update AudioManager music volume (handles play/stop automatically)
                AudioManager.Instance.MusicVolume = e.NewValue;

                System.Diagnostics.Debug.WriteLine($"Music volume changed to: {e.NewValue:F2}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in music slider change: {ex.Message}");
            }
            finally
            {
                UpdateMusicLabel();
            }
        }

        private void UpdateSoundLabel()
        {
            var percentage = (int)(SoundSlider.Value * 100);
            SoundValueLabel.Text = $"{percentage}%";
        }

        private void UpdateMusicLabel()
        {
            var percentage = (int)(MusicSlider.Value * 100);
            MusicValueLabel.Text = $"{percentage}%";
        }


        private async void OnBlockClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            // Check if it's the current player's turn
            if (_isPlayerVsPlayer)
            {
                if (!_vm.IsUserTurn) return; // Not the current player's turn
            }
            else
            {
                if (!_vm.IsUserTurn || !_vm.GameActive) return; // Not user's turn or game not active
            }

            // Find which row and position this button is in
            int row = -1;
            int position = -1;

            if (Row1.Children.Contains(button))
            {
                row = 0;
                position = Row1.Children.IndexOf(button);
            }
            else if (Row2.Children.Contains(button))
            {
                row = 1;
                position = Row2.Children.IndexOf(button);
            }
            else if (Row3.Children.Contains(button))
            {
                row = 2;
                position = Row3.Children.IndexOf(button);
            }

            if (row == -1 || position >= _vm.Counts[row]) return; // Invalid position

            // Toggle selection state
            ToggleBlockSelection(button);

            // Show/hide selection controls based on whether any blocks are selected
            UpdateSelectionControlsVisibility();
        }

        private void ToggleBlockSelection(Button button)
        {
            // Check if button is currently selected
            bool isSelected = button.BackgroundColor == GameConstants.Colors.SelectedBlock;

            if (isSelected)
            {
                // Deselect - return to normal appearance
                UIHelpers.ApplyNormalStyle(button);
            }
            else
            {
                // Select - change to selected appearance
                UIHelpers.ApplySelectedStyle(button);
            }

            // Update selection controls visibility
            UpdateSelectionControlsVisibility();
        }

        private void UpdateSelectionControlsVisibility()
        {
            // Check if any blocks are selected
            bool hasSelections = CountSelectedBlocksInRow(Row1) > 0 ||
                               CountSelectedBlocksInRow(Row2) > 0 ||
                               CountSelectedBlocksInRow(Row3) > 0;

            SelectionControlsFrame.IsVisible = hasSelections;

            if (hasSelections)
            {
                // Update selection info
                int totalSelected = CountSelectedBlocksInRow(Row1) +
                                  CountSelectedBlocksInRow(Row2) +
                                  CountSelectedBlocksInRow(Row3);
                SelectionInfoLabel.Text = $"{totalSelected} block(s) selected. Click 'Remove Selected' to confirm.";
            }
        }

        private async void OnRemoveSelectedClicked(object sender, EventArgs e)
        {
            if (!_vm.GameActive) return;

            // In player vs player mode, check if it's the current player's turn
            if (_isPlayerVsPlayer)
            {
                if (!_vm.IsUserTurn) return;
            }
            else
            {
                if (!_vm.IsUserTurn) return;
            }

            // Count selected blocks in each row
            int[] selectedCounts = new int[3];
            selectedCounts[0] = CountSelectedBlocksInRow(Row1);
            selectedCounts[1] = CountSelectedBlocksInRow(Row2);
            selectedCounts[2] = CountSelectedBlocksInRow(Row3);

            // Validate the move using GameLogic
            if (!_gameLogic.IsValidSingleRowSelection(selectedCounts) || selectedCounts.Sum() == 0)
            {
                await UIHelpers.ShowValidationError(this, "Invalid Move", 
                    "You must select blocks from exactly ONE row only. Please clear your selection and try again.");
                ClearAllSelections();
                UpdateSelectionControlsVisibility();
                return;
            }

            // Save current state for undo
            SaveGameState();

            // Add sparkle effect to selected blocks before removal (only for computer mode)
            if (!_isPlayerVsPlayer)
            {
            await AddSparkleEffectToSelectedBlocks();
            }

            // Play block remove sound
            AudioManager.Instance.PlayBlockRemoveSound();

            // Execute move using GameLogic
            // Track last mover (the player who is making this move now)
            _lastMoverPlayer = _currentPlayer;
            _gameLogic.IsUserTurn = true; // Set to player's turn
            if (!_gameLogic.ExecuteMultiRowMove(selectedCounts))
            {
                await UIHelpers.ShowValidationError(this, "Move Failed", "Unable to execute the move. Please try again.");
                return;
            }

            // Clear all selections before updating display
            ClearAllSelections();
            UpdateBlockDisplay();
            UpdateSelectionControlsVisibility();


            if (_isPlayerVsPlayer)
            {
                // Player vs Player mode - switch turns
                _vm.IsUserTurn = false;
                _currentPlayer = _currentPlayer == 1 ? 2 : 1;

                TurnIndicator.Text = "●";
                TurnIndicator.TextColor = Color.FromHex("#FF6B6B");
                string currentPlayerName = _currentPlayer == 1 ? Player1NameLabel.Text : Player2NameLabel.Text;
                PromptLabel.Text = $"{currentPlayerName}'s turn!";

                // Switch back to user turn for the next player
                _vm.IsUserTurn = true;
            }
            else
            {
                // AI mode - computer's turn
                _vm.IsUserTurn = false;
                _currentPlayer = 2; // Set current player to 2 (computer) for AI mode
                TurnIndicator.Text = "●";
                TurnIndicator.TextColor = Color.FromHex("#FF6B6B");

                // Computer makes move with highlighting
                _gameLogic.IsUserTurn = false; // Set to computer's turn
                var (computerRow, computerNum) = _gameLogic.GenerateRandomMove();
                
                // Show computer is thinking
                PromptLabel.Text = $"🤖 {Player2NameLabel.Text} is thinking...";
                
                // Highlight the blocks that will be removed
                await HighlightComputerMove(computerRow, computerNum);
                
                // Execute the move AFTER highlighting is complete
                // Track last mover as Player 2 (computer)
                _lastMoverPlayer = 2;
                _gameLogic.ExecuteMove(computerRow, computerNum);

                // Play block remove sound for computer move
                AudioManager.Instance.PlayBlockRemoveSound();

                // Update display after highlighting is complete (this will reset styles)
                UpdateBlockDisplay();

                // Brief delay to show the computer move message
                await Task.Delay(500);

                // Switch back to player's turn
                _vm.IsUserTurn = true;
                _currentPlayer = 1; // Set current player back to 1 (player) for AI mode
                _gameLogic.IsUserTurn = true; // Set GameLogic to player's turn
                PromptLabel.Text = $"{Player1NameLabel.Text}'s turn! ";
                TurnIndicator.Text = "●";
                TurnIndicator.TextColor = Color.FromHex("#45ca21");
            }
        }

        private async void OnClearSelectionClicked(object sender, EventArgs e)
        {
            ClearAllSelections();
            UpdateSelectionControlsVisibility();
        }

        private async Task AddSparkleEffectToSelectedBlocks()
        {
            var selectedButtons = new List<Button>();

            // Find all selected buttons using constants
            foreach (var child in Row1.Children)
            {
                if (child is Button button && button.IsVisible && button.BackgroundColor == GameConstants.Colors.SelectedBlock)
                    selectedButtons.Add(button);
            }
            foreach (var child in Row2.Children)
            {
                if (child is Button button && button.IsVisible && button.BackgroundColor == GameConstants.Colors.SelectedBlock)
                    selectedButtons.Add(button);
            }
            foreach (var child in Row3.Children)
            {
                if (child is Button button && button.IsVisible && button.BackgroundColor == GameConstants.Colors.SelectedBlock)
                    selectedButtons.Add(button);
            }

            // Add sparkle effect to all selected buttons
            foreach (var button in selectedButtons)
            {
                await AddSparkleEffect(button);
            }
        }

        private async void OnPlayAgainClicked(object sender, EventArgs e)
        {
            TimesUpOverlay.IsVisible = false;
            TimesUpBackgroundOverlay.IsVisible = false;
            await RestartGame();
        }

        private async void OnHomeDialogClicked(object sender, EventArgs e)
        {
            TimesUpOverlay.IsVisible = false;
            TimesUpBackgroundOverlay.IsVisible = false;
			// Reset scores before returning home (only in Player vs Player mode)
			if (_isPlayerVsPlayer)
			{
				var app = Application.Current as App;
				if (app != null)
				{
					app.PlayerWins = 0;
					app.AppWins = 0;
				}
			}
            await Navigation.PushAsync(new MainPage());
        }

        private void OnCloseGameOverModal(object sender, EventArgs e)
        {
            TimesUpOverlay.IsVisible = false;
            TimesUpBackgroundOverlay.IsVisible = false;
        }

        private async void StartStarAnimation()
        {
            try
            {
                // Create sparkling animation for all stars
                var stars = new[] { Star1, Star2, Star3, Star4, Star5, Star6, Star7, Star8 };

                foreach (var star in stars)
                {
                    if (star != null)
                    {
                        // Create a continuous sparkling animation
                        var animation = new Animation();

                        // Fade in and out animation
                        var fadeIn = new Animation(v => star.Opacity = v, 0.3, 1.0);
                        var fadeOut = new Animation(v => star.Opacity = v, 1.0, 0.3);

                        // Scale animation for twinkling effect
                        var scaleUp = new Animation(v => star.Scale = v, 0.8, 1.2);
                        var scaleDown = new Animation(v => star.Scale = v, 1.2, 0.8);

                        // Combine animations
                        animation.Add(0, 0.5, fadeIn);
                        animation.Add(0, 0.5, scaleUp);
                        animation.Add(0.5, 1.0, fadeOut);
                        animation.Add(0.5, 1.0, scaleDown);

                        // Start animation with random delay for each star
                        var randomDelay = new Random().Next(0, 2000);
                        await Task.Delay(randomDelay);

                        // Repeat animation infinitely
                        animation.Commit(star, GameConstants.AnimationKeys.STAR_SPARKLE, 16, GameConstants.STAR_ANIMATION_DURATION, Easing.Linear, (v, c) => {
                            if (c)
                            {
                                // Restart animation
                                Device.BeginInvokeOnMainThread(() => StartStarAnimation());
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in star animation: {ex.Message}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateScoresLabel();

            // Resume music if not playing
            AudioManager.Instance.PlayMusic();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Stop music
            AudioManager.Instance.StopMusic();
        }

        #region GameLogic Event Handlers
        private void OnGameStateChanged(object sender, GameStateChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Update UI based on game state changes
                UpdateBlockDisplay();
            });
        }

        private void OnGameEnded(object sender, GameEndedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (_isPlayerVsPlayer)
                {
                    // In player vs player mode, determine winner based on _currentPlayer
                    // _currentPlayer represents who just made the move that ended the game
                    bool player1Won = (_currentPlayer == 1);
                    EndGame(player1Won);
                }
                else
                {
                    // In AI mode, use the GameLogic's UserWon determination
                EndGame(e.UserWon);
                }
            });
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Dispose managed resources
                _timer?.Dispose();
                
                // Unsubscribe from GameLogic events
                if (_gameLogic != null)
                {
                    _gameLogic.GameStateChanged -= OnGameStateChanged;
                    _gameLogic.GameEnded -= OnGameEnded;
                    _gameLogic = null;
                }
                
                // Cancel all animations
                this.AbortAnimation(GameConstants.AnimationKeys.STAR_SPARKLE);
                this.AbortAnimation(GameConstants.AnimationKeys.SPARKLE_EFFECT);
                this.AbortAnimation(GameConstants.AnimationKeys.BUTTON_PRESS);
                
                _disposed = true;
            }
        }

        ~GamePage()
        {
            Dispose(false);
        }
        #endregion
    }
}