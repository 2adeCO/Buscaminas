using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        public MinePosition()
        {
            popped = false;
            isMine = false;
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
        public bool IsMine()
        {
            return isMine;
        }
        public bool IsPopped()
        {
            return popped;
        }
        public void Pop(MinePosition[,] MineField, int col, int row, int totalCols, int totalRows)
        {
            if (!this.IsPopped())
            {
                popped = true;
                if (this.IsMine())
                {
                    MessageBox.Show("Petaste...");

                }
                else
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
        public MainWindow()
        {
            InitializeComponent();
        }
        public void UpdateMineField(bool gameEnd)
        {
            for(int i = 0; i < currentCols; i++)
            {
                for(int j = 0; j < currentRows; j++)
                {
                    if (!gameEnd)
                    {
                        if (MineArray[i, j].IsPopped())
                        {
                            if (MineArray[i, j].GetValue() == 0)
                            {
                                ButtonArray[i, j].Background = Brushes.LightGreen;
                                ButtonArray[i, j].Content = " ";
                            }
                            else
                            {
                                if (MineArray[i, j].GetValue() == 1)
                                {
                                    ButtonArray[i, j].Background = Brushes.LawnGreen;
                                    ButtonArray[i, j].Content = MineArray[i, j].GetValue().ToString();
                                }
                                else
                                {
                                    if (MineArray[i, j].GetValue() == 2)
                                    {
                                        ButtonArray[i, j].Background = Brushes.DarkOliveGreen;
                                        ButtonArray[i, j].Content = MineArray[i, j].GetValue().ToString();
                                    }
                                    else
                                    {
                                        if (MineArray[i, j].GetValue() >= 3)
                                        {
                                            ButtonArray[i, j].Background = Brushes.DarkSeaGreen;
                                            ButtonArray[i, j].Content = MineArray[i, j].GetValue().ToString();
                                        }
                                    }
                                }

                            }
                            if (MineArray[i, j].IsMine())
                            {
                                ButtonArray[i, j].Background = Brushes.Red;
                                ButtonArray[i, j].Content = "X";
                            }
                        }

                    }
                    else
                    {
                        if(MineArray[i, j].IsMine())
                        {
                            ButtonArray[i, j].Background = Brushes.Red;
                            ButtonArray[i, j].Content = "X";
                        }
                    }
                }
            }
        }
        public void DeployMines(object sender, EventArgs e)
        {
            int deployMines = int.Parse(mines.Text);
            currentCols = int.Parse(cols.Text);
            currentRows = int.Parse(rows.Text);

            minefield.Children.Clear();
            minefield.RowDefinitions.Clear();
            minefield.ColumnDefinitions.Clear();

            ButtonArray = new Button[currentCols, currentRows];
            MineArray = new MinePosition[currentCols,currentRows];

            for (int i = 0; i < currentCols; i++)
            {
                minefield.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                Debug.WriteLine(i + " col"); 
                
            }
            for (int j = 0; j < currentRows; j++)
            {
                minefield.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                Debug.WriteLine(j + " row");   
            }
            for (int i = 0; i < currentCols; i++)
            {
                for (int j = 0; j < currentRows; j++)
                {
                    Button button = new Button();
                    button.Click += CheckMine;
                    Grid.SetRow(button, j);
                    Grid.SetColumn(button, i);
                    minefield.Children.Add(button);
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
            }
        }
        public void ChangeScreen(object sender, EventArgs e)
        {
            if(sender is Grid)
            {
                Grid myGrid = (Grid)sender;
                //Columnas 2
                //Filas 4
                //100x100

                double width = myGrid.Width / currentCols;
                double height = myGrid.Height / currentRows;

                if(width > height)
                {
                    myGrid.Margin = new Thickness(height / width / 2, 0, height / width / 2, 0);
                }
                else
                {
                    if(width < height)
                    {
                        myGrid.Margin = new Thickness((width/ height) / 2, 0, width/ height/ 2, 0);

                    }
                    else
                    {

                    }
                }
                

            }

        }
    }
}