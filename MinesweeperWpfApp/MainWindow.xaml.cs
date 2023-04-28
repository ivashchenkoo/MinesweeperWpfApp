using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MinesweeperModel;

namespace MinesweeperWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SolidColorBrush openedCellBrush = new SolidColorBrush(Color.FromRgb(130, 130, 130));
        readonly SolidColorBrush flagCellBrush = new SolidColorBrush(Color.FromRgb(255, 212, 138));
        readonly SolidColorBrush explodedCellBrush = new SolidColorBrush(Color.FromRgb(255, 138, 138));
        readonly SolidColorBrush defaultCellBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221));

        bool GameOver;
        MinesweeperBoard board;
        Button[,] buttons;

        public MainWindow()
        {
            InitializeComponent();
            board = new MinesweeperBoard(12, 12, 20);
            GenerateButtonGrid();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            GameOver = false;
            board = new MinesweeperBoard(12, 12, 20);
            GenerateButtonGrid();
        }

        private void GenerateButtonGrid()
        {
            MinesweeperBoardGrid.Children.Clear();

            buttons = new Button[board.Width, board.Height];
            Grid grid = new Grid();
            for (int x = 0; x < board.Width; x++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
            for (int y = 0; y < board.Height; y++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    buttons[i, j] = new Button
                    {
                        Width = 20,
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    buttons[i, j].PreviewMouseLeftButtonUp += CellLeftClickHandler;
                    buttons[i, j].PreviewMouseRightButtonUp += CellRightClickHandler;

                    Grid.SetColumn(buttons[i, j], i);
                    Grid.SetRow(buttons[i, j], j);
                    grid.Children.Add(buttons[i, j]);
                }
            }

            MinesweeperBoardGrid.Children.Add(grid);
        }

        private void UpdateButtonsGrid()
        {
            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    if ((string)buttons[i, j].Tag != "Mine")
                    {
                        if (board.Grid[i, j].IsOpen)
                        {
                            buttons[i, j].Background = openedCellBrush;
                            if (board.Grid[i, j].HasMine)
                            {
                                buttons[i, j].Content = "X";
                                buttons[i, j].Background = explodedCellBrush;
                            }
                            else if (board.Grid[i, j].IsOpen && board.Grid[i, j].NumberOfMinesAround > 0)
                            {
                                buttons[i, j].Content = board.Grid[i, j].NumberOfMinesAround;
                            }
                        }
                        else
                        {
                            buttons[i, j].Content = "";
                        }
                    }
                    else
                    {
                        buttons[i, j].Background = flagCellBrush;
                        buttons[i, j].Content = "X";
                    }
                }
            }
        }

        private void CellRightClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (GameOver)
            {
                return;
            }
            Button button = (Button)sender;
            if ((string)button.Tag != "Mine")
            {
                button.Background = flagCellBrush;
                button.Content = "X";
                button.Tag = "Mine";
            }
            else
            {
                button.Background = defaultCellBrush;
                button.Content = "";
                button.Tag = "";
            }
        }

        private void CellLeftClickHandler(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (GameOver || (string)button.Tag == "Mine")
            {
                return;
            }
            int x = Grid.GetColumn(button);
            int y = Grid.GetRow(button);

            Cell cell = board.Grid[x, y];
            board.OpenCell(cell);
            UpdateButtonsGrid();
            if (cell.HasMine)
            {
                MessageBox.Show("Game over!");
                GameOver = true;
            }
        }

        private void PrintBoard(MinesweeperBoard board)
        {
            Debug.WriteLine(CreateSeparator(board.Width + 1));
            for (int i = 0; i < board.Width; i++)
            {
                for (int j = 0; j < board.Height; j++)
                {
                    Debug.Write("|");
                    if (board.Grid[i, j].HasMine)
                    {
                        Debug.Write(" o ");
                    }
                    else if (board.Grid[i, j].IsOpen)
                    {
                        Debug.Write(" . ");
                    }
                    else if (board.Grid[i, j].NumberOfMinesAround > 0)
                    {
                        Debug.Write($" {board.Grid[i, j].NumberOfMinesAround} ");
                    }
                    else
                    {
                        Debug.Write("   ");
                    }
                }
                Debug.WriteLine("|");
                Debug.WriteLine(CreateSeparator(board.Width + 1));
            }
            Debug.WriteLine("");
        }

        private static string CreateSeparator(int verticalSeparatorsNumber)
        {
            return string.Join("---", new string('+', verticalSeparatorsNumber).ToCharArray());
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            int x = int.Parse(TextBoxX.Text);
            int y = int.Parse(TextBoxY.Text);
            Cell cell = board.Grid[x, y];
            if (cell.HasMine)
            {
                Debug.WriteLine("Game over!");
                return;
            }
            board.OpenCell(board.Grid[x, y]);
            PrintBoard(board);
        }
    }
}
