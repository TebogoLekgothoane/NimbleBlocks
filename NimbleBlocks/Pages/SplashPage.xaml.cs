using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NimbleBlocks
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
            StartAnimations();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(4000);
            await Navigation.PushAsync(new CinematicIntroPage());
        }

        private async void StartAnimations()
        {
            

            // Animate stars
            _ = AnimateStar(Star1, 2000);
            _ = AnimateStar(Star2, 2500);
            _ = AnimateStar(Star3, 1800);
            _ = AnimateStar(Star4, 2200);
            _ = AnimateStar(Star5, 1900);
            _ = AnimateStar(Star6, 2100);
            _ = AnimateStar(Star7, 1700);
            _ = AnimateStar(Star8, 2300);

            // Animate loading text
            _ = AnimateLoadingText();
            
            // Animate loading dots
            _ = AnimateLoadingDots();
        }

        private async Task AnimateStar(Label star, int duration)
        {
            while (true)
            {
                await star.FadeTo(0.3, (uint)(duration / 2));
                await star.FadeTo(0.8, (uint)(duration / 2));
            }
        }

        private async Task AnimateLoadingText()
        {
            string[] loadingTexts = { "Loading...", "Preparing blocks...", "Setting up game...", "Almost ready..." };
            int index = 0;

            while (true)
            {
                LoadingLabel.Text = loadingTexts[index];
                await LoadingLabel.FadeTo(0.5, 500u);
                await LoadingLabel.FadeTo(1.0, 500u);
                await Task.Delay(1000);
                index = (index + 1) % loadingTexts.Length;
            }
        }

        private async Task AnimateLoadingDots()
        {
            while (true)
            {
                // Dot 1
                await Dot1.FadeTo(1.0, 200u);
                await Task.Delay(200);
                await Dot1.FadeTo(0.3, 200u);
                
                // Dot 2
                await Dot2.FadeTo(1.0, 200u);
                await Task.Delay(200);
                await Dot2.FadeTo(0.3, 200u);
                
                // Dot 3
                await Dot3.FadeTo(1.0, 200u);
                await Task.Delay(200);
                await Dot3.FadeTo(0.3, 200u);
                
                await Task.Delay(300);
            }
        }
    }
}
