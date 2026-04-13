using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NimbleBlocks
{
    public partial class AvatarSelectionPage : ContentPage
    {
        private string _difficulty;
        private int _timeLimit;
        private bool _isPlayerVsPlayer;

        // Selected avatars and names
        private string _player1Avatar = "🤴";
        private string _player1Name = "Player 1";
        private string _player2Avatar = "👸";
        private string _player2Name = "Player 2";
        private string _computerAvatar = "🤖";

        // Avatar button references
        private Button[] _player1Avatars;
        private Button[] _player2Avatars;
        private Button[] _computerAvatars;

        // Currently selected buttons
        private Button _selectedP1Avatar;
        private Button _selectedP2Avatar;
        private Button _selectedComputerAvatar;

        public AvatarSelectionPage(string difficulty, int timeLimit, bool isPlayerVsPlayer)
        {
            InitializeComponent();
            _difficulty = difficulty;
            _timeLimit = timeLimit;
            _isPlayerVsPlayer = isPlayerVsPlayer;

            // Delay the initialization to avoid potential timing issues
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    InitializeAvatarButtons();
                    SetDefaultSelections();
                    UpdatePlayer2Visibility();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in delayed initialization: {ex.Message}");
                }
            });
        }

        private void InitializeAvatarButtons()
        {
            // Initialize Player 1 avatar buttons
            _player1Avatars = new Button[]
            {
                P1Avatar1, P1Avatar2, P1Avatar3, P1Avatar4,
                P1Avatar5, P1Avatar6, P1Avatar7, P1Avatar8
            };

            // Initialize Player 2 avatar buttons
            _player2Avatars = new Button[]
            {
                P2Avatar1, P2Avatar2, P2Avatar3, P2Avatar4,
                P2Avatar5, P2Avatar6, P2Avatar7, P2Avatar8
            };

            // Initialize Computer avatar buttons
            _computerAvatars = new Button[]
            {
                ComputerAvatar1, ComputerAvatar2, ComputerAvatar3, ComputerAvatar4, ComputerAvatar5
            };
        }

        private void SetDefaultSelections()
        {
            try
            {
                // Set default selections
                _selectedP1Avatar = P1Avatar1;
                _selectedP2Avatar = P2Avatar2;
                _selectedComputerAvatar = ComputerAvatar1;

                // Apply selected styles only if buttons are available
                if (_player1Avatars != null && _player1Avatars.Length > 0)
                    UpdateAvatarSelection(_selectedP1Avatar, _player1Avatars);
                if (_player2Avatars != null && _player2Avatars.Length > 0)
                    UpdateAvatarSelection(_selectedP2Avatar, _player2Avatars);
                if (_computerAvatars != null && _computerAvatars.Length > 0)
                    UpdateAvatarSelection(_selectedComputerAvatar, _computerAvatars);
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Error in SetDefaultSelections: {ex.Message}");
                // Don't throw, just log the error
            }
        }

        private void UpdatePlayer2Visibility()
        {
            // Show/hide Player 2 section based on game mode
            if (Player2Frame != null)
            {
                Player2Frame.IsVisible = _isPlayerVsPlayer;
                System.Diagnostics.Debug.WriteLine($"Player2Frame visibility set to: {_isPlayerVsPlayer}");
            }

            // Show/hide Computer section based on game mode
            if (ComputerFrame != null)
            {
                ComputerFrame.IsVisible = !_isPlayerVsPlayer;
                System.Diagnostics.Debug.WriteLine($"ComputerFrame visibility set to: {!_isPlayerVsPlayer}");
            }

            // Update header text based on game mode
            UpdateHeaderText();
        }

        private void UpdateHeaderText()
        {
            // Find and update the header label
            var headerLabel = FindHeaderLabel();
            if (headerLabel != null)
            {
                if (_isPlayerVsPlayer)
                {
                    headerLabel.Text = "🎭 Choose Characters - Player vs Player";
                }
                else
                {
                    headerLabel.Text = "🎭 Choose Your Character - vs Computer";
                }
            }
        }

        private Label FindHeaderLabel()
        {
            // Find the header label by looking for the label with "Choose Characters" text
            foreach (var child in ((StackLayout)((ScrollView)Content).Content).Children)
            {
                if (child is Frame frame)
                {
                    var stackLayout = frame.Content as StackLayout;
                    if (stackLayout != null)
                    {
                        foreach (var frameChild in stackLayout.Children)
                        {
                            if (frameChild is Label label && label.Text.Contains("Choose Characters"))
                            {
                                return label;
                            }
                        }
                    }
                }
            }
            return null;
        }


        private void OnPlayer1AvatarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                AudioManager.Instance.PlaySoundEffect();
                _selectedP1Avatar = button;
                _player1Avatar = button.Text;
                UpdateAvatarSelection(button, _player1Avatars);
            }
        }

        private void OnPlayer2AvatarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                AudioManager.Instance.PlaySoundEffect();
                _selectedP2Avatar = button;
                _player2Avatar = button.Text;
                UpdateAvatarSelection(button, _player2Avatars);
            }
        }

        private void OnComputerAvatarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                AudioManager.Instance.PlaySoundEffect();
                _selectedComputerAvatar = button;
                _computerAvatar = button.Text;
                UpdateAvatarSelection(button, _computerAvatars);
            }
        }

        private void UpdateAvatarSelection(Button selectedButton, Button[] allButtons)
        {
            try
            {
                foreach (var button in allButtons)
                {
                    if (button == null) continue;

                    if (button == selectedButton)
                    {
                        // Apply selected style properties directly
                        button.BackgroundColor = Color.FromHex("#FF4500");
                        button.BorderColor = Color.OrangeRed;
                        button.TextColor = Color.White;
                        button.BorderWidth = 1;
                        button.CornerRadius = 50;
                        button.FontSize = 32;
                        button.WidthRequest = 80;
                        button.HeightRequest = 80;
                        button.Margin = new Thickness(10);
                    }
                    else
                    {
                        // Apply unselected style properties directly
                        button.BackgroundColor = Color.Transparent;
                        button.BorderColor = Color.Transparent;
                        button.BorderWidth = 1;
                        button.FontSize = 32;
                        button.WidthRequest = 80;
                        button.HeightRequest = 80;
                        button.Margin = new Thickness(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Error in UpdateAvatarSelection: {ex.Message}");
                throw;
            }
        }

        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            AudioManager.Instance.PlaySoundEffect();
            // Get names from entries
            _player1Name = Player1NameEntry.Text?.Trim() ?? "Player 1";
            _player2Name = Player2NameEntry.Text?.Trim() ?? "Player 2";

            // Validate names
            if (string.IsNullOrWhiteSpace(_player1Name))
            {
                await DisplayAlert("Invalid Name", "Please enter a name for Player 1.", "OK");
                return;
            }

            if (_isPlayerVsPlayer && string.IsNullOrWhiteSpace(_player2Name))
            {
                await DisplayAlert("Invalid Name", "Please enter a name for Player 2.", "OK");
                return;
            }

            // Create character data
            var characterData = new CharacterData
            {
                Player1Avatar = _player1Avatar,
                Player1Name = _player1Name,
                Player2Avatar = _isPlayerVsPlayer ? _player2Avatar : _computerAvatar,
                Player2Name = _isPlayerVsPlayer ? _player2Name : "Computer",
                IsPlayerVsPlayer = _isPlayerVsPlayer
            };

            // Navigate to game page with character data
            var gamePage = new GamePage(_difficulty, _timeLimit, characterData);
            await Navigation.PushAsync(gamePage);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            AudioManager.Instance.PlaySoundEffect();
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            AudioManager.Instance.PlayMusic();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            AudioManager.Instance.StopMusic();
        }

    }

    // Character data class to pass between pages
    public class CharacterData
    {
        public string Player1Avatar { get; set; }
        public string Player1Name { get; set; }
        public string Player2Avatar { get; set; }
        public string Player2Name { get; set; }
        public bool IsPlayerVsPlayer { get; set; }
    }
}