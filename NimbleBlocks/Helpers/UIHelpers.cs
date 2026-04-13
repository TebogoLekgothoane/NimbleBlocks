using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NimbleBlocks
{
    /// <summary>
    /// Helper class for common UI operations to reduce code duplication
    /// </summary>
    public static class UIHelpers
    {
        #region Button Styling
        /// <summary>
        /// Applies selected style to a button
        /// </summary>
        public static void ApplySelectedStyle(Button button)
        {
            if (button == null) return;

            button.BackgroundColor = GameConstants.Colors.SelectedBlock;
            button.TextColor = GameConstants.Colors.Black;
            button.Scale = 1.1;
            button.BorderWidth = 2;
            button.BorderColor = GameConstants.Colors.NeonOrange;
        }

        /// <summary>
        /// Applies normal style to a button
        /// </summary>
        public static void ApplyNormalStyle(Button button)
        {
            if (button == null) return;

            button.BackgroundColor = Color.FromHex("#FF4500");
            button.TextColor = GameConstants.Colors.White;
            button.Scale = 1.0;
            button.BorderWidth = 1;
            button.BorderColor = GameConstants.Colors.Transparent;
        }

        /// <summary>
        /// Applies disabled style to a button
        /// </summary>
        public static void ApplyDisabledStyle(Button button)
        {
            if (button == null) return;

            button.BackgroundColor = GameConstants.Colors.NeonBlackLight;
            button.TextColor = GameConstants.Colors.NeonOrangeDark;
            button.Scale = 1.0;
            button.Opacity = 0.5;
        }

        /// <summary>
        /// Applies computer highlight style to a button (same as player selection)
        /// </summary>
        public static void ApplyComputerHighlightStyle(Button button)
        {
            if (button == null) return;

            button.BackgroundColor = GameConstants.Colors.SelectedBlock;
            button.TextColor = GameConstants.Colors.Black;
            button.Scale = 1.1;
            button.BorderWidth = 2;
            button.BorderColor = GameConstants.Colors.NeonOrange;
        }

        /// <summary>
        /// Applies neon primary button style
        /// </summary>
        public static void ApplyPrimaryButtonStyle(Button button)
        {
            if (button == null) return;

            button.BackgroundColor = GameConstants.Colors.NeonOrange;
            button.TextColor = GameConstants.Colors.White;
            button.FontSize = GameConstants.BUTTON_FONT_SIZE;
            button.FontAttributes = FontAttributes.Bold;
            button.CornerRadius = GameConstants.BUTTON_CORNER_RADIUS;
            button.HeightRequest = GameConstants.BUTTON_HEIGHT;
            button.Margin = GameConstants.Margins.Large;
        }

        /// <summary>
        /// Applies neon secondary button style
        /// </summary>
        public static void ApplySecondaryButtonStyle(Button button)
        {
            if (button == null) return;

            button.BackgroundColor = GameConstants.Colors.Transparent;
            button.TextColor = GameConstants.Colors.NeonOrangeLight;
            button.FontSize = GameConstants.BUTTON_FONT_SIZE;
            button.CornerRadius = GameConstants.BUTTON_CORNER_RADIUS;
            button.HeightRequest = GameConstants.BUTTON_HEIGHT;
            button.Margin = GameConstants.Margins.Large;
            button.BorderWidth = 2;
            button.BorderColor = GameConstants.Colors.NeonOrangeLight;
        }
        #endregion

        #region Animations
        /// <summary>
        /// Performs button press animation
        /// </summary>
        public static async Task AnimateButtonPress(Button button)
        {
            if (button == null) return;

            await button.ScaleTo(0.95, GameConstants.BUTTON_PRESS_DURATION, Easing.SinInOut);
            await button.ScaleTo(1.0, GameConstants.BUTTON_PRESS_DURATION, Easing.SinInOut);
        }

        /// <summary>
        /// Performs fade in animation
        /// </summary>
        public static async Task AnimateFadeIn(VisualElement element, uint duration = GameConstants.FADE_DURATION)
        {
            if (element == null) return;

            element.Opacity = 0;
            await element.FadeTo(1.0, duration, Easing.CubicInOut);
        }

        /// <summary>
        /// Performs fade out animation
        /// </summary>
        public static async Task AnimateFadeOut(VisualElement element, uint duration = GameConstants.FADE_DURATION)
        {
            if (element == null) return;

            await element.FadeTo(0.0, duration, Easing.CubicInOut);
        }

        /// <summary>
        /// Performs scale animation
        /// </summary>
        public static async Task AnimateScale(VisualElement element, double scale, uint duration = GameConstants.SCALE_DURATION)
        {
            if (element == null) return;

            await element.ScaleTo(scale, duration, Easing.BounceOut);
        }

        /// <summary>
        /// Performs sparkle effect on a button
        /// </summary>
        public static async Task AnimateSparkleEffect(Button button)
        {
            if (button == null) return;

            // Store original colors
            var originalBackgroundColor = button.BackgroundColor;
            var originalTextColor = button.TextColor;

            // Change to bright green during sparkle
            button.BackgroundColor = GameConstants.Colors.Success;
            button.TextColor = GameConstants.Colors.Black;

            // Create sparkle animation
            var sparkleAnimation = new Animation();
            sparkleAnimation.Add(0, 0.3, new Animation(v => button.Scale = (float)v, 1.0, 1.4, Easing.BounceOut));
            sparkleAnimation.Add(0.3, 0.6, new Animation(v => button.Scale = (float)v, 1.4, 1.0, Easing.BounceIn));
            sparkleAnimation.Add(0, 0.6, new Animation(v => button.Rotation = (float)v, 0, 360, Easing.Linear));
            sparkleAnimation.Add(0, 0.2, new Animation(v => button.Opacity = (float)v, 1.0, 0.6, Easing.SinInOut));
            sparkleAnimation.Add(0.2, 0.4, new Animation(v => button.Opacity = (float)v, 0.6, 1.0, Easing.SinInOut));
            sparkleAnimation.Add(0.4, 0.6, new Animation(v => button.Opacity = (float)v, 1.0, 0.7, Easing.SinInOut));
            sparkleAnimation.Add(0.6, 0.8, new Animation(v => button.Opacity = (float)v, 0.7, 1.0, Easing.SinInOut));

            sparkleAnimation.Commit(button, GameConstants.AnimationKeys.SPARKLE_EFFECT, 16, GameConstants.SPARKLE_DURATION);

            // Reset to original colors
            button.BackgroundColor = originalBackgroundColor;
            button.TextColor = originalTextColor;
        }
        #endregion

        #region Layout Helpers
        /// <summary>
        /// Creates a neon frame with consistent styling
        /// </summary>
        public static Frame CreateNeonFrame()
        {
            return new Frame
            {
                BackgroundColor = GameConstants.Colors.Transparent,
                CornerRadius = GameConstants.FRAME_CORNER_RADIUS,
                HasShadow = true,
                Padding = GameConstants.Margins.Large,
                Margin = GameConstants.Margins.Medium
            };
        }

        /// <summary>
        /// Creates a neon label with consistent styling
        /// </summary>
        public static Label CreateNeonLabel(string text, int fontSize = GameConstants.BUTTON_FONT_SIZE, 
            Color? textColor = null, FontAttributes fontAttributes = FontAttributes.None)
        {
            return new Label
            {
                Text = text,
                TextColor = textColor ?? GameConstants.Colors.NeonOrange,
                FontSize = fontSize,
                FontAttributes = fontAttributes,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        /// <summary>
        /// Creates a neon button with consistent styling
        /// </summary>
        public static Button CreateNeonButton(string text, bool isPrimary = true)
        {
            var button = new Button
            {
                Text = text,
                FontSize = GameConstants.BUTTON_FONT_SIZE,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = GameConstants.BUTTON_CORNER_RADIUS,
                HeightRequest = GameConstants.BUTTON_HEIGHT,
                Margin = GameConstants.Margins.Large
            };

            if (isPrimary)
            {
                ApplyPrimaryButtonStyle(button);
            }
            else
            {
                ApplySecondaryButtonStyle(button);
            }

            return button;
        }
        #endregion

        #region Validation Helpers
        /// <summary>
        /// Shows a validation error alert
        /// </summary>
        public static async Task ShowValidationError(Page page, string title, string message)
        {
            if (page == null) return;

            await page.DisplayAlert(title, message, "OK");
        }

        /// <summary>
        /// Shows a success message
        /// </summary>
        public static async Task ShowSuccessMessage(Page page, string title, string message)
        {
            if (page == null) return;

            await page.DisplayAlert(title, message, "OK");
        }
        #endregion

        #region Color Helpers
        /// <summary>
        /// Gets difficulty color based on difficulty level
        /// </summary>
        public static Color GetDifficultyColor(string difficulty)
        {
            switch (difficulty)
            {
                case GameConstants.GameModes.EASY:
                    return GameConstants.Colors.Success;
                case GameConstants.GameModes.INTERMEDIATE:
                    return GameConstants.Colors.Warning;
                case GameConstants.GameModes.DIFFICULT:
                    return GameConstants.Colors.Error;
                default:
                    return GameConstants.Colors.NeonOrange;
            }
        }

        /// <summary>
        /// Gets time limit based on difficulty
        /// </summary>
        public static int GetTimeLimit(string difficulty)
        {
            switch (difficulty)
            {
                case GameConstants.GameModes.EASY:
                    return GameConstants.EASY_TIME_LIMIT;
                case GameConstants.GameModes.INTERMEDIATE:
                    return GameConstants.INTERMEDIATE_TIME_LIMIT;
                case GameConstants.GameModes.DIFFICULT:
                    return GameConstants.DIFFICULT_TIME_LIMIT;
                default:
                    return GameConstants.INTERMEDIATE_TIME_LIMIT;
            }
        }
        #endregion
    }
}
