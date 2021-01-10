﻿using System;
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
        public abstract void Paint(Graphics g, Pen p, GridCoordinates start, SimpleTransform transform, bool extraLines);

        /// <summary>
        /// Given a point on the grid, determine the grid point that this line segment would connect it to.
        /// </summary>
        public abstract GridCoordinates Target(GridCoordinates source);

        private Rectangle CalculateBoundingRectangle(Ellipse ellipse)
        {
            Size sizeOfBoundingRect = new Size((int)(2.0 * ellipse.XRadius), (int)(2.0 * ellipse.YRadius));
            Point startOfBoundingRect = new Point((int)(ellipse.XCenter - ellipse.XRadius), (int)(ellipse.YCenter - ellipse.YRadius));
            return new Rectangle(startOfBoundingRect, sizeOfBoundingRect);
        }

        protected void DrawArcs(Graphics g, Pen myPen, bool extraLines, Ellipse ellipse, float startAngle, float sweepAngle)
        {
            Rectangle boundingRect = CalculateBoundingRectangle(ellipse);
            g.DrawArc(myPen, boundingRect, startAngle, sweepAngle);

            if (extraLines)
            {
                Rectangle outerBoundingRect = CalculateBoundingRectangle(ellipse.increaseRadius(+4));
                g.DrawArc(myPen, outerBoundingRect, startAngle, sweepAngle);

                Rectangle innerBoundingRect = CalculateBoundingRectangle(ellipse.increaseRadius(-4));
                g.DrawArc(myPen, innerBoundingRect, startAngle, sweepAngle);
            }
        }
    }

    class DiagonalForwardUp : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            g.DrawLine(pen, xCoor, yCoor, xCoor + transform.XScale, yCoor - transform.YScale);
            if (extraLines)
            {
                g.DrawLine(pen, xCoor - 4, yCoor, xCoor + transform.XScale - 4, yCoor - transform.YScale);
                g.DrawLine(pen, xCoor + 4, yCoor, xCoor + transform.XScale + 4, yCoor - transform.YScale);
            }
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row - 1, Col = source.Col + 1 };
        }
    }

    class DiagonalForwardDown : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            g.DrawLine(pen, xCoor, yCoor, xCoor + transform.XScale, yCoor + transform.YScale);
            if (extraLines)
            {
                g.DrawLine(pen, xCoor - 4, yCoor, xCoor + transform.XScale - 4, yCoor + transform.YScale);
                g.DrawLine(pen, xCoor + 4, yCoor, xCoor + transform.XScale + 4, yCoor + transform.YScale);
            }
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 1, Col = source.Col + 1 };
        }
    }

    class DiagonalBackwardUp : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            g.DrawLine(pen, xCoor, yCoor, xCoor - transform.XScale, yCoor - transform.YScale);
            if (extraLines)
            {
                g.DrawLine(pen, xCoor - 4, yCoor, xCoor - transform.XScale - 4, yCoor - transform.YScale);
                g.DrawLine(pen, xCoor + 4, yCoor, xCoor - transform.XScale + 4, yCoor - transform.YScale);
            }
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row - 1, Col = source.Col - 1 };
        }
    }

    class DiagonalBackwardDown : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            g.DrawLine(pen, xCoor, yCoor, xCoor - transform.XScale, yCoor + transform.YScale);
            if (extraLines)
            {
                g.DrawLine(pen, xCoor - 4, yCoor, xCoor - transform.XScale - 4, yCoor + transform.YScale);
                g.DrawLine(pen, xCoor + 4, yCoor, xCoor - transform.XScale + 4, yCoor + transform.YScale);
            }
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 1, Col = source.Col - 1 };
        }
    }

    class DownwardArcingLeft : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            double xRadius = transform.XScale / (0.5 * Math.Sqrt(2));
            double yRadius = transform.YScale / (0.5 * Math.Sqrt(2));

            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            double xCenterOfRect = xCoor + transform.XScale;
            double yCenterOfRect = yCoor + transform.YScale;

            Ellipse ellipse = new Ellipse(xCenterOfRect, yCenterOfRect, xRadius, yRadius);

            DrawArcs(g, pen, extraLines, ellipse, 135, 90);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 2, Col = source.Col };
        }
    }

    class DownwardArcingRight : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            double xRadius = transform.XScale / (0.5 * Math.Sqrt(2));
            double yRadius = transform.YScale / (0.5 * Math.Sqrt(2));

            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            double xCenterOfRect = xCoor - transform.XScale;
            double yCenterOfRect = yCoor + transform.YScale;

            Ellipse ellipse = new Ellipse(xCenterOfRect, yCenterOfRect, xRadius, yRadius);

            DrawArcs(g, pen, extraLines, ellipse, -45, 90);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row + 2, Col = source.Col };
        }
    }

    class ForwardArcingUp : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            double xRadius = transform.XScale / (0.5 * Math.Sqrt(2));
            double yRadius = transform.YScale / (0.5 * Math.Sqrt(2));

            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            double xCenterOfRect = xCoor + transform.XScale;
            double yCenterOfRect = yCoor + transform.YScale;

            Ellipse ellipse = new Ellipse(xCenterOfRect, yCenterOfRect, xRadius, yRadius);

            DrawArcs(g, pen, extraLines, ellipse, 225, 90);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row, Col = source.Col + 2 };
        }
    }

    class ForwardArcingDown : LineSegment
    {
        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, bool extraLines)
        {
            double xRadius = transform.XScale / (0.5 * Math.Sqrt(2));
            double yRadius = transform.YScale / (0.5 * Math.Sqrt(2));

            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            double xCenterOfRect = xCoor + transform.XScale;
            double yCenterOfRect = yCoor - transform.YScale;

            Ellipse ellipse = new Ellipse(xCenterOfRect, yCenterOfRect, xRadius, yRadius);

            DrawArcs(g, pen, extraLines, ellipse, 45, 90);
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row, Col = source.Col + 2 };
        }
    }
}
