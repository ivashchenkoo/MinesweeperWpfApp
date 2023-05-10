using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

			var standardDifficulties = Difficulty.GetDefaultDifficulties();
			Difficulty[] difficulties = new Difficulty[standardDifficulties.Length + 1];
			for (int i = 0; i < standardDifficulties.Length; i++)
			{
				difficulties[i] = standardDifficulties[i];
			}
			difficulties[standardDifficulties.Length] = new Difficulty
			{
				Description = "Custom",
				Height = 12,
				Width = 12,
				MinesNumber = 20
			};
			DifficultyComboBox.ItemsSource = difficulties;
			DifficultyComboBox.SelectedIndex = 0;

			TextBoxesGrid.Visibility = Visibility.Collapsed;
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			Difficulty difficulty = (Difficulty)DifficultyComboBox.SelectedItem;

			if (difficulty.Description == "Custom")
			{
				if (WidthTextBox.Text != string.Empty && HeightTextBox.Text != string.Empty && MinesTextBox.Text != string.Empty)
				{
					difficulty.Width = int.Parse(WidthTextBox.Text);
					difficulty.Height = int.Parse(HeightTextBox.Text);
					difficulty.MinesNumber = int.Parse(MinesTextBox.Text);
				}
				else
				{
					return;
				}
			}

			RestartGame(difficulty);
		}

		private void RestartGame(int width, int height, int mines)
		{
			_gameOver = false;
			board = new MinesweeperBoard(width, height, mines);
			GenerateButtonGrid();
		}

		private void RestartGame(Difficulty difficulty) => RestartGame(difficulty.Width, difficulty.Height, difficulty.MinesNumber);

		private void GenerateButtonGrid()
		{
			MinesweeperBoardGrid.Children.Clear();

			_buttons = new Button[board.Height, board.Width];
			Grid grid = new Grid();
			for (int x = 0; x < board.Height; x++)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			}
			for (int y = 0; y < board.Width; y++)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			}
			for (int i = 0; i < board.Height; i++)
			{
				for (int j = 0; j < board.Width; j++)
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
			int tagCount = 0;
			for (int i = 0; i < board.Height; i++)
			{
				for (int j = 0; j < board.Width; j++)
				{
					if ((string)_buttons[i, j].Tag == "Mine")
					{
						tagCount++;
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

			MinesCountTextBlock.Text = (board.MinesNumber - tagCount).ToString();
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
				button.Tag = "Mine";
			}
			else
            {
                button.Background = _defaultCellBrush;
                button.Content = "";
                button.Tag = "";
			}
			UpdateButtonsGrid();
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
                MinesCountTextBlock.Text = "0";
                MessageBox.Show("You win!");
            }

			UpdateButtonsGrid();

			if (cell.HasMine)
			{
				button.Background = _explodedCellBrush;
				MessageBox.Show("Game over!", "Minesweeper");
				_gameOver = true;
			}
		}

		private void PrintBoard(MinesweeperBoard board)
		{
			Debug.WriteLine(CreateSeparator(board.Width + 1));
			for (int i = 0; i < board.Height; i++)
			{
				for (int j = 0; j < board.Width; j++)
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

		private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Difficulty difficulty = (Difficulty)DifficultyComboBox.SelectedItem;

			if (difficulty.Description == "Custom")
			{
				TextBoxesGrid.Visibility = Visibility.Visible;
				return;
			}
			else
			{
				TextBoxesGrid.Visibility = Visibility.Collapsed;
			}

			RestartGame(difficulty);
		}

		private void Dimension_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			string text = Regex.Match(textBox.Text, "[0-9]+").Value;
			if (int.TryParse(text, out int digits))
			{
				if (digits > 100)
				{
					textBox.Text = "100";
					textBox.SelectionStart = textBox.Text.Length;
				}
				if (digits <= 0)
				{
					textBox.Text = "1";
					textBox.SelectionStart = textBox.Text.Length;
				}

				if (WidthTextBox.Text != string.Empty && HeightTextBox.Text != string.Empty && MinesTextBox.Text != string.Empty)
				{
					int width = int.Parse(WidthTextBox.Text);
					int height = int.Parse(HeightTextBox.Text);
					int mines = int.Parse(MinesTextBox.Text);
					int possibleMines = (int)Math.Ceiling(width * height * 0.4 + 0.5);
					MinesTextBox.Text = Math.Min(mines, possibleMines).ToString();
					textBox.SelectionStart = textBox.Text.Length;
				}
			}
		}
	}
}
