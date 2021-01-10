using System;
using System.Collections.Generic;
using System.Text;

namespace CelticKnotwork1
{
    class Knotwork
    {
        private class GridPoint
        {
            private readonly List<LineStyle> Lines = new List<LineStyle>();

            public void Add(LineStyle line)
            {
                Lines.Add(line);
            }

            public IEnumerable<LineStyle> Get()
            {
                return Lines;
            }
        }

        public int Rows { get; private set; }
        public int Cols { get; private set; }
        GridPoint[,] gridPoints;

        public Knotwork(int rows, int cols)
        {
            this.Rows = rows;
            this.Cols = cols;

            gridPoints = new GridPoint[Rows, Cols];
            for (int col = 0; col < Cols; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    gridPoints[row, col] = new GridPoint();
                }
            }
        }

        public void AddLine(int row, int col, LineStyle line)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Cols)
            {
                return; //TODO?+ Issue a warning?
            }
            gridPoints[row, col].Add(line);
        }

        /// <summary>
        /// Given a point on the grid, find all the lines connected to that point - whether outgoing or incoming.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public IEnumerable<LineStyle> GetAllLines(GridCoordinates p)
        {
            var res = new List<LineStyle>();

            // As an optimization, we don't search the entire knotwork.
            // Since every line spans at most 2 points, we look only at the points that are at most 2 points away from (row,col).
            int minRow = Math.Max(0, p.Row - 2);
            int maxRow = Math.Min(Rows - 1, p.Row + 2);
            int minCol = Math.Max(0, p.Col - 2);
            int maxCol = Math.Min(Cols - 1, p.Col + 2);

            for (int currentRow = minRow; currentRow < maxRow; currentRow++)
            {
                for (int currentCol = minCol; currentCol < maxCol; currentCol++)
                {
                    if (currentRow == p.Row && currentCol == p.Col)
                    {
                        // This is the grid point we came from: (row,col).
                        // So all lines defined here are outgoing, and are relevant.
                        // Add them all to the result.
                        var iter = this.gridPoints[p.Row, p.Col].Get();
                        foreach (LineStyle currentLine in iter)
                        {
                            res.Add(currentLine);
                        }
                    }
                    else
                    {
                        // This is one of the neighbouring grid points.
                        // Check if it has any lines connecting to (row,col).
                        // If so, add them to the result.
                        var iter = this.gridPoints[p.Row, p.Col].Get().GetEnumerator();
                        while (iter.MoveNext())
                        {
                            LineStyle currentLine = iter.Current;
                            Tuple<int, int> target = Target(currentLine, currentRow, currentCol);

                            if (target.Item1 == p.Row && target.Item2 == p.Col)
                            {
                                res.Add(currentLine);
                            }
                        }
                    }
                }
            }

            return res;
        }

        public IEnumerable<LineStyle> GetOutgoingLines(int row, int col)
        {
            var lines = this.gridPoints[row, col].Get();
            return lines;
        }

        Tuple<int,int> Target(LineStyle line, int row, int col)
        {
            Tuple<int, int> res;
            switch (line)
            {
                case LineStyle.diagonalForwardUp:
                    res = new Tuple<int,int>(row - 1, col + 1);
                    break;
                case LineStyle.diagonalForwardDown:
                    res = new Tuple<int, int>(row + 1, col + 1);
                    break;
                case LineStyle.diagonalBackwardUp:
                    res = new Tuple<int, int>(row - 1, col - 1);
                    break;
                case LineStyle.diagonalBackwardDown:
                    res = new Tuple<int, int>(row + 1, col - 1);
                    break;
                case LineStyle.roundedDownArcingLeft:
                    res = new Tuple<int, int>(row + 2, col);
                    break;
                case LineStyle.roundedDownArcingRight:
                    res = new Tuple<int, int>(row + 2, col);
                    break;
                case LineStyle.roundedForwardArcingUp:
                    res = new Tuple<int, int>(row - 2, col);
                    break;
                case LineStyle.roundedForwardArcingDown:
                    res = new Tuple<int, int>(row - 2, col);
                    break;
                default:
                    res = null;
                    break;
            }
            return res;
        }
    }

}
