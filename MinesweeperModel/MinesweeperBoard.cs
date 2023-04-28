using System;

namespace MinesweeperModel
{
    public class MinesweeperBoard
    {
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
        public int MinesNumber { get; }
        /// <summary>
        /// 2D array of the type Cell
        /// </summary>
        public Cell[,] Grid { get; set; }

        // constructor
        public MinesweeperBoard(int width, int height, int minesNumber)
        {
            // initialize dimensions of the board
            Width = width;
            Height = height;
            MinesNumber = minesNumber;

            // create the 2D array of the type Cell
            Grid = new Cell[Width, Height];

            // fill the 2D array with cells, each with unique x and y coordinates
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
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
            Random random = new Random();

            // generate unique x and y coordinates and mark the corresponding cell as the cell with mine
            for (int i = 0; i < MinesNumber; i++)
            {
                // generate random x and y coordinates
                int x = random.Next(Width);
                int y = random.Next(Height);

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
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
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
                        if (Grid[i, j].ColumnNumber + 1 < Height && Grid[Grid[i, j].RowNumber - 1, Grid[i, j].ColumnNumber + 1].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                    }
                    if (Grid[i, j].RowNumber + 1 < Width)
                    {
                        if (Grid[Grid[i, j].RowNumber + 1, Grid[i, j].ColumnNumber].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                        if (Grid[i, j].ColumnNumber - 1 >= 0 && Grid[Grid[i, j].RowNumber + 1, Grid[i, j].ColumnNumber - 1].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                        if (Grid[i, j].ColumnNumber + 1 < Height && Grid[Grid[i, j].RowNumber + 1, Grid[i, j].ColumnNumber + 1].HasMine)
                        {
                            Grid[i, j].NumberOfMinesAround++;
                        }
                    }
                    if (Grid[i, j].ColumnNumber - 1 >= 0 && Grid[Grid[i, j].RowNumber, Grid[i, j].ColumnNumber - 1].HasMine)
                    {
                        Grid[i, j].NumberOfMinesAround++;
                    }
                    if (Grid[i, j].ColumnNumber + 1 < Height && Grid[Grid[i, j].RowNumber, Grid[i, j].ColumnNumber + 1].HasMine)
                    {
                        Grid[i, j].NumberOfMinesAround++;
                    }
                }
            }
        }

        /// <summary>
        /// Opens the specified cell and adjacent ones, if there are no mines nearby
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>True if the cell was open, false if the selected cell contains a mine</returns>
        public void OpenCell(Cell currentCell)
        {
            if (currentCell.IsOpen)
            {
                return;
            }
            currentCell.IsOpen = true;
            if (currentCell.NumberOfMinesAround > 0 || currentCell.HasMine)
            {
                return;
            }

            if (currentCell.RowNumber - 1 >= 0)
            {
                OpenCell(Grid[currentCell.RowNumber - 1, currentCell.ColumnNumber]);
                if (currentCell.ColumnNumber - 1 >= 0)
                {
                    OpenCell(Grid[currentCell.RowNumber - 1, currentCell.ColumnNumber - 1]);
                }
                if (currentCell.ColumnNumber + 1 < Height)
                {
                    OpenCell(Grid[currentCell.RowNumber - 1, currentCell.ColumnNumber + 1]);
                }
            }
            if (currentCell.RowNumber + 1 < Width)
            {
                OpenCell(Grid[currentCell.RowNumber + 1, currentCell.ColumnNumber]);
                if (currentCell.ColumnNumber - 1 >= 0)
                {
                    OpenCell(Grid[currentCell.RowNumber + 1, currentCell.ColumnNumber - 1]);
                }
                if (currentCell.ColumnNumber + 1 < Height)
                {
                    OpenCell(Grid[currentCell.RowNumber + 1, currentCell.ColumnNumber + 1]);
                }
            }
            if (currentCell.ColumnNumber - 1 >= 0)
            {
                OpenCell(Grid[currentCell.RowNumber, currentCell.ColumnNumber - 1]);
            }
            if (currentCell.ColumnNumber + 1 < Height)
            {
                OpenCell(Grid[currentCell.RowNumber, currentCell.ColumnNumber + 1]);
            }
        }
    }
}
