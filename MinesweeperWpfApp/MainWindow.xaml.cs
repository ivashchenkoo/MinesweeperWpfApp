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
		private readonly SolidColorBrush _openedCellBrush = new SolidColorBrush(Color.FromRgb(190, 190, 190));
		private readonly SolidColorBrush _flagCellBrush = new SolidColorBrush(Color.FromRgb(255, 212, 138));
		private readonly SolidColorBrush _explodedCellBrush = new SolidColorBrush(Color.FromRgb(255, 71, 71));
		private readonly SolidColorBrush _defaultCellBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221));
		const int buttonSize = 20;

	    private bool _gameOver;
		private MinesweeperBoard board;
		private Button[,] _buttons;

		public MainWindow()
		{
			InitializeComponent();
			RestartGame();
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			RestartGame();
		}

		private void RestartGame()
		{
			_gameOver = false;
			board = new MinesweeperBoard(12, 12, 20);
			GenerateButtonGrid();
		}

		private void GenerateButtonGrid()
		{
			MinesweeperBoardGrid.Children.Clear();

			_buttons = new Button[board.Width, board.Height];
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
					_buttons[i, j] = new Button
					{
						Width = buttonSize,
						Height = buttonSize,
						HorizontalAlignment = HorizontalAlignment.Left,
						VerticalAlignment = VerticalAlignment.Top
					};

					_buttons[i, j].PreviewMouseLeftButtonUp += CellLeftClickHandler;
					_buttons[i, j].PreviewMouseRightButtonUp += CellRightClickHandler;
					_buttons[i, j].Background = _defaultCellBrush;

					Grid.SetRow(_buttons[i, j], i);
					Grid.SetColumn(_buttons[i, j], j);
					grid.Children.Add(_buttons[i, j]);
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
					if ((string)_buttons[i, j].Tag == "Mine")
					{
						_buttons[i, j].Background = _flagCellBrush;
						_buttons[i, j].Content = "M";
					}

					if (board.Grid[i, j].IsOpen)
					{
						_buttons[i, j].Background = _openedCellBrush;
						if (board.Grid[i, j].HasMine)
						{
							_buttons[i, j].Content = "X";
						}
						else if (board.Grid[i, j].IsOpen && board.Grid[i, j].NumberOfMinesAround > 0)
						{
							_buttons[i, j].Content = board.Grid[i, j].NumberOfMinesAround;
						}
						else
						{
							_buttons[i, j].Content = "";
						}
					}
				}
			}
		}

		private void CellRightClickHandler(object sender, MouseButtonEventArgs e)
		{
			if (_gameOver)
			{
				return;
			}
			Button button = (Button)sender;
			if ((string)button.Tag != "Mine")
			{
				button.Background = _flagCellBrush;
				button.Content = "M";
				button.Tag = "Mine";
			}
			else
			{
				button.Background = _defaultCellBrush;
				button.Content = "";
				button.Tag = "";
			}
		}

		private void CellLeftClickHandler(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			if (_gameOver || (string)button.Tag == "Mine")
			{
				return;
			}
			int x = Grid.GetRow(button);
			int y = Grid.GetColumn(button);

			Cell cell = board.Grid[x, y];
			board.OpenCell(cell);

			if (board.IsBoardComplete)
			{
				var cellsHaveMines = board.GetCellsWithMines();
				for (int i = 0; i < cellsHaveMines.Length; i++)
				{
					_buttons[cellsHaveMines[i].RowNumber, cellsHaveMines[i].ColumnNumber].Tag = "Mine";
				}
				_gameOver = true;
			}

			UpdateButtonsGrid();

			if (cell.HasMine)
			{
				button.Background = _explodedCellBrush;
				MessageBox.Show("Game over!");
				_gameOver = true;
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
	}
}
