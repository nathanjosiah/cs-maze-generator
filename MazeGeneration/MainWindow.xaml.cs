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

        public void renderMaze(int size)
        {
            if(maze == null)
            {
                return;
            }

            MazeRenderer renderer = new MazeRenderer(size);
            renderer.renderToElement(maze, canvas);

        }

        private void btnGenerateMaze_Click(object sender, RoutedEventArgs e)
        {
            createMaze();
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Maze.png"; 
            dlg.DefaultExt = ".png"; 
            dlg.Filter = "Image Documents (.png)|*.png"; 

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                CanvasRenderer renderer = new CanvasRenderer();
                renderer.createBitmapFromVisual(canvas, dlg.FileName);
            }
            return; 
        }

        private void txtNumRows_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            numRows = Convert.ToInt32(text.Text);
        }

        private void txtNumColumns_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            numColumns = Convert.ToInt32(text.Text);
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
}
