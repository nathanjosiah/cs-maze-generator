using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MazeGeneration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Maze maze;
        int numRows = 30;
        int numColumns = 20;
        int cellSize = 15;

        public MainWindow()
        {
            InitializeComponent();
            createMaze();
        }

        public void createMaze()
        {
            maze = new Maze(numRows, numColumns);
            renderMaze(cellSize);
        }

        public Line createLine()
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            return myLine;
        }

        public void renderMaze(int size)
        {
            if(maze == null)
            {
                return;
            }
            canvas.Children.Clear();
            foreach (List<Cell> row in maze.getCells())
            {
                foreach (Cell cell in row)
                {

                    Line myLine;

                    if (!cell.openSides.Contains(Direction.NORTH))
                    {
                        // Top
                        myLine = createLine();
                        myLine.X1 = cell.column * size;
                        myLine.X2 = (cell.column + 1) * size;
                        myLine.Y1 = cell.row * size;
                        myLine.Y2 = cell.row * size;
                        canvas.Children.Add(myLine);
                    }

                    if (!cell.openSides.Contains(Direction.EAST))
                    {
                        // Right
                        myLine = createLine();
                        myLine.X1 = (cell.column + 1) * size;
                        myLine.X2 = (cell.column + 1) * size;
                        myLine.Y1 = cell.row * size;
                        myLine.Y2 = (cell.row + 1) * size;
                        canvas.Children.Add(myLine);
                    }

                    if (!cell.openSides.Contains(Direction.SOUTH))
                    {
                        // Bottom
                        myLine = createLine();
                        myLine.X1 = cell.column * size;
                        myLine.X2 = (cell.column + 1) * size;
                        myLine.Y1 = (cell.row + 1) * size;
                        myLine.Y2 = (cell.row + 1) * size;
                        canvas.Children.Add(myLine);
                    }

                    if (!cell.openSides.Contains(Direction.WEST))
                    { 
                        // Left
                        myLine = createLine();
                        myLine.X1 = cell.column * size;
                        myLine.X2 = cell.column * size;
                        myLine.Y1 = cell.row * size;
                        myLine.Y2 = (cell.row + 1) * size;
                        canvas.Children.Add(myLine);
                    }
                }
            }

            return;
            /*Console.Write("=========\n");
            foreach (List<Cell> crow in maze.getCells())
            {
                foreach (Cell ccell in crow)
                {
                    Console.Write("" + ccell.openSide + "\t|");
                }
                Console.Write("\n");
            }
            Console.Write("=========\n\n\n");*/
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (sender == generateButton)
            {
                createMaze();
            }
            else
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "Maze.png"; // Default file name
                dlg.DefaultExt = ".png"; // Default file extension
                dlg.Filter = "Image Documents (.png)|*.png"; // Filter files by extension

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    CreateBitmapFromVisual(canvas, dlg.FileName);
                }
                return;

                PrintDialog prnt = new PrintDialog();
                prnt.ShowDialog();
                prnt.PrintVisual(canvas, "Printing Canvas");
            }
        }

        void CreateBitmapFromVisual(Visual target, string filename)
        {
            if (target == null)
                return;

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);

            RenderTargetBitmap rtb = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();

            png.Frames.Add(BitmapFrame.Create(rtb));

            using (Stream stm = File.Create(filename))
            {
                png.Save(stm);
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if(sender == txtRows)
            {
                numRows = Convert.ToInt32(text.Text);
            }
            else
            {
                numColumns = Convert.ToInt32(text.Text);
            }
        }

        private void sldSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            cellSize = Convert.ToInt32(slider.Value);
            if (cellSizeLabel != null)
            {
                cellSizeLabel.Content = "Cell size: " + cellSize;
            }
            renderMaze(cellSize);
        }
    }

    public class Maze
    {
        private int numRows, numColumns;
        private List<List<Cell>> cells;
        private Random random = new Random();
        private Cell deepestCell;
        private int deepestCellCount = 0;
        private int currentCount = 0;

        public Maze(int rows, int columns)
        {
            numRows = rows;
            numColumns = columns;

            this.cells = this.generateGrid(rows, columns);

            makeMaze(0,0, new List<Cell>());

            markOpenCell(deepestCell);
        }

        private void makeMaze(int row, int column, List<Cell>visited)
        {
            Cell currentCell = cells[row][column];

            if(visited.Count() == 0)
            {

                markOpenCell(currentCell);
            }

            List<NeighborCell> neighbors = getSurroundingCells(row, column, cells);

            visited.Add(cells[row][column]);

            while (true)
            {
                List<NeighborCell> unvisited = new List<NeighborCell>();
                foreach (NeighborCell possible in neighbors)
                {
                    if (visited.Contains(possible.cell))
                    {
                        continue;
                    }
                    unvisited.Add(possible);
                }

                if (unvisited.Count() == 0)
                {
                    if(deepestCellCount < currentCount && isExternalCell(currentCell))
                    {
                        deepestCellCount = currentCount;
                        deepestCell = currentCell;
                    }
                    currentCount -= 1;
                    break;
                }

                int index = random.Next(0, unvisited.Count());
                NeighborCell cell = unvisited[index];
                Cell neighbor_cell = cell.cell;
                neighbor_cell.openSides.Add(cell.openSide);
                currentCell.openSides.Add(cell.direction);

                currentCount += 1;
                this.makeMaze(cell.cell.row, cell.cell.column, visited);
            }

        }

        public List<List<Cell>> generateGrid(int rows,int columns)
        {
            List<List<Cell>> cells = new List<List<Cell>>();
            for (int i = 0; i < rows; i++)
            {
                List<Cell> row = new List<Cell>();

                for (int j = 0; j < columns; j++)
                {
                    Cell cell = new Cell(i,j);
                    row.Add(cell);
                }

                cells.Add(row);
            }

            return cells;
        }

        public List<NeighborCell> getSurroundingCells(int row, int column, List<List<Cell>> cells)
        { 

            List<NeighborCell> surrounding = new List<NeighborCell>();
            // N
            if (row - 1 >= 0)
            {
                surrounding.Add(new NeighborCell(cells[row - 1][column],Direction.NORTH, Direction.SOUTH));
            }

            // S
            if (row + 2 <= numRows)
            {
                surrounding.Add(new NeighborCell(cells[row + 1][column], Direction.SOUTH, Direction.NORTH));
            }

            // E
            if (column + 2 <= numColumns)
            {
                surrounding.Add(new NeighborCell(cells[row][column + 1], Direction.EAST, Direction.WEST));
            }

            // W
            if (column - 1 >= 0)
            {
                surrounding.Add(new NeighborCell(cells[row][column - 1], Direction.WEST, Direction.EAST));
            }

            return surrounding;
        }

        public List<List<Cell>> getCells()
        {
            return cells;
        }

        private void markOpenCell(Cell cell)
        {
            if (cell.row == 0)
            {
                cell.openSides.Add(Direction.NORTH);
            }
            else if (cell.row == numRows - 1)
            {
                cell.openSides.Add(Direction.SOUTH);
            }
            else if (cell.column == 0)
            {
                cell.openSides.Add(Direction.WEST);
            }
            else if (cell.column == numColumns - 1)
            {
                cell.openSides.Add(Direction.EAST);
            }
        }
        private bool isExternalCell(Cell cell)
        {
            return (cell.row == 0 || cell.row == numRows - 1 || cell.column == 0 || cell.column == numColumns - 1);
        }
    }

    public class Cell
    {
        public int row { get; }
        public int column {get; }
        public List<Direction> openSides = new List<Direction>();

        public Cell(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }

    public enum Direction
    {
        BLANK = 0,
        NORTH = 1,
        EAST = 2,
        SOUTH = 3,
        WEST = 4
    }

    public class NeighborCell
    {
        public Cell cell { get; }
        public Direction openSide { get; }
        public Direction direction { get; }

        public NeighborCell(Cell cell, Direction direction,Direction openSide)
        {
            this.cell = cell;
            this.openSide = openSide;
            this.direction = direction;
        }
    }
}
