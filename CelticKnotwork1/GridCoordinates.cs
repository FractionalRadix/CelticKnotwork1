using System;
using System.Collections.Generic;
using System.Text;

namespace CelticKnotwork1
{
    public class GridCoordinates
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public override bool Equals(object obj)
        {
            return obj is GridCoordinates coordinates &&
                   Row == coordinates.Row &&
                   Col == coordinates.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }
    }
}
