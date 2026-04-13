using Xamarin.Forms;

namespace NimbleBlocks.Resources
{
    public partial class Logo : ContentView
    {
        public static readonly BindableProperty LogoSizeProperty =
            BindableProperty.Create(nameof(LogoSize), typeof(double), typeof(Logo), 50.0, propertyChanged: OnLogoSizeChanged);

        public static readonly BindableProperty ShowShadowProperty =
            BindableProperty.Create(nameof(ShowShadow), typeof(bool), typeof(Logo), true);

        public static readonly BindableProperty LogoOpacityProperty =
            BindableProperty.Create(nameof(LogoOpacity), typeof(double), typeof(Logo), 1.0, propertyChanged: OnLogoOpacityChanged);

        public double LogoSize
        {
            get => (double)GetValue(LogoSizeProperty);
            set => SetValue(LogoSizeProperty, value);
        }

        public bool ShowShadow
        {
            get => (bool)GetValue(ShowShadowProperty);
            set => SetValue(ShowShadowProperty, value);
        }

        public double LogoOpacity
        {
            get => (double)GetValue(LogoOpacityProperty);
            set => SetValue(LogoOpacityProperty, value);
        }

        public Logo()
        {
            InitializeComponent();
            LoadLogo();
        }

        private void LoadLogo()
        {
            // Try to load the logo image
            try
            {
                LogoImage.Source = ImageSource.FromResource("NimbleBlocks.Sounds.NimbleBlocksLogo1.jpg");
            }
            catch
            {
                // If that fails, show fallback
                FallbackLogo.IsVisible = true;
            }
            
            // Check if image loaded successfully
            LogoImage.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == Image.IsLoadingProperty.PropertyName)
                {
                    if (!LogoImage.IsLoading && LogoImage.Source != null)
                    {
                        // Image finished loading, hide fallback
                        FallbackLogo.IsVisible = false;
                    }
                    else if (!LogoImage.IsLoading && LogoImage.Source == null)
                    {
                        // Image failed to load, show fallback
                        FallbackLogo.IsVisible = true;
                    }
                }
            };
        }

        private static void OnLogoSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Logo logo)
            {
                logo.LogoImage.WidthRequest = (double)newValue;
                logo.LogoImage.HeightRequest = (double)newValue;
            }
        }
        private static void OnLogoOpacityChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Logo logo)
            {
                logo.LogoImage.Opacity = (double)newValue;
            }
        }
    }
}
