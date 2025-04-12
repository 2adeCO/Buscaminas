using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace Buscaminas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MinePosition
    {
        int value;
        bool isMine;
        bool popped;
        bool isFlagged;
        Image flagImage;
        public MinePosition()
        {
            popped = false;
            isFlagged = false;
            isMine = false;
            flagImage = new Image();
            flagImage.Source = new BitmapImage(new Uri("pack://application:,,,/img/flagged.png")) ;
            flagImage.Stretch = System.Windows.Media.Stretch.UniformToFill;
        }
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
        public bool IsMine()
        {
            return isMine;
        }
        public bool IsPopped()
        {
            return popped;
        }
        public void ColorButton(Button button)
        {
            if (!IsPopped())
            {
                button.Background = Brushes.Gray;
            }
            {
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
                if (isMine)
                {

                    button.Background = Brushes.Red;
                    button.Content = "X";

                }
            }
                
                
            }
           
            
        public void Pop(MinePosition[,] MineField, int col, int row, int totalCols, int totalRows)
        {
            if (!this.IsPopped())
            {
                popped = true;
                if (!isMine)
                {
                    if (this.GetValue() == 0)
                    {
                        for (int i = col - 1; i < col + 2; i++)
                        {
                            for(int j = row - 1; j < row +2; j++)
                            {
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
        MinePosition[,] MineArray;
        Button[,] ButtonArray; 
        Random rnd = new Random();
        int currentCols = 0;
        int currentRows = 0;
        bool gameLost = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void UpdateMineField(bool gameEnd)
        {
            if (gameEnd)
            {
                MessageBox.Show("Petaste...", "OUUUCH");
                gameLost = true;
            }
            
            for (int i = 0; i < currentCols; i++)
            {
                for(int j = 0; j < currentRows; j++)
                {
                    if (!gameEnd)
                    {
                        if (MineArray[i, j].IsPopped())
                        {
                            MineArray[i, j].ColorButton(ButtonArray[i, j]);
                        }
                    }
                    else
                    {
                        if(MineArray[i, j].IsMine())
                        {
                            MineArray[i, j].Pop(MineArray,i,j,currentCols,currentRows);
                            MineArray[i, j].ColorButton(ButtonArray[i, j]);
                        }
                        
                    }
                }
            }
        }
        public void FlagMine(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button currentButton = (Button)sender;

                int col = Grid.GetColumn(currentButton);
                int row = Grid.GetRow(currentButton);
                Debug.WriteLine(col + "" + row + " " + MineArray[col, row].GetValue());


                MineArray[col, row].Flag(currentButton);
                UpdateMineField(false);
                GameState();
            }
        }
        public void DeployMines(object sender, EventArgs e)
        {
            gameLost = false;

            if(!int.TryParse(mines.Text, out int deployMines) || !int.TryParse(cols.Text, out currentCols) || !int.TryParse(rows.Text, out currentRows))
            {
                MessageBox.Show("Añade los atributos necesarios...");
                return;
            }
            if(deployMines >= currentCols * currentRows)
            {
                MessageBox.Show("Demasiadas minas para el campo...");
                return;
            }
            if(deployMines <= 0 || currentCols <= 0 || currentRows <= 0)
            {
                MessageBox.Show("Valores inválidos...");
                return;
            }


            MineField.Children.Clear();
            MineField.RowDefinitions.Clear();
            MineField.ColumnDefinitions.Clear();

            ButtonArray = new Button[currentCols, currentRows];
            MineArray = new MinePosition[currentCols,currentRows];

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
            for (int i = 0; i < currentCols; i++)
            {
                for (int j = 0; j < currentRows; j++)
                {
                    Button button = new Button();
                    button.Click += CheckMine;
                    button.MouseRightButtonUp += FlagMine;
                    button.Background = Brushes.Gray;
                    Grid.SetRow(button, j);
                    Grid.SetColumn(button, i);
                    MineField.Children.Add(button);
                    ButtonArray[i,j] = button;
                }
            }
            for (int i = 0; i < currentCols; i++)
            {
                for (int j = 0; j < currentRows; j++)
                {
                    MineArray[i, j] = new MinePosition(); 
                }
            }
            for (int i = 0; i < deployMines; i++)
            {
                int col;

                int row;

                do
                {
                    col = rnd.Next(0, currentCols);
                    row = rnd.Next(0, currentRows);

                } while (MineArray[col, row].IsMine());

                MineArray[col, row].MakeMine();
                Debug.WriteLine("Mine made at: " + col + row);

            }

            int value = 0;
            //Columnas minas
            for( int i = 0;i < currentCols; i++)
            {
                //Filas minas
                for(int j = 0;j < currentRows; j++)
                {
                    //Una columna arriba de la que estamos
                    for (int k = i-1; k < i+2; k++)
                    {
                        //Una fila arriba de la que estamos
                        for(int l = j-1; l < j+2; l++)
                        {
                            
                            
                                if (k >= 0 && k < currentCols && l >= 0 && l< currentRows)
                                {
                                    if (MineArray[k, l].IsMine())
                                    {
                                        value += 1;
                                    }
                                }
                           
                        }
                    }
                    Debug.WriteLine(i +""+ j + " " + value);
                    MineArray[i, j].SetValue(value);
                    value = 0;
                }
            }
            ChangeScreen(null, null);

        }
        public void CheckMine(object sender, EventArgs e)
        {
            if(sender is Button)
            {
                Button currentButton = (Button)sender;

                int col = Grid.GetColumn(currentButton);
                int row = Grid.GetRow(currentButton);
                Debug.WriteLine(col + "" + row + " " +MineArray[col, row].GetValue());
            
                
                MineArray[col, row].Pop(MineArray,col,row, currentCols,currentRows);
                UpdateMineField(MineArray[col,row].IsMine());
                GameState();
            }
        }
        public void GameState()
        {
            if (!gameLost)
            {
                for (int i = 0; i < currentCols; i++)
                {
                    for (int j = 0; j < currentRows; j++)
                    {
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
                                return ;
                            }
                        }
                    }
                }
                MessageBox.Show("¡Ganaste!","Enhorabuena");
            }
            
            
        }
        public void ChangeScreen(object sender, EventArgs e)
        {
                //Columnas 2
                //Filas 4
                //100x100
                if(MineField.ActualHeight == 0 || MineField.ActualWidth == 0)
            {
                Debug.WriteLine("Faulty resize skipped !");
                return;
            }
                Debug.WriteLine("Changing size detected !");
                double width = MineField.ActualWidth / currentCols;
                double height = MineField.ActualHeight / currentRows;

                double desiredSize = Math.Min(width, height);

            Debug.WriteLine("Desired size: " + desiredSize);
            Debug.WriteLine("Undesired size: " + Math.Max(width,height));
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
                    Debug.WriteLine("Size too small !");
                    for (int i = 0; i < currentRows; i++)
                    {
                        MineField.RowDefinitions[i] = new RowDefinition(){Height = new GridLength(1, GridUnitType.Star)};
                        Debug.WriteLine("Row count made: " + i);
                    }
                for (int i = 0; i < currentCols; i++)
                    {
                        MineField.ColumnDefinitions[i] = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                        Debug.WriteLine("Col count made: " + i);
                    }
                    }



         

        }
    }
}