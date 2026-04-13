using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NimbleBlocks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Level : ContentPage
    {
        public Level()
        {
            InitializeComponent();
            StartAnimations();
            StartStarAnimation();
        }

        private async void StartAnimations()
        {
            // Staggered button animations
            await Task.Delay(200);
            await EasyButton.ScaleTo(1.0, 300, Easing.BounceOut);

            await Task.Delay(100);
            await IntermediateButton.ScaleTo(1.0, 300, Easing.BounceOut);

            await Task.Delay(100);
            await DifficultButton.ScaleTo(1.0, 300, Easing.BounceOut);

            await Task.Delay(100);
            await BackButton.ScaleTo(1.0, 300, Easing.BounceOut);
        }

        private async void OnEasyClicked(object sender, EventArgs e)
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Button press animation with border color change
            await EasyButton.ScaleTo(0.9, 100, Easing.SinInOut);
            EasyButton.BorderColor = Color.FromHex("#00FF00");
            EasyButton.BorderWidth = 1;
            await EasyButton.ScaleTo(1.0, 100, Easing.SinInOut);
            await Task.Delay(150);
            await ShowGameModeModal("Easy", 30);
        }

        private async void OnIntermediateClicked(object sender, EventArgs e)
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Button press animation with border color change
            await IntermediateButton.ScaleTo(0.9, 100, Easing.SinInOut);
            IntermediateButton.BorderColor = Color.FromHex("#FF8C00");
            IntermediateButton.BorderWidth = 2;
            await IntermediateButton.ScaleTo(1.0, 100, Easing.SinInOut);
            await Task.Delay(150);
            await ShowGameModeModal("Intermediate", 20);
        }

        private async void OnDifficultClicked(object sender, EventArgs e)
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Button press animation with border color change
            await DifficultButton.ScaleTo(0.9, 100, Easing.SinInOut);
            DifficultButton.BorderColor = Color.FromHex("#FF0000");
            DifficultButton.BorderWidth = 3;
            await DifficultButton.ScaleTo(1.0, 100, Easing.SinInOut);
            await Task.Delay(150);
            await ShowGameModeModal("Difficult", 10);
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Button press animation
            await BackButton.ScaleTo(0.9, 100, Easing.SinInOut);
            await BackButton.ScaleTo(1.0, 100, Easing.SinInOut);
            await Task.Delay(100);
            await Navigation.PopAsync();
        }

        private string _currentDifficulty;
        private int _currentTimeLimit;

        private async Task ShowGameModeModal(string difficulty, int timeLimit)
        {
            // Store the difficulty and time limit for the modal buttons
            _currentDifficulty = difficulty;
            _currentTimeLimit = timeLimit;

            // Update the modal content
            GameModeDescriptionLabel.Text = $"{difficulty}\nTime Limit: {timeLimit}s";

            // Show the modal with fade animation
            GameModeModal.IsVisible = true;
            GameModeModal.Opacity = 0.0;

            // Fade in animation
            await GameModeModal.FadeTo(1.0, 300, Easing.CubicOut);
        }

        private async void OnCloseGameModeModal(object sender, EventArgs e)
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Fade out animation
            await GameModeModal.FadeTo(0.0, 300, Easing.CubicIn);

            GameModeModal.IsVisible = false;
        }

        private async Task CloseGameModeModalAsync()
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Fade out animation
            await GameModeModal.FadeTo(0.0, 300, Easing.CubicIn);

            GameModeModal.IsVisible = false;
        }

        private async void OnComputerVsPlayerClicked(object sender, EventArgs e)
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Close modal and navigate
            await CloseGameModeModalAsync();
            await Navigation.PushAsync(new AvatarSelectionPage(_currentDifficulty, _currentTimeLimit, false));
        }

        private async void OnPlayerVsPlayerClicked(object sender, EventArgs e)
        {
            // Play sound effect
            AudioManager.Instance.PlaySoundEffect();

            // Close modal and navigate
            await CloseGameModeModalAsync();
            await Navigation.PushAsync(new AvatarSelectionPage(_currentDifficulty, _currentTimeLimit, true));
        }

        private void StartStarAnimation()
        {
            var stars = new[] { Star1, Star2, Star3, Star4, Star5, Star6, Star7, Star8 };

            foreach (var star in stars)
            {
                if (star != null)
                {
                    AnimateStar(star);
                }
            }
        }

        private async void AnimateStar(Label star)
        {
            try
            {
                var randomDelay = new Random().Next(0, 2000);
                await Task.Delay(randomDelay);

                // Use optimized animation with proper cancellation
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

                // Commit with optimized frame rate and proper cancellation
                animation.Commit(star, "StarSparkle", 16, 2000, Easing.Linear, (v, c) =>
                {
                    if (!c && star != null && star.Parent != null)
                    {
                        AnimateStar(star);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in star animation: {ex.Message}");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Fade in animation when page appears
            this.Opacity = 0;
            await this.FadeTo(1.0, 400, Easing.CubicInOut);
        }
    }
}
