// App.xaml.cs
using Xamarin.Forms;

namespace NimbleBlocks
{
    public partial class App : Application
    {
        // Track wins globally (useful for displaying stats between sessions)
        public int PlayerWins { get; set; } = 0;
        public int AppWins { get; set; } = 0;

        public App()
        {
            InitializeComponent();

            // Preload AudioManager early to ensure seamless audio playback
            var _ = AudioManager.Instance;

            // Set up global navigation page with consistent theme
            MainPage = new NavigationPage(new SplashPage())
            {
                BarBackgroundColor = Color.FromHex("#000000"),
                BarTextColor = Color.White
            };
        }

        protected override void OnStart()
        {
            // Start background music once app launches
            if (!AudioManager.Instance.IsMusicPlaying)
                AudioManager.Instance.PlayMusic();
        }

        protected override void OnSleep()
        {
            // Pause music when app sleeps to save battery
            if (AudioManager.Instance.IsMusicPlaying)
                AudioManager.Instance.PauseMusic();
        }

        protected override void OnResume()
        {
            // Resume music when app returns to foreground
            if (!AudioManager.Instance.IsMusicPlaying)
                AudioManager.Instance.PlayMusic();
        }
    }
}
