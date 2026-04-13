using System;
using System.Linq;

namespace NimbleBlocks
{
    /// <summary>
    /// Input validation utilities for the NimbleBlocks game
    /// </summary>
    public static class InputValidators
    {
        #region Player Name Validation
        /// <summary>
        /// Validates player name input
        /// </summary>
        /// <param name="name">The name to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidatePlayerName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var trimmedName = name.Trim();
            
            // Check length constraints
            if (trimmedName.Length < GameConstants.Validation.MIN_NAME_LENGTH || 
                trimmedName.Length > GameConstants.Validation.MAX_NAME_LENGTH)
                return false;

            // Check for invalid characters (no numbers, special characters)
            if (trimmedName.Any(c => char.IsDigit(c) || char.IsSymbol(c) || char.IsPunctuation(c)))
                return false;

            // Check for empty or whitespace-only names
            if (trimmedName.All(c => char.IsWhiteSpace(c)))
                return false;

            return true;
        }

        /// <summary>
        /// Gets validation error message for player name
        /// </summary>
        /// <param name="name">The name that failed validation</param>
        /// <returns>Error message string</returns>
        public static string GetPlayerNameValidationError(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Player name cannot be empty.";

            var trimmedName = name.Trim();

            if (trimmedName.Length < GameConstants.Validation.MIN_NAME_LENGTH)
                return $"Player name must be at least {GameConstants.Validation.MIN_NAME_LENGTH} characters long.";

            if (trimmedName.Length > GameConstants.Validation.MAX_NAME_LENGTH)
                return $"Player name cannot exceed {GameConstants.Validation.MAX_NAME_LENGTH} characters.";

            if (trimmedName.Any(c => char.IsDigit(c)))
                return "Player name cannot contain numbers.";

            if (trimmedName.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
                return "Player name cannot contain special characters.";

            if (trimmedName.All(c => char.IsWhiteSpace(c)))
                return "Player name cannot be only spaces.";

            return "Invalid player name.";
        }
        #endregion

        #region Difficulty Validation
        /// <summary>
        /// Validates difficulty level
        /// </summary>
        /// <param name="difficulty">The difficulty to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateDifficulty(string difficulty)
        {
            if (string.IsNullOrWhiteSpace(difficulty))
                return false;

            var validDifficulties = new[]
            {
                GameConstants.GameModes.EASY,
                GameConstants.GameModes.INTERMEDIATE,
                GameConstants.GameModes.DIFFICULT
            };

            return validDifficulties.Contains(difficulty.Trim());
        }

        /// <summary>
        /// Gets validation error message for difficulty
        /// </summary>
        /// <param name="difficulty">The difficulty that failed validation</param>
        /// <returns>Error message string</returns>
        public static string GetDifficultyValidationError(string difficulty)
        {
            if (string.IsNullOrWhiteSpace(difficulty))
                return "Difficulty level cannot be empty.";

            return $"Invalid difficulty level. Must be one of: {string.Join(", ", GameConstants.GameModes.EASY, GameConstants.GameModes.INTERMEDIATE, GameConstants.GameModes.DIFFICULT)}";
        }
        #endregion

        #region Game Move Validation
        /// <summary>
        /// Validates a game move
        /// </summary>
        /// <param name="row">The row index (0-2)</param>
        /// <param name="count">The number of blocks to remove</param>
        /// <param name="currentCounts">Current block counts for each row</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateGameMove(int row, int count, int[] currentCounts)
        {
            // Validate row index
            if (row < 0 || row >= currentCounts.Length)
                return false;

            // Validate count
            if (count < GameConstants.MIN_BLOCKS_PER_MOVE || count > GameConstants.MAX_BLOCKS_PER_MOVE)
                return false;

            // Validate that row has enough blocks
            if (count > currentCounts[row])
                return false;

            // Validate that row has blocks to remove
            if (currentCounts[row] <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Validates multiple row selections (must be from same row)
        /// </summary>
        /// <param name="selectedCounts">Array of selected counts per row</param>
        /// <returns>True if valid (only one row has selections), false otherwise</returns>
        public static bool ValidateSingleRowSelection(int[] selectedCounts)
        {
            if (selectedCounts == null || selectedCounts.Length == 0)
                return false;

            int rowsWithSelections = selectedCounts.Count(count => count > 0);
            return rowsWithSelections == 1;
        }

        /// <summary>
        /// Gets validation error message for game move
        /// </summary>
        /// <param name="row">The row index</param>
        /// <param name="count">The number of blocks</param>
        /// <param name="currentCounts">Current block counts</param>
        /// <returns>Error message string</returns>
        public static string GetGameMoveValidationError(int row, int count, int[] currentCounts)
        {
            if (row < 0 || row >= currentCounts.Length)
                return "Invalid row selection.";

            if (count < GameConstants.MIN_BLOCKS_PER_MOVE)
                return $"Must remove at least {GameConstants.MIN_BLOCKS_PER_MOVE} block.";

            if (count > GameConstants.MAX_BLOCKS_PER_MOVE)
                return $"Cannot remove more than {GameConstants.MAX_BLOCKS_PER_MOVE} blocks at once.";

            if (count > currentCounts[row])
                return $"Not enough blocks in row {row + 1}. Only {currentCounts[row]} available.";

            if (currentCounts[row] <= 0)
                return $"No blocks available in row {row + 1}.";

            return "Invalid move.";
        }
        #endregion

        #region Time Validation
        /// <summary>
        /// Validates time limit
        /// </summary>
        /// <param name="timeLimit">Time limit in seconds</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateTimeLimit(int timeLimit)
        {
            return timeLimit > 0 && timeLimit <= 300; // Max 5 minutes
        }

        /// <summary>
        /// Gets validation error message for time limit
        /// </summary>
        /// <param name="timeLimit">The time limit that failed validation</param>
        /// <returns>Error message string</returns>
        public static string GetTimeLimitValidationError(int timeLimit)
        {
            if (timeLimit <= 0)
                return "Time limit must be greater than 0.";

            if (timeLimit > 300)
                return "Time limit cannot exceed 5 minutes (300 seconds).";

            return "Invalid time limit.";
        }
        #endregion

        #region Avatar Validation
        /// <summary>
        /// Validates avatar selection
        /// </summary>
        /// <param name="avatar">The avatar emoji/character</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateAvatar(string avatar)
        {
            if (string.IsNullOrWhiteSpace(avatar))
                return false;

            // Check if it's a valid emoji (basic check)
            var trimmedAvatar = avatar.Trim();
            return trimmedAvatar.Length > 0 && trimmedAvatar.Length <= 4; // Most emojis are 1-4 characters
        }

        /// <summary>
        /// Gets validation error message for avatar
        /// </summary>
        /// <param name="avatar">The avatar that failed validation</param>
        /// <returns>Error message string</returns>
        public static string GetAvatarValidationError(string avatar)
        {
            if (string.IsNullOrWhiteSpace(avatar))
                return "Avatar cannot be empty.";

            return "Invalid avatar selection.";
        }
        #endregion

        #region General Validation Helpers
        /// <summary>
        /// Validates that a value is within a range
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="min">Minimum value (inclusive)</param>
        /// <param name="max">Maximum value (inclusive)</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Validates that a string is not null or empty
        /// </summary>
        /// <param name="value">The string to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateNotNullOrEmpty(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Validates that a string is not null or whitespace
        /// </summary>
        /// <param name="value">The string to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateNotNullOrWhiteSpace(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        #endregion
    }
}
