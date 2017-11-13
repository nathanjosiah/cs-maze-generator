using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGeneration
{
    public class Maze
    {
        private int numRows, numColumns;
        private List<List<Cell>> cells;
        private Random random = new Random();
        private Cell deepestCell;
        private int deepestCellCount = 0;
        private int currentCount = 0;

        private IProgress<int> progress;

        public Maze(int rows, int columns,IProgress<int> progress)
        {
            numRows = rows;
            numColumns = columns;
            this.progress = progress;

            this.cells = this.generateGrid(rows, columns);

            makeMaze(0, 0, new List<Cell>());

            if(deepestCell != null)
            {
                markOpenCell(deepestCell);
            }
        }

        private void makeMaze(int row, int column, List<Cell> visited)
        {
            if(cells.Count() == 0 || cells[row].Count() == 0)
            {
                return;
            }
            Cell currentCell = cells[row][column];

            if (visited.Count() == 0)
            {

                markOpenCell(currentCell);
            }

            List<NeighborCell> neighbors = getSurroundingCells(row, column, cells);

            visited.Add(cells[row][column]);
            progress.Report((int)((float)(visited.Count() / (float)(numRows * numColumns)) * 100));

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
                    if (deepestCellCount < currentCount && isExternalCell(currentCell))
                    {
                        deepestCellCount = currentCount;
                        deepestCell = currentCell;
                    }
                    if(currentCell.openSides.Count() > 2)
                    {
                        currentCount -= 1;
                    }
                    break;
                }

                int index = random.Next(0, unvisited.Count());
                NeighborCell cell = unvisited[index];
                Cell neighbor_cell = cell.cell;
                neighbor_cell.openSides.Add(cell.openSide);
                currentCell.openSides.Add(cell.direction);

                if (currentCell.openSides.Count() > 2)
                {
                    currentCount += 1;
                }
                this.makeMaze(cell.cell.row, cell.cell.column, visited);
            }

        }

        public List<List<Cell>> generateGrid(int rows, int columns)
        {
            List<List<Cell>> cells = new List<List<Cell>>();
            for (int i = 0; i < rows; i++)
            {
                List<Cell> row = new List<Cell>();

                for (int j = 0; j < columns; j++)
                {
                    Cell cell = new Cell(i, j);
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
                surrounding.Add(new NeighborCell(cells[row - 1][column], Direction.NORTH, Direction.SOUTH));
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
}
