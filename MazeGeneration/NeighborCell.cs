using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGeneration
{
    public class NeighborCell
    {
        public Cell cell { get; }
        public Direction openSide { get; }
        public Direction direction { get; }

        public NeighborCell(Cell cell, Direction direction, Direction openSide)
        {
            this.cell = cell;
            this.openSide = openSide;
            this.direction = direction;
        }
    }
}
