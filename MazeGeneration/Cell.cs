using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGeneration
{
    public enum Direction
    {
        BLANK = 0,
        NORTH = 1,
        EAST = 2,
        SOUTH = 3,
        WEST = 4

    }
    public class Cell
    {
        public int row { get; }
        public int column { get; }
        public List<Direction> openSides = new List<Direction>();

        public Cell(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }
}
