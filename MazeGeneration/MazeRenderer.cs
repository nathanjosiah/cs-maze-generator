using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MazeGeneration
{
    class MazeRenderer
    {
        private int size = 10;

        public MazeRenderer(int size)
        {
            this.size = size;
        }

        private Line createLine()
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            return myLine;
        }

        public void renderToElement(Maze maze, Panel canvas)
        {
            canvas.Children.Clear();
            foreach (List<Cell> row in maze.getCells())
            {
                foreach (Cell cell in row)
                {
                    renderCell(cell, canvas);
                }
            }
        }
        private void renderCell(Cell cell,Panel canvas)
        {
            Line myLine;

            /*
            if(cell.openSides.Count() > 2)
            {
                System.Windows.Shapes.Rectangle rect = new Rectangle();
                rect.Fill = System.Windows.Media.Brushes.HotPink;
                Canvas.SetLeft(rect,cell.column * size);
                Canvas.SetTop(rect,cell.row * size);
                rect.Width = size;
                rect.Height = size;
                canvas.Children.Add(rect);
            }
            */

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
}
