using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CelticKnotwork1
{
    /// <summary>
    /// A connection between two points on the grid.
    /// Despite the name, this is not always a line segment; it can also be an arc.
    /// </summary>
    abstract class LineSegment
    {
        public abstract void Accept(LineSegmentVisitor v);

        /// <summary>
        /// Given a point on the grid, determine the grid point that this line segment would connect it to.
        /// </summary>
        public abstract GridCoordinates Target(GridCoordinates source);

        public abstract Direction IngoingDirection();
        public abstract Direction OutgoingDirection();
    }

    class DiagonalForwardDown : LineSegment
    {
        public override Direction IngoingDirection()
        {
            return Direction.NorthWest; // Anything that goes in, comes in from the NorthWest.
        }

        public override Direction OutgoingDirection()
        {
            return Direction.SouthEast; // Anything that goes out, goes to the SouthEast.
        }

        public override void Accept(LineSegmentVisitor v)
        {
            v.VisitDiagonalForwardDown(this);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 1, Col = source.Col + 1 };
        }
    }

    class DiagonalBackwardDown : LineSegment
    {
        public override Direction IngoingDirection()
        {
            return Direction.NorthEast;
        }

        public override Direction OutgoingDirection()
        {
            return Direction.SouthWest;
        }

        public override void Accept(LineSegmentVisitor v)
        {
            v.VisitDiagonalBackwardDown(this);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 1, Col = source.Col - 1 };
        }
    }

    class VerticalArcingLeft : LineSegment
    {
        public override Direction IngoingDirection()
        {
            return Direction.SouthEast;
        }

        public override Direction OutgoingDirection()
        {
            return Direction.NorthEast;
        }

        public override void Accept(LineSegmentVisitor v)
        {
            v.VisitVerticalArcingLeft(this);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 2, Col = source.Col };
        }
    }

    class VerticalArcingRight : LineSegment
    {
        public override Direction IngoingDirection()
        {
            return Direction.NorthWest;
        }

        public override Direction OutgoingDirection()
        {
            return Direction.SouthWest;
        }

        public override void Accept(LineSegmentVisitor v)
        {
            v.VisitVerticalArcingRight(this);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 2, Col = source.Col };
        }
    }

    class HorizontalArcingUp : LineSegment
    {
        public override Direction IngoingDirection()
        {
            return Direction.SouthWest;
        }

        public override Direction OutgoingDirection()
        {
            return Direction.SouthEast;
        }

        public override void Accept(LineSegmentVisitor v)
        {
            v.VisitHorizontalArcingUp(this);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row, Col = source.Col + 2 };
        }
    }

    class HorizontalArcingDown : LineSegment
    {
        public override Direction IngoingDirection()
        {
            return Direction.NorthWest;
        }

        public override Direction OutgoingDirection()
        {
            return Direction.NorthEast;
        }

        public override void Accept(LineSegmentVisitor v)
        {
            v.VisitHorizontalArcingDown(this);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row, Col = source.Col + 2 };
        }
    }
}