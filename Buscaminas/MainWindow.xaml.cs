using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Buscaminas
{
    //class that will be used to store the mines and their values, such as the number of adjacent mines and whether or not they have been clicked
    public class MinePosition
    {
        //Value of the mine, 0-8
        int value;
        //True if the mine is a mine
        bool isMine;
        //True if the mine has been clicked
        bool popped;
        //True if the mine is flagged
        bool isFlagged;
        //Image that will be used to show a flagged mineposition
        Image flagImage;

        //Constructor
        public MinePosition()
        {
            popped = false;
            isFlagged = false;
            isMine = false;
            //Instance of the image
            flagImage = new Image();
            //Set the image source to the flagged image
            flagImage.Source = new BitmapImage(new Uri("pack://application:,,,/img/flagged.png"));
            //Set the image strech property to uniform to fill, so it will fill the button no matter the size
            flagImage.Stretch = System.Windows.Media.Stretch.UniformToFill;
        }
        //Standard setters and getters
        public void SetValue(int value)
        {
            this.value = value;
        }
        public void MakeMine()
        {
            isMine = true;
        }
        public int GetValue()
        {
            return this.value;
        }
        public bool IsFlagged()
        {
            return isFlagged;
        }
        public bool IsMine()
        {
            return isMine;
        }
        public bool IsPopped()
        {
            return popped;
        }
        //Method to flag the mine, if it is flagged, it will unflag it, if it is not flagged, it will flag it
        public void Flag(Button button)
        {
            isFlagged = !isFlagged;
            if (isFlagged)
            {
                button.Content = flagImage;
            }
            else
            {
                ColorButton(button);
            }
        }
        //Method to decorate the button
        public void ColorButton(Button button)
        {
            //Will only change the color of the button if it is popped
            if (!IsPopped())
            {
                button.Background = Brushes.Gray;
                button.Content = "";
            }
            else
            {
                //If the mine is popped, it will change the color of the button depending on the value of the mine
                switch (GetValue())
                {
                    case 0:
                        {
                            button.Background = Brushes.LightGreen;
                            button.Content = "";
                            break;
                        }
                    case 1:
                        {
                            button.Background = Brushes.LawnGreen;
                            button.Content = GetValue().ToString();
                            break;
                        }
                    case 2:
                        {
                            button.Background = Brushes.DarkSeaGreen;
                            button.Content = GetValue().ToString();
                            break;
                        }
                    case >= 3:
                        {
                            button.Background = Brushes.DarkOliveGreen;
                            button.Content = GetValue().ToString();
                            break;
                        }

                }
                //If the mine is a mine, it will change the color of the button to red and set the content to X
                if (isMine)
                {

                    button.Background = Brushes.Red;
                    button.Content = "X";

                }
            }
        }

        //Method to pop the mine
        public void Pop(MinePosition[,] MineField, int col, int row, int totalCols, int totalRows)
        {
            //Will only pop the mine if it is not already popped
            if (!this.IsPopped())
            {
                popped = true;
                //If it's not a mine, it will check its surrounding to pop them too (Requiring that its own value is 0, and such surrounding mine is 0 too)
                if (!isMine)
                {
                    if (this.GetValue() == 0)
                    {
                        //i is the column, j is the row, will check in a 3x3 grid around the mine
                        for (int i = col - 1; i < col + 2; i++)
                        {
                            for (int j = row - 1; j < row + 2; j++)
                            {
                                //Will check if the mine is in the bounds of the grid
                                if (i >= 0 && i < totalCols && j >= 0 && j < totalRows)
                                {
                                    if (!MineField[i, j].IsPopped())
                                    {
                                        MineField[i, j].Pop(MineField, i, j, totalCols, totalRows);
                                    }

                                }
                            }

                        }
                    }
                }

            }

        }
    }
    public partial class MainWindow : Window
    {
        //Grid that will be used to store the mines and play the game
        MinePosition[,] MineArray;
        //Grid that will be used to store the buttons and interact with their colors
        Button[,] ButtonArray;
        //Random number generator
        Random rnd;
        //Current number of columns and rows
        int currentCols;
        int currentRows;
        //True if the game is lost, false if the game is still winnable
        bool gameLost;
        //Constructor
        public MainWindow()
        {
            InitializeComponent();
            rnd = new Random();
            currentCols = 0;
            currentRows = 0;
            gameLost = false;
        }
        //Method that will be used to update the minefield, it will change the color of the buttons depending on the value of the mine
        public void UpdateMineField(bool gameEnd)
        {
            //If the game is lost, it will show a message to the user
            if (gameEnd)
            {
                MessageBox.Show("Petaste...", "OUUUCH");
                gameLost = true;
            }
            //i is columns, j is rows
            for (int i = 0; i < currentCols; i++)
            {
                for (int j = 0; j < currentRows; j++)
                {
                    //If the game is not lost, it will check if the mine is popped, if it is, it will change the color of the button
                    if (!gameEnd)
                    {
                        if (MineArray[i, j].IsPopped())
                        {
                            MineArray[i, j].ColorButton(ButtonArray[i, j]);
                        }
                    }
                    else
                    {
                        //If the game is lost, it will check if the mine is a mine, if it is, it will reveal it
                        if (MineArray[i, j].IsMine())
                        {
                            MineArray[i, j].Pop(MineArray, i, j, currentCols, currentRows);
                            MineArray[i, j].ColorButton(ButtonArray[i, j]);
                        }

                    }
                }
            }
        }
        //Method to flag the mine, it receives the button that was clicked and the event
        public void FlagMine(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                //Get the button that was clicked
                Button currentButton = (Button)sender;
                //Get its column and row
                int col = Grid.GetColumn(currentButton);
                int row = Grid.GetRow(currentButton);
                //Debug the column and row
                Debug.WriteLine(col + "" + row + " " + MineArray[col, row].GetValue());
                //Flag the mine
                MineArray[col, row].Flag(currentButton);
                //Update the minefield with the parameter that the game hasn't been lost;
                UpdateMineField(false);
                //Check if this flag wins the game
                GameState();
            }
        }
        //Method that will be used change the grid cols and rows and generate a minefield with them
        public void DeployMines(object sender, EventArgs e)
        {
            //Because this is a new game, we will reset the gameLost variable to false
            gameLost = false;
            //We will parse the values of the mines, columns and rows
            if (!int.TryParse(Mines.Text, out int deployMines) || !int.TryParse(Cols.Text, out currentCols) || !int.TryParse(Rows.Text, out currentRows))
            {
                //If the parsing fails, we will show a message to the user
                MessageBox.Show("Añade los atributos necesarios...");
                return;
            }
            //If the number of mines is greater or equal to the number of total cells, we will show a message to the user
            if (deployMines >= currentCols * currentRows)
            {
                MessageBox.Show("Demasiadas minas para el campo...");
                return;
            }
            //If the number of mines, columns or rows is less than or equal to 0, we will show a message to the user
            if (deployMines <= 0 || currentCols <= 0 || currentRows <= 0)
            {
                MessageBox.Show("Valores inválidos...");
                return;
            }

            //Clearing of the grid's past values
            MineField.Children.Clear();
            MineField.RowDefinitions.Clear();
            MineField.ColumnDefinitions.Clear();


            //Creating a column and row definition for each column and row
            for (int i = 0; i < currentCols; i++)
            {
                MineField.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                Debug.WriteLine(i + " col");

            }
            for (int j = 0; j < currentRows; j++)
            {
                MineField.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                Debug.WriteLine(j + " row");
            }


            //Instances of the arrays that will play our game
            ButtonArray = new Button[currentCols, currentRows];
            MineArray = new MinePosition[currentCols, currentRows];
            //Creating the buttons and adding them to the grid
            for (int i = 0; i < currentCols; i++)
            {
                for (int j = 0; j < currentRows; j++)
                {
                    Button button = new Button();
                    //Adding actions to the buttons                    
                    button.Click += CheckMine;
                    button.MouseRightButtonUp += FlagMine;
                    //Adding gray as the unpopped color
                    button.Background = Brushes.Gray;
                    //Adding the button to the grid
                    Grid.SetRow(button, j);
                    Grid.SetColumn(button, i);
                    MineField.Children.Add(button);
                    //Adding the button to the array
                    ButtonArray[i, j] = button;
                }
            }
            //Creating the mines in the arrays
            for (int i = 0; i < currentCols; i++)
            {
                for (int j = 0; j < currentRows; j++)
                {
                    MineArray[i, j] = new MinePosition();
                }
            }
            //Creating true mines in the grid
            for (int i = 0; i < deployMines; i++)
            {
                int col;
                int row;
                //We will only pass a position if it's not a mine already
                do
                {
                    col = rnd.Next(0, currentCols);
                    row = rnd.Next(0, currentRows);

                } while (MineArray[col, row].IsMine());

                MineArray[col, row].MakeMine();
                //Debug the mine making
                Debug.WriteLine("Mine made at: " + col + row);

            }

            int value = 0;
            //i is cols, j is rows, k(cols) and l(rows) are the surrounding mines
            for (int i = 0; i < currentCols; i++)
            {
                for (int j = 0; j < currentRows; j++)
                {
                    for (int k = i - 1; k < i + 2; k++)
                    {
                        for (int l = j - 1; l < j + 2; l++)
                        {
                            //If its not out of bounds, we will check if the mine position is a mine
                            if (k >= 0 && k < currentCols && l >= 0 && l < currentRows)
                            {
                                //For each mine around the current position that is a mine, we will add 1 to the value
                                if (MineArray[k, l].IsMine())
                                {
                                    value += 1;
                                }
                            }

                        }
                    }
                    //Debug the mine value
                    Debug.WriteLine(i + "" + j + " " + "has a value of: " + value);
                    //Set the value operated and set the value back to 0 for the next iteration
                    MineArray[i, j].SetValue(value);
                    value = 0;
                }
            }
            //Setting the size of the buttons to the size of the grid so that they will show as squares
            ChangeScreen(null, null);

        }
        //Method that will be used to interact with the mines
        public void CheckMine(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                //Get the button that was clicked
                Button currentButton = (Button)sender;
                //Get its position
                int col = Grid.GetColumn(currentButton);
                int row = Grid.GetRow(currentButton);
                //Debug the column and row and its value
                Debug.WriteLine(col + "" + row + " has a value of: " + MineArray[col, row].GetValue());

                //Pop the current mine
                MineArray[col, row].Pop(MineArray, col, row, currentCols, currentRows);
                //Update the color of the whole grid in case the game is lost or multiple mine positions are popped
                UpdateMineField(MineArray[col, row].IsMine());
                //Check if the game is lost
                GameState();
            }
        }
        //Method that will be used to check if the game is lost, won or still winnable
        public void GameState()
        {
            //Can only win if gameLost is false
            if (!gameLost)
            {
                for (int i = 0; i < currentCols; i++)
                {
                    for (int j = 0; j < currentRows; j++)
                    {
                        //Checking if all the true mines have been flagged and all the false mines have been popped
                        if (MineArray[i, j].IsMine())
                        {
                            if (!MineArray[i, j].IsFlagged())
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (!MineArray[i, j].IsPopped())
                            {
                                return;
                            }
                        }
                    }
                }
                //Congratulations if the game is won
                MessageBox.Show("¡Ganaste!", "Enhorabuena");
            }


        }
        //Method that will be used to change the size of the buttons to the size of the grid
        public void ChangeScreen(object sender, EventArgs e)
        {
            //If the grid is empty or has no size, we will skip the resize
            if (MineField.ActualHeight == 0 || MineField.ActualWidth == 0)
            {
                Debug.WriteLine("Faulty resize skipped !");
                return;
            }

            Debug.WriteLine("Changing size detected !");
            double width = MineField.ActualWidth / currentCols;
            double height = MineField.ActualHeight / currentRows;
            //Will get the smaller of the two sides to make the buttons square
            double desiredSize = Math.Min(width, height);
            //Debug of the size
            Debug.WriteLine("Desired size: " + desiredSize);
            Debug.WriteLine("Undesired size: " + Math.Max(width, height));
            //If the desired size can fit in the grid, we will set the size of the buttons to the desired size
            if (desiredSize * currentCols <= this.ActualWidth && desiredSize * currentRows <= ((this.ActualHeight / 10) * 7))
            {
                Debug.WriteLine("Establishing desired size !");
                for (int i = 0; i < currentRows; i++)
                {
                    MineField.RowDefinitions[i].Height = new GridLength(desiredSize);
                }
                for (int i = 0; i < currentCols; i++)
                {
                    MineField.ColumnDefinitions[i].Width = new GridLength(desiredSize);
                }
            }

            else
            {
                //If the size has to be smaller, we will set the size of the buttons to 1 star so that next iteration it will be resized correctly
                Debug.WriteLine("Size too small !");
                for (int i = 0; i < currentRows; i++)
                {
                    MineField.RowDefinitions[i] = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
                }
                for (int i = 0; i < currentCols; i++)
                {
                    MineField.ColumnDefinitions[i] = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                }
            }
        }
    }
}