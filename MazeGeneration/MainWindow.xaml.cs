﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        Color selectedColor = Colors.LightSteelBlue;
        Progress<int> progress;

        public MainWindow()
        {
            InitializeComponent();
            progress = new Progress<int>(percent =>
            {
                progressBar.Value = percent;
            });
            colorPicker.ItemsSource = typeof(Colors).GetProperties();
        }

        public void createMaze()
        {
            maze = new Maze(numRows, numColumns, progress);
            this.Dispatcher.Invoke(() => {
                renderMaze();
            });
        }

        public void renderMaze()
        {
            if(maze == null)
            {
                return;
            }

            MazeRenderer renderer = new MazeRenderer(cellSize,selectedColor);
            renderer.renderToElement(maze, canvas);

        }

        private async void btnGenerateMaze_Click(object sender, RoutedEventArgs e)
        {
            generateButton.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;
            await Task.Run(() => createMaze());
            progressBar.Visibility = Visibility.Hidden;
            generateButton.IsEnabled = true;
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
            if (text.Text.Length == 0)
            {
                return;
            }
            numRows = Convert.ToInt32(text.Text.Replace(" ",""));
        }

        private void txtNumColumns_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if(text.Text.Length == 0)
            {
                return;
            }
            numColumns = Convert.ToInt32(text.Text.Replace(" ", ""));
        }

        private void sldSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            cellSize = Convert.ToInt32(slider.Value);
            if (cellSizeLabel != null)
            {
                cellSizeLabel.Content = "Cell size: " + cellSize;
            }
            renderMaze();
        }

        private void txtNumbersOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selectedColor = (Color)(colorPicker.SelectedItem as PropertyInfo).GetValue(null, null);
                renderMaze();
            }
            catch(NullReferenceException ex)
            {

            }
        }
    }
}
