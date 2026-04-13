using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NimbleBlocks
{
    public partial class CinematicIntroPage : ContentPage
    {
        public CinematicIntroPage()
        {
            InitializeComponent();
            StartCinematicSequence();
        }

        private async void StartCinematicSequence()
        {
            // Hide fallback background initially
            FallbackBackground.IsVisible = false;

            // Load the embedded resource image
            bool imageLoaded = false;
            
            // Try different resource paths
            string[] resourcePaths = {
                "NimbleBlocks.cinematicbackground.jpg",
                "cinematicbackground.jpg",
                "NimbleBlocks.Resources.cinematicbackground.jpg"
            };
            
            foreach (string resourcePath in resourcePaths)
            {
                try
                {
                    BackgroundImage.Source = ImageSource.FromResource(resourcePath);
                    System.Diagnostics.Debug.WriteLine($"Cinematic background image loaded successfully with path: {resourcePath}");
                    imageLoaded = true;
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load with path '{resourcePath}': {ex.Message}");
                }
            }
            
            if (imageLoaded)
            {
                // Show the background image immediately
                BackgroundImage.IsVisible = true;
                BackgroundImage.Opacity = 1.0;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("All resource paths failed, showing fallback background");
                // Show fallback background if image fails to load
                FallbackBackground.IsVisible = true;
                BackgroundImage.IsVisible = false;
            }





            // Wait a moment to see if image loads
            await Task.Delay(1000);

            // Check if image loaded successfully
            if (BackgroundImage.Source == null)
            {
                FallbackBackground.IsVisible = true;
            }

            // Wait briefly to display the image
            await Task.Delay(500);

            // Fade out the background image quickly
            await BackgroundImage.FadeTo(0.0, 500, Easing.CubicOut);
            await FallbackBackground.FadeTo(0.0, 300, Easing.CubicOut);

            // Navigate to main page immediately
            await Navigation.PushAsync(new MainPage());
            Navigation.RemovePage(this);
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent back button during intro
            return true;
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
}