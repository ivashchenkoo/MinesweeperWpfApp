using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MinesweeperModel;

namespace MinesweeperWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        // max board size for the custom difficulty
        const int MaxBoardSize = 100;

        // the coefficient of the maximum number of mines relative to the dimensions of the board
        const double MinesCoef = 0.4;

        // width and height of buttons to generate
        const int ButtonSize = 20;

        // contents of buttons to generate
        const string ButtonDefaultContent = "";
        const string ButtonMineContent = "X";
        const string ButtonFlagContent = "M";

        // game state contents for the restart button
        const string RestartButtonDefaultContent = "☺";
        const string RestartButtonVictoryContent = "☻";
        const string RestartButtonDefeatContent = "😕";

        // background colors of buttons to generate
        private readonly SolidColorBrush _openedCellBrush = new SolidColorBrush(Color.FromRgb(190, 190, 190));
        private readonly SolidColorBrush _flagCellBrush = new SolidColorBrush(Color.FromRgb(255, 212, 138));
        private readonly SolidColorBrush _explodedCellBrush = new SolidColorBrush(Color.FromRgb(255, 71, 71));
        private readonly SolidColorBrush _defaultCellBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221));

        // timer to keep track of game time
        private readonly DispatcherTimer _timer;
        // the current game time
        private TimeSpan _time;

        // a tag for tracking the state of the game 
        private bool _gameOver;
        // the minesweeper board
        private MinesweeperBoard _board;
        // the buttons grid
        private Button[,] _buttons;
        #endregion

        // constructor
        public MainWindow()
        {
            InitializeComponent();

            // reset game time
            _time = TimeSpan.FromSeconds(0);

            // initialize the timer
            _timer = new DispatcherTimer(
                new TimeSpan(0, 0, 1), DispatcherPriority.Normal,
                delegate
                {
                    TimerTextBlock.Text = _time.ToString("m':'ss");
                    _time = _time.Add(TimeSpan.FromSeconds(1));
                },
                Application.Current.Dispatcher
            );

            // get default difficulties
            var standardDifficulties = Difficulty.GetDefaultDifficulties();

            // add "Custom" difficulty to the difficulties array
            Difficulty[] difficulties = new Difficulty[standardDifficulties.Length + 1];
            for (int i = 0; i < standardDifficulties.Length; i++)
            {
                difficulties[i] = standardDifficulties[i];
            }
            difficulties[standardDifficulties.Length] = new Difficulty
            {
                Description = "Custom"
            };

            // set the difficulties array as itemsource for the DifficultyCombox
            DifficultyComboBox.ItemsSource = difficulties;

            // set the selection for the DifficultyCombox
            DifficultyComboBox.SelectedIndex = 0;
        }

        #region Methods
        /// <summary>
        /// Generates the buttons grid for the minesweeper board
        /// </summary>
        private void GenerateButtonGrid()
        {
            // clear the MinesweeperBoardGrid
            MinesweeperBoardGrid.Children.Clear();

            // create the 2D array of the type Button with the board dimensions
            _buttons = new Button[_board.Height, _board.Width];

            // create the grid and initialize its rows and columns
            Grid grid = new Grid();
            for (int x = 0; x < _board.Height; x++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int y = 0; y < _board.Width; y++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // fill the 2D buttons array with buttons, each on the unique position in the grid
            for (int i = 0; i < _board.Height; i++)
            {
                for (int j = 0; j < _board.Width; j++)
                {
                    _buttons[i, j] = new Button
                    {
                        // set the button size and aligment
                        Width = ButtonSize,
                        Height = ButtonSize,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    // set event handlers for the button
                    _buttons[i, j].PreviewMouseLeftButtonUp += CellLeftClickHandler;
                    _buttons[i, j].PreviewMouseRightButtonUp += CellRightClickHandler;

                    // set the defualt background color for the button
                    _buttons[i, j].Background = _defaultCellBrush;

                    // set the button position in the grid
                    Grid.SetRow(_buttons[i, j], i);
                    Grid.SetColumn(_buttons[i, j], j);

                    // add the button to the grid
                    grid.Children.Add(_buttons[i, j]);
                }
            }

            // add the generated grid to the MinesweeperBoardGrid
            MinesweeperBoardGrid.Children.Add(grid);
        }

        /// <summary>
        /// Handles the input values of the specified textbox and changes the contents of others if needed
        /// </summary>
        /// <param name="textBox">The textbox for handling the input data</param>
        /// <param name="maxValue">The maximum value for the specified textBox</param>
        private void HandleDifficultyTextboxes(TextBox textBox, int maxValue)
        {
            // remove all characters except numbers from the text
            string text = Regex.Match(textBox.Text, "[0-9]+").Value;

            // try to parse the received text as an integer
            if (int.TryParse(text, out int digits))
            {
                // if the parsed value is greater than maxValue
                if (digits > maxValue)
                {
                    // set the textbox text as maxValue
                    textBox.Text = maxValue.ToString();
                    // set caret to end of text
                    textBox.SelectionStart = textBox.Text.Length;
                    return;
                }
                // if the parsed value is less than or equal to zero
                else if (digits <= 0)
                {
                    // set the textbox text as 1
                    textBox.Text = "1";
                    // set caret to end of text
                    textBox.SelectionStart = textBox.Text.Length;
                    return;
                }

                // if dimensions textboxes are not empty
                if (WidthTextBox.Text != string.Empty && HeightTextBox.Text != string.Empty && MinesTextBox.Text != string.Empty)
                {
                    // fill in the difficulty dimensionals with textbox`s texts
                    int width = int.Parse(WidthTextBox.Text);
                    int height = int.Parse(HeightTextBox.Text);
                    int mines = int.Parse(MinesTextBox.Text);

                    // calculate the possible number of mines for the specified dimensions
                    int possibleMines = (int)Math.Ceiling(width * height * MinesCoef);
                    // set the max mines number to the mines textbox
                    MinesTextBox.Text = Math.Min(mines, possibleMines).ToString();
                    // set caret to end of text
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

        /// <summary>
        /// Restarts the game
        /// </summary>
        /// <param name="width">The width of the new minesweeper board</param>
        /// <param name="height">The height of the new minesweeper board</param>
        /// <param name="mines">The mines count of the new minesweeper board</param>
        private void RestartGame(int width, int height, int mines)
        {
            // reset the game state
            _gameOver = false;
            // set the default content for the restart button
            RestartButton.Content = RestartButtonDefaultContent;
            // create a new minesweeper board instance
            _board = new MinesweeperBoard(width, height, mines);
            // regenerate the buttons grid for a new board
            GenerateButtonGrid();
            // set the mines count to the Text property of the MinesCountTextbox 
            MinesCountTextBlock.Text = _board.MinesNumber.ToString();

            // clear the timer textbox
            TimerTextBlock.Text = "0:00";
            // reset game time
            _time = TimeSpan.FromSeconds(0);
            // stop the timer
            _timer.Stop();
        }

        /// <summary>
        /// Restarts the game
        /// </summary>
        /// <param name="difficulty">The instance of the Difficulty class</param>
        private void RestartGame(Difficulty difficulty) => RestartGame(difficulty.Width, difficulty.Height, difficulty.MinesNumber);

        /// <summary>
        /// Updates the buttons grid with the state of the board cells
        /// </summary>
        private void UpdateButtonsGrid()
        {
            // start a timer when the button is pressed for the first time in a new game
            if (!_timer.IsEnabled && !_gameOver)
            {
                _timer.Start();
            }

            // go through all the buttons
            for (int i = 0; i < _board.Height; i++)
            {
                for (int j = 0; j < _board.Width; j++)
                {
                    // if a button has the mine flag
                    if (_board.Grid[i, j].IsMarked)
                    {
                        // set the flag backround color for the button
                        _buttons[i, j].Background = _flagCellBrush;
                        // set the flag content for the button
                        _buttons[i, j].Content = ButtonFlagContent;
                        continue;
                    }

                    // set the default backround color for the button
                    _buttons[i, j].Background = _defaultCellBrush;
                    // set the default content for the button
                    _buttons[i, j].Content = ButtonDefaultContent;

                    // if the cell with specified x and y is open
                    if (_board.Grid[i, j].IsOpen)
                    {
                        // set the open background color for the button
                        _buttons[i, j].Background = _openedCellBrush;
                        // if the cell with specified x and y has mine
                        if (_board.Grid[i, j].HasMine)
                        {
                            // set the mine content for the button
                            _buttons[i, j].Content = ButtonMineContent;
                        }
                        // if NumberOfMinesAround property is greater than zero
                        else if (_board.Grid[i, j].NumberOfMinesAround > 0)
                        {
                            // set cell NumberOfMinesAround as button content
                            _buttons[i, j].Content = _board.Grid[i, j].NumberOfMinesAround;
                        }
                        else
                        {
                            // set the default content for the button if NumberOfMinesAround property
                            _buttons[i, j].Content = ButtonDefaultContent;
                        }
                    }
                }
            }

            // update the MinesCountTextBlock text with the difference between the total number of mines on the board and the marked cells number
            MinesCountTextBlock.Text = (_board.MinesNumber - _board.MarkedCellsNumber).ToString();
        }
        #endregion

        #region EventHandlers
        /// <summary>
        /// The event handler for the LMB click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellLeftClickHandler(object sender, RoutedEventArgs e)
        {
            // return if the game state is GameOver or the button has the flag tag
            if (_gameOver)
            {
                return;
            }

            // cast the sender parameter as the Button
            Button button = (Button)sender;

            // initialize x and y variables from the row and column of the button
            int x = Grid.GetRow(button);
            int y = Grid.GetColumn(button);

            // get a board cell with x and y coordinates
            Cell cell = _board.Grid[x, y];

            if (cell.IsMarked)
            {
                return;
            }

            // open cell
            _board.OpenCell(cell);

            // if the board is complete
            if (_board.IsBoardComplete)
            {
                // set the flag tag for all buttons that correspond to cells with mines
                var cellsHaveMines = _board.GetCellsWithMines();
                for (int i = 0; i < cellsHaveMines.Length; i++)
                {
                    cellsHaveMines[i].IsMarked = true;
                }

                // set the text of the MinesCountTextBlock as zero
                MinesCountTextBlock.Text = "0";
                // set the game state as GameOver
                _gameOver = true;
                // set the victory content for the restart button
                RestartButton.Content = RestartButtonVictoryContent;
                // stop the timer
                _timer.Stop();

                MessageBox.Show("You win!");
            }

            // update the buttons grid
            UpdateButtonsGrid();

            // if the cell has mine
            if (cell.HasMine)
            {
                // set the exploded background color for the button
                button.Background = _explodedCellBrush;
                // set the game state as GameOver
                _gameOver = true;
                // set the defeat content for the restart button
                RestartButton.Content = RestartButtonDefeatContent;
                // stop the timer
                _timer.Stop();

                MessageBox.Show("Game over!", "Minesweeper");
            }
        }

        /// <summary>
        /// The event handler for the RMB click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellRightClickHandler(object sender, MouseButtonEventArgs e)
        {
            // return if the game state is GameOver
            if (_gameOver)
            {
                return;
            }

            // cast the sender parameter as the Button
            Button button = (Button)sender;

            // initialize x and y variables from the row and column of the button
            int x = Grid.GetRow(button);
            int y = Grid.GetColumn(button);

            // reversing the state of the cell mark
            _board.Grid[x, y].IsMarked = !_board.Grid[x, y].IsMarked;

            // update the buttons grid
            UpdateButtonsGrid();
        }

        /// <summary>
        /// The event handler for the DifficultyComboBox selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // cast the selected combobox item to a Difficulty class
            Difficulty difficulty = (Difficulty)DifficultyComboBox.SelectedItem;

            // if "Custom" difficulty is selected
            if (difficulty.Description == "Custom")
            {
                // set dimensions textboxes visibility as visible
                DimensionalGrid.Visibility = Visibility.Visible;

                // clear textboxes
                WidthTextBox.Text = string.Empty;
                HeightTextBox.Text = string.Empty;
                MinesTextBox.Text = string.Empty;
                return;
            }
            else
            {
                // set dimensions textboxes visibility as collapsed
                DimensionalGrid.Visibility = Visibility.Collapsed;
            }

            // restart the game
            RestartGame(difficulty);
        }

        /// <summary>
        /// The event handler for the dimensions textboxes text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dimension_TextChanged(object sender, TextChangedEventArgs e)
        {
            // cast the sender parameter as the TextBox
            TextBox textBox = (TextBox)sender;

            HandleDifficultyTextboxes(textBox, MaxBoardSize);
        }

        /// <summary>
        /// The event handler for the mines number textbox text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinesNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // cast the sender parameter as the TextBox
            TextBox textBox = (TextBox)sender;

            HandleDifficultyTextboxes(textBox, (int)Math.Ceiling(MaxBoardSize * MaxBoardSize * MinesCoef));
        }

        /// <summary>
        /// The event handler of the restart button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // cast the selected combobox item to a Difficulty class
            Difficulty difficulty = (Difficulty)DifficultyComboBox.SelectedItem;

            // if "Custom" difficulty is selected
            if (difficulty.Description == "Custom")
            {
                // if dimension textboxes are not empty
                if (WidthTextBox.Text != string.Empty && HeightTextBox.Text != string.Empty && MinesTextBox.Text != string.Empty)
                {
                    // fill in the difficulty dimensionals with textbox`s texts
                    difficulty.Width = int.Parse(WidthTextBox.Text);
                    difficulty.Height = int.Parse(HeightTextBox.Text);
                    difficulty.MinesNumber = int.Parse(MinesTextBox.Text);
                }
                else
                {
                    return;
                }
            }

            // restart the game with the selected difficulty
            RestartGame(difficulty);
        }
        #endregion
    }
}
