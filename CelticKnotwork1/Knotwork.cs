using System;
using System.Collections.Generic;
using System.Text;

namespace CelticKnotwork1
{
    class Knotwork
    {
        private class GridPoint
        {
            private readonly List<LineSegment> Lines = new List<LineSegment>();

            public void Add(LineSegment line)
            {
                Lines.Add(line);
            }

            public IEnumerable<LineSegment> Get()
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

        public void AddLine(int row, int col, LineSegment line)
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
        /// <param name="p">Coordinates of a point in the grid.</param>
        /// <returns>A list of line segments that start or end in the given point.</returns>
        public IEnumerable<LineSegment> GetAllLines(GridCoordinates p)
        {
            var res = new List<LineSegment>();

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
                        foreach (LineSegment currentLine in iter)
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
                            LineSegment currentLine = iter.Current;
                            GridCoordinates target = currentLine.Target(new GridCoordinates { Row = currentRow, Col = currentCol });
                            if (target.Row == p.Row && target.Col == p.Col)
                            {
                                res.Add(currentLine);
                            }
                        }
                    }
                }
            }

            return res;
        }

        public IEnumerable<LineSegment> GetOutgoingLines(int row, int col)
        {
            var lines = this.gridPoints[row, col].Get();
            return lines;
        }
    }
}
