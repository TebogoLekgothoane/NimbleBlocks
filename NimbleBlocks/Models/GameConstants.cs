using Xamarin.Forms;

namespace NimbleBlocks
{
    public static class GameConstants
    {
        #region Game Rules
        public const int ROW_1_BLOCKS = 3;
        public const int ROW_2_BLOCKS = 4;
        public const int ROW_3_BLOCKS = 5;
        public const int TOTAL_BLOCKS = ROW_1_BLOCKS + ROW_2_BLOCKS + ROW_3_BLOCKS;
        public const int MIN_BLOCKS_PER_MOVE = 1;
        public const int MAX_BLOCKS_PER_MOVE = 5;
        #endregion

        #region Time Limits (in seconds)
        public const int EASY_TIME_LIMIT = 30;
        public const int INTERMEDIATE_TIME_LIMIT = 20;
        public const int DIFFICULT_TIME_LIMIT = 10;
        #endregion

        #region UI Dimensions
        public const int BLOCK_SIZE = 60;
        public const int BLOCK_CORNER_RADIUS = 8;
        public const int BUTTON_HEIGHT = 50;
        public const int BUTTON_CORNER_RADIUS = 25;
        public const int FRAME_CORNER_RADIUS = 20;
        public const int AVATAR_SIZE = 70;
        #endregion

        #region Animation Durations (in milliseconds)
        public const int BUTTON_PRESS_DURATION = 100;
        public const int FADE_DURATION = 300;
        public const int SCALE_DURATION = 500;
        public const int SPARKLE_DURATION = 800;
        public const int STAR_ANIMATION_DURATION = 2000;
        #endregion

        #region Colors
        public static class Colors
        {
            public static readonly Color NeonOrange = Color.FromHex("#FF4500");
            public static readonly Color NeonOrangeLight = Color.FromHex("#FF6B35");
            public static readonly Color NeonOrangeDark = Color.FromHex("#FF8C42");
            public static readonly Color NeonBlack = Color.FromHex("#000000");
            public static readonly Color NeonBlackLight = Color.FromHex("#1a0a0a");
        public static readonly Color NeonBlackDark = Color.FromHex("#0a0a0a");
        public static readonly Color SelectedBlock = Color.FromHex("#FFA500");
        public static readonly Color Success = Color.FromHex("#00FF00");
        public static readonly Color Warning = Color.FromHex("#FF8C00");
        public static readonly Color Error = Color.FromHex("#FF0000");
        public static readonly Color Transparent = Color.Transparent;
        public static readonly Color White = Color.White;
        public static readonly Color Black = Color.Black;
        }
        #endregion

        #region Font Sizes
        public const int TITLE_FONT_SIZE = 32;
        public const int SUBTITLE_FONT_SIZE = 18;
        public const int BUTTON_FONT_SIZE = 18;
        public const int BLOCK_FONT_SIZE = 20;
        public const int AVATAR_FONT_SIZE = 32;
        #endregion

        #region Margins and Padding
        public static class Spacing
        {
            public const int Small = 5;
            public const int Medium = 10;
            public const int Large = 20;
            public const int ExtraLarge = 30;
        }

        public static class Margins
        {
            public static readonly Thickness Small = new Thickness(5);
            public static readonly Thickness Medium = new Thickness(10);
            public static readonly Thickness Large = new Thickness(20);
            public static readonly Thickness ExtraLarge = new Thickness(30);
        }
        #endregion

        #region Game Modes
        public static class GameModes
        {
            public const string EASY = "Easy";
            public const string INTERMEDIATE = "Intermediate";
            public const string DIFFICULT = "Difficult";
        }

        public static class PlayerTypes
        {
            public const string PLAYER_1 = "Player 1";
            public const string PLAYER_2 = "Player 2";
            public const string COMPUTER = "Computer";
            public const string AI = "AI";
        }
        #endregion

        #region Audio
        public static class Audio
        {
            public const double DEFAULT_MUSIC_VOLUME = 0.6;
            public const double DEFAULT_SOUND_VOLUME = 0.8;
            public const double MIN_VOLUME = 0.0;
            public const double MAX_VOLUME = 1.0;
        }
        #endregion

        #region Validation
        public static class Validation
        {
            public const int MIN_NAME_LENGTH = 2;
            public const int MAX_NAME_LENGTH = 20;
            public const int MIN_PLAYER_NAME_LENGTH = 1;
        }
        #endregion

        #region Animation Keys
        public static class AnimationKeys
        {
            public const string STAR_SPARKLE = "StarSparkle";
            public const string SPARKLE_EFFECT = "SparkleEffect";
            public const string BUTTON_PRESS = "ButtonPress";
            public const string FADE_IN = "FadeIn";
            public const string FADE_OUT = "FadeOut";
        }
        #endregion
    }
}
