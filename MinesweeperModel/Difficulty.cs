namespace MinesweeperModel
{
    /// <summary>
    /// Logic for the minesweeper game difficulty
    /// </summary>
    public class Difficulty
    {
        /// <summary>
        /// The name of the difficulty
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The width of the minesweeper board
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The height of the minesweeper board
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The number of mines on the minesweeper board
        /// </summary>
        public int MinesNumber { get; set; }

        /// <summary>
        /// Creates an array with Difficulty class instances for default minesweeper game difficulties
        /// </summary>
        /// <returns>An array with default minesweeper game difficulties</returns>
        public static Difficulty[] GetDefaultDifficulties()
        {
            // create and fill an array with Difficulty class instances
            return new Difficulty[]
            {
                new Difficulty
                {
                    Description = "Beginner",
                    Width = 12,
                    Height = 12,
                    MinesNumber = 20
                },
                new Difficulty
                {
                    Description = "Intermediate",
                    Width = 16,
                    Height = 16,
                    MinesNumber = 40
                },
                new Difficulty
                {
                    Description = "Expert",
                    Width = 25,
                    Height = 16,
                    MinesNumber = 130
                }
            };
        }
    }
}
