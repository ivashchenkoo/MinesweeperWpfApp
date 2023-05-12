namespace MinesweeperModel
{
    /// <summary>
    /// Logic for the minesweeper board cell
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// The X coordinate of the cell on the board
        /// </summary>
        public int RowNumber { get; }
        /// <summary>
        /// The Y coordinate of the cell on the board
        /// </summary>
        public int ColumnNumber { get; }
        /// <summary>
        /// Indicates whether the cell has a mine
        /// </summary>
        public bool HasMine { get; set; }
        /// <summary>
        /// Indicates whether the cell is open
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// The number of adjacent cells that have a mine
        /// </summary>
        public int NumberOfMinesAround { get; set; }

        // constructor
        public Cell(int x, int y)
        {
            // set the RowNumber property as the x parameter
            RowNumber = x;
            // set the ColumnNumber property as the y parameter
            ColumnNumber = y;
        }
    }
}
