using System;

namespace MinesweeperModel
{
    /// <summary>
    /// Logic for the minesweeper board
    /// </summary>
    public class MinesweeperBoard
    {
        /// <summary>
        /// The width of the minesweeper board
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// The height of the minesweeper board
        /// </summary>
        public int Height { get; }
        /// <summary>
        /// The number of mines on the minesweeper board
        /// </summary>
        public int MinesNumber { get; }
        /// <summary>
        /// 2D array of the type Cell
        /// </summary>
        public Cell[,] Grid { get; }
        /// <summary>
        /// The total number of grid cells
        /// </summary>
        public int CellsNumber { get => Width * Height; }
        /// <summary>
        /// The number of cells that do not contain mine
        /// </summary>
        public int SafeCellsNumber { get => CellsNumber - MinesNumber; }
        /// <summary>
        /// The number of cells that do not contain mine
        /// </summary>
        public int MarkedCellsNumber { get => GetMarkedCellsCount(); }
        /// <summary>
        /// True, if all safe cells are open
        /// </summary>
        public bool IsBoardComplete { get => SafeCellsNumber == GetOpenedCellsCount(); }

        // constructor
        public MinesweeperBoard(Difficulty difficulty) : this(difficulty.Width, difficulty.Height, difficulty.MinesNumber) { }

        // constructor
        public MinesweeperBoard(int width, int height, int minesNumber)
        {
            // if any of the parameters is less than or equal to zero
            if (width <= 0 || height <= 0 || minesNumber <= 0)
            {
                // throw an exception that parameters must be greater than zero
                throw new ArgumentOutOfRangeException("The width, height and number of mines must be greater than zero!");
            }

            // initialize dimensions of the board
            Width = width;
            Height = height;
            MinesNumber = minesNumber;

            if (CellsNumber < MinesNumber)
            {
                throw new ArgumentOutOfRangeException("The mines number must not be greater than the total number of cells on the board!");
            }

            // create the 2D array of the type Cell
            Grid = new Cell[Height, Width];

            // fill the 2D array with cells, each with unique x and y coordinates
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Grid[i, j] = new Cell(i, j);
                }
            }

            // generate the board 
            GenerateBoard();
        }

        /// <summary>
        /// Generates the minesweeper board
        /// </summary>
        private void GenerateBoard()
        {
            // create an instance of the Random class
            Random random = new Random();

            // generate unique x and y coordinates and mark the corresponding cell as the cell with mine
            for (int i = 0; i < MinesNumber; i++)
            {
                // generate random x and y coordinates
                int x = random.Next(Height);
                int y = random.Next(Width);

                // regenarate coordinates if generated are not unique
                if (Grid[x, y].HasMine)
                {
                    i--;
                    continue;
                }

                // mark the cell as the cell with mine
                Grid[x, y].HasMine = true;
            }

            // set the number of surrounding mines for each cell that does not have a mine
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Grid[i, j].HasMine)
                    {
                        continue;
                    }

                    if (Grid[i, j].RowNumber - 1 >= 0)
                    {
                        if (Grid[Grid[i, j].RowNumber - 1, Grid[i, j].ColumnNumber].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                        if (Grid[i, j].ColumnNumber - 1 >= 0 && Grid[Grid[i, j].RowNumber - 1, Grid[i, j].ColumnNumber - 1].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                        if (Grid[i, j].ColumnNumber + 1 < Width && Grid[Grid[i, j].RowNumber - 1, Grid[i, j].ColumnNumber + 1].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                    }
                    if (Grid[i, j].RowNumber + 1 < Height)
                    {
                        if (Grid[Grid[i, j].RowNumber + 1, Grid[i, j].ColumnNumber].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                        if (Grid[i, j].ColumnNumber - 1 >= 0 && Grid[Grid[i, j].RowNumber + 1, Grid[i, j].ColumnNumber - 1].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                        if (Grid[i, j].ColumnNumber + 1 < Width && Grid[Grid[i, j].RowNumber + 1, Grid[i, j].ColumnNumber + 1].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                    }
                    if (Grid[i, j].ColumnNumber - 1 >= 0 && Grid[Grid[i, j].RowNumber, Grid[i, j].ColumnNumber - 1].HasMine)
                    {
                        Grid[i, j].NumberOfMinesAround++;
                    }
                    if (Grid[i, j].ColumnNumber + 1 < Width && Grid[Grid[i, j].RowNumber, Grid[i, j].ColumnNumber + 1].HasMine)
                    {
                        Grid[i, j].NumberOfMinesAround++;
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns an array with the cells that contain mines
        /// </summary>
        /// <returns>An array with cells that contain mines</returns>
        public Cell[] GetCellsWithMines()
        {
            // initialize an array with the length of the number of mines on the board
            Cell[] cellsHaveMines = new Cell[MinesNumber];

            // initialize an iterator for the cellsHaveMines array
            int m = 0;

            // go through all the cells
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    // add the grid cell to the array if the cell contains mine
                    if (Grid[i, j].HasMine)
                    {
                        cellsHaveMines[m++] = Grid[i, j];
                    }
                }
            }

            return cellsHaveMines;
        }

        /// <summary>
        /// Counts cells that have been marked
        /// </summary>
        /// <returns>The number of marked cells</returns>
        public int GetMarkedCellsCount()
        {
            // initialize the counter variable
            int count = 0;

            // go through all the cells
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    // increase the variable if the cell is marked
                    if (Grid[i, j].IsMarked)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Counts cells that have been opened
        /// </summary>
        /// <returns>The number of open cells</returns>
        public int GetOpenedCellsCount()
        {
            // initialize the counter variable
            int count = 0;

            // go through all the cells
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    // increase the variable if the cell is open
                    if (Grid[i, j].IsOpen)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Opens the specified cell and adjacent ones, if there are no mines nearby
        /// </summary>
        /// <param name="currentCell"></param>
        public void OpenCell(Cell currentCell)
        {
            // return if the cell is already open
            if (currentCell.IsOpen)
            {
                return;
            }

            // open all cells that contain a mine and return if the currentCell contains a mine
            if (currentCell.HasMine)
            {
                OpenCellsWithMines();
                return;
            }

            // mark the current cell as open
            currentCell.IsOpen = true;

            // clear the cell mark
            currentCell.IsMarked = false;

            // return if there are mines around the current cell
            if (currentCell.NumberOfMinesAround > 0)
            {
                return;
            }

            // open the neighboring cells
            if (currentCell.RowNumber - 1 >= 0)
            {
                OpenCell(Grid[currentCell.RowNumber - 1, currentCell.ColumnNumber]);
                if (currentCell.ColumnNumber - 1 >= 0)
                {
                    OpenCell(Grid[currentCell.RowNumber - 1, currentCell.ColumnNumber - 1]);
                }
                if (currentCell.ColumnNumber + 1 < Width)
                {
                    OpenCell(Grid[currentCell.RowNumber - 1, currentCell.ColumnNumber + 1]);
                }
            }
            if (currentCell.RowNumber + 1 < Height)
            {
                OpenCell(Grid[currentCell.RowNumber + 1, currentCell.ColumnNumber]);
                if (currentCell.ColumnNumber - 1 >= 0)
                {
                    OpenCell(Grid[currentCell.RowNumber + 1, currentCell.ColumnNumber - 1]);
                }
                if (currentCell.ColumnNumber + 1 < Width)
                {
                    OpenCell(Grid[currentCell.RowNumber + 1, currentCell.ColumnNumber + 1]);
                }
            }
            if (currentCell.ColumnNumber - 1 >= 0)
            {
                OpenCell(Grid[currentCell.RowNumber, currentCell.ColumnNumber - 1]);
            }
            if (currentCell.ColumnNumber + 1 < Width)
            {
                OpenCell(Grid[currentCell.RowNumber, currentCell.ColumnNumber + 1]);
            }
        }

        /// <summary>
        /// Opens all cells that contains a mine
        /// </summary>
        private void OpenCellsWithMines()
        {
            // go through all the cells
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    // check if there is a mine in the cell
                    if (Grid[i, j].HasMine)
                    {
                        // mark the cell as open
                        Grid[i, j].IsOpen = true;
                    }
                }
            }
        }
    }
}
