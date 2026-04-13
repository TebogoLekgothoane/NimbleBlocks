using System;
using System.Collections.Generic;
using System.Linq;

namespace NimbleBlocks
{
    /// <summary>
    /// Handles core game logic for the NimbleBlocks game
    /// </summary>
    public class GameLogic
    {
        #region Properties
        public int[] BlockCounts { get; private set; }
        public bool IsUserTurn { get; set; }
        public bool GameActive { get; set; }
        public int TotalBlocks => BlockCounts.Sum();
        public bool IsGameOver => TotalBlocks == 0;
        #endregion

        #region Events
        public event EventHandler<GameStateChangedEventArgs> GameStateChanged;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        #endregion

        #region Constructor
        public GameLogic()
        {
            InitializeGame();
        }
        #endregion

        #region Game Initialization
        /// <summary>
        /// Initializes the game with starting block counts
        /// </summary>
        public void InitializeGame()
        {
            BlockCounts = new int[]
            {
                GameConstants.ROW_1_BLOCKS,
                GameConstants.ROW_2_BLOCKS,
                GameConstants.ROW_3_BLOCKS
            };
            IsUserTurn = true;
            GameActive = true;
            OnGameStateChanged();
        }

        /// <summary>
        /// Resets the game to initial state
        /// </summary>
        public void ResetGame()
        {
            InitializeGame();
        }
        #endregion

        #region Move Validation and Execution
        /// <summary>
        /// Validates if a move is legal
        /// </summary>
        /// <param name="row">Row index (0-2)</param>
        /// <param name="count">Number of blocks to remove</param>
        /// <returns>True if move is valid</returns>
        public bool IsValidMove(int row, int count)
        {
            return InputValidators.ValidateGameMove(row, count, BlockCounts);
        }

        /// <summary>
        /// Validates if multiple row selections are legal (must be from same row)
        /// </summary>
        /// <param name="selectedCounts">Array of selected counts per row</param>
        /// <returns>True if selection is valid</returns>
        public bool IsValidSingleRowSelection(int[] selectedCounts)
        {
            return InputValidators.ValidateSingleRowSelection(selectedCounts);
        }

        /// <summary>
        /// Executes a move by removing blocks from specified row
        /// </summary>
        /// <param name="row">Row index (0-2)</param>
        /// <param name="count">Number of blocks to remove</param>
        /// <returns>True if move was successful</returns>
        public bool ExecuteMove(int row, int count)
        {
            if (!IsValidMove(row, count))
                return false;

            BlockCounts[row] -= count;
            OnGameStateChanged();

            // Check if game is over
            if (IsGameOver)
            {
                GameActive = false;
                OnGameEnded();
            }

            return true;
        }

        /// <summary>
        /// Executes a move from multiple row selections (validates single row rule)
        /// </summary>
        /// <param name="selectedCounts">Array of selected counts per row</param>
        /// <returns>True if move was successful</returns>
        public bool ExecuteMultiRowMove(int[] selectedCounts)
        {
            if (!IsValidSingleRowSelection(selectedCounts))
                return false;

            // Find the row with selections
            int selectedRow = -1;
            for (int i = 0; i < selectedCounts.Length; i++)
            {
                if (selectedCounts[i] > 0)
                {
                    selectedRow = i;
                    break;
                }
            }

            if (selectedRow == -1)
                return false;

            return ExecuteMove(selectedRow, selectedCounts[selectedRow]);
        }
        #endregion

        #region Game State Management
        /// <summary>
        /// Gets available rows (rows with blocks remaining)
        /// </summary>
        /// <returns>List of available row indices</returns>
        public List<int> GetAvailableRows()
        {
            var availableRows = new List<int>();
            for (int i = 0; i < BlockCounts.Length; i++)
            {
                if (BlockCounts[i] > 0)
                    availableRows.Add(i);
            }
            return availableRows;
        }

        /// <summary>
        /// Gets the number of blocks in a specific row
        /// </summary>
        /// <param name="row">Row index (0-2)</param>
        /// <returns>Number of blocks in the row</returns>
        public int GetBlockCount(int row)
        {
            if (row < 0 || row >= BlockCounts.Length)
                return 0;
            return BlockCounts[row];
        }

        /// <summary>
        /// Checks if a specific row has blocks
        /// </summary>
        /// <param name="row">Row index (0-2)</param>
        /// <returns>True if row has blocks</returns>
        public bool HasBlocks(int row)
        {
            return GetBlockCount(row) > 0;
        }

        /// <summary>
        /// Gets a copy of current block counts
        /// </summary>
        /// <returns>Copy of block counts array</returns>
        public int[] GetBlockCountsCopy()
        {
            return (int[])BlockCounts.Clone();
        }
        #endregion

        #region AI Logic
        /// <summary>
        /// Generates a random computer move
        /// </summary>
        /// <returns>Tuple of (row, count) for the move</returns>
        public (int row, int count) GenerateRandomMove()
        {
            var availableRows = GetAvailableRows();
            if (availableRows.Count == 0)
                return (0, 0);

            var random = new Random();
            int row = availableRows[random.Next(availableRows.Count)];
            int maxBlocks = Math.Min(BlockCounts[row], GameConstants.MAX_BLOCKS_PER_MOVE);
            int count = random.Next(GameConstants.MIN_BLOCKS_PER_MOVE, maxBlocks + 1);

            return (row, count);
        }

        /// <summary>
        /// Generates a strategic computer move (for future enhancement)
        /// </summary>
        /// <returns>Tuple of (row, count) for the move</returns>
        public (int row, int count) GenerateStrategicMove()
        {
            // For now, just return random move
            // This can be enhanced with proper Nim game strategy later
            return GenerateRandomMove();
        }
        #endregion

        #region Game History (for undo functionality)
        /// <summary>
        /// Saves current game state for undo functionality
        /// </summary>
        /// <returns>Game state snapshot</returns>
        public GameStateSnapshot SaveState()
        {
            return new GameStateSnapshot
            {
                BlockCounts = GetBlockCountsCopy(),
                IsUserTurn = IsUserTurn,
                GameActive = GameActive
            };
        }

        /// <summary>
        /// Restores game state from snapshot
        /// </summary>
        /// <param name="snapshot">Game state snapshot to restore</param>
        public void RestoreState(GameStateSnapshot snapshot)
        {
            if (snapshot == null)
                return;

            BlockCounts = snapshot.BlockCounts;
            IsUserTurn = snapshot.IsUserTurn;
            GameActive = snapshot.GameActive;
            OnGameStateChanged();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Raises game state changed event
        /// </summary>
        protected virtual void OnGameStateChanged()
        {
            GameStateChanged?.Invoke(this, new GameStateChangedEventArgs(GetBlockCountsCopy(), IsUserTurn, GameActive));
        }

        /// <summary>
        /// Raises game ended event
        /// </summary>
        protected virtual void OnGameEnded()
        {
            GameEnded?.Invoke(this, new GameEndedEventArgs(IsUserTurn));
        }
        #endregion
    }

    #region Event Args Classes
    /// <summary>
    /// Event arguments for game state changes
    /// </summary>
    public class GameStateChangedEventArgs : EventArgs
    {
        public int[] BlockCounts { get; }
        public bool IsUserTurn { get; }
        public bool GameActive { get; }

        public GameStateChangedEventArgs(int[] blockCounts, bool isUserTurn, bool gameActive)
        {
            BlockCounts = blockCounts;
            IsUserTurn = isUserTurn;
            GameActive = gameActive;
        }
    }

    /// <summary>
    /// Event arguments for game end
    /// </summary>
    public class GameEndedEventArgs : EventArgs
    {
        public bool UserWon { get; }

        public GameEndedEventArgs(bool userWon)
        {
            UserWon = userWon;
        }
    }

    /// <summary>
    /// Game state snapshot for undo functionality
    /// </summary>
    public class GameStateSnapshot
    {
        public int[] BlockCounts { get; set; }
        public bool IsUserTurn { get; set; }
        public bool GameActive { get; set; }
    }
    #endregion
}
