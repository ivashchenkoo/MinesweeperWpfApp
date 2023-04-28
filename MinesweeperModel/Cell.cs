namespace MinesweeperModel
{
    public class Cell
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public bool HasMine { get; set; }
        public bool IsOpen { get; set; }
        public int NumberOfMinesAround { get; set; }

        public Cell(int x, int y)
        {
            RowNumber = x;
            ColumnNumber = y;
        }
    }
}
