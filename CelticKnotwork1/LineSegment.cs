﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CelticKnotwork1
{
    enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };

    /// <summary>
    /// A connection between two points on the grid.
    /// Despite the name, this is not always a line segment; it can also be an arc.
    /// </summary>
    abstract class LineSegment
    {
        public abstract void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines);

        public abstract void Paint2(Graphics g, Pen pen,  GridCoordinates start, SimpleTransform transform, double? extraLines);

        public abstract String generateSVG(GridCoordinates start, SimpleTransform transform);

        /// <summary>
        /// Given a point on the grid, determine the grid point that this line segment would connect it to.
        /// </summary>
        public abstract GridCoordinates Target(GridCoordinates source);

        private static Rectangle CalculateBoundingRectangle(Ellipse ellipse)
        {
            Size sizeOfBoundingRect = new Size((int)(2.0 * ellipse.XRadius), (int)(2.0 * ellipse.YRadius));
            Point startOfBoundingRect = new Point((int)(ellipse.XCenter - ellipse.XRadius), (int)(ellipse.YCenter - ellipse.YRadius));
            return new Rectangle(startOfBoundingRect, sizeOfBoundingRect);
        }

        protected static void DrawArcs(Graphics g, Pen myPen, double? extraLines, Ellipse ellipse, float startAngle, float sweepAngle)
        {
            Rectangle boundingRect = CalculateBoundingRectangle(ellipse);
            g.DrawArc(myPen, boundingRect, startAngle, sweepAngle);

            if (extraLines != null)
            {
                Rectangle outerBoundingRect = CalculateBoundingRectangle(ellipse.increaseRadius(+4.0 /* +extraLines.Value */ ));
                g.DrawArc(myPen, outerBoundingRect, startAngle, sweepAngle);

                Rectangle innerBoundingRect = CalculateBoundingRectangle(ellipse.increaseRadius(-4.0 /* -extraLines.Value */));
                g.DrawArc(myPen, innerBoundingRect, startAngle, sweepAngle);
            }
        }

        public abstract Direction IngoingDirection();
        public abstract Direction OutgoingDirection();

        protected static void DrawQuarterCircle(Graphics g, Pen pen, SimpleTransform transform, GridCoordinates p0, double startRadians, double? extraLines)
        {
            //double startRadians = 1.75; // Vertical arc, arcing towards the right.
            //double startRadians = 1.25; // Horizontal arc, arcing upward.
            //double startRadians = 0.75; // Vertical arc, arcing towards the left.
            //double startRadians = 0.25; // Horizontal arc, arcing downward.

            Point? d0 = null;
            Point? d0Inner = null, d0Outer = null;

            Point d1;
            Point d1Inner = new Point(0, 0), d1Outer = new Point(0, 0);

            double radius = Math.Sqrt(2);
            double innerRadius = 0.0, outerRadius = 0.0;


            if (extraLines != null)
            {
                innerRadius = radius - extraLines.Value;
                outerRadius = radius + extraLines.Value;
            }

            for (double t = 0.0; t <= 1.0; t += 0.1)
            {
                double angle = (startRadians + 0.5 * t) * Math.PI;
                double ca = Math.Cos(angle);
                double sa = Math.Sin(angle);

                double x1 = p0.Col + radius * ca;
                double y1 = p0.Row + radius * sa;
                d1 = transform.Apply(x1, y1);

                if (extraLines != null)
                {
                    double x1Inner = p0.Col + innerRadius * ca;
                    double y1Inner = p0.Row + innerRadius * sa;
                    d1Inner = transform.Apply(x1Inner, y1Inner);

                    double x1Outer = p0.Col + outerRadius * ca;
                    double y1Outer = p0.Row + outerRadius * sa;
                    d1Outer = transform.Apply(x1Outer, y1Outer);
                }

                if (d0 != null)
                {
                    g.DrawLine(pen, d0.Value, d1);
                    if (extraLines != null)
                    {
                        g.DrawLine(pen, d0Inner.Value, d1Inner);
                        g.DrawLine(pen, d0Outer.Value, d1Outer);
                    }
                }
                d0 = d1;
                d0Inner = d1Inner;
                d0Outer = d1Outer;
            }
        }
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

        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;
            Point p0 = transform.Apply(start.Col, start.Row);
            Point p1 = transform.Apply(start.Col + 1, start.Row + 1);

            //g.DrawLine(pen, xCoor, yCoor, xCoor + transform.XScale, yCoor + transform.YScale);
            g.DrawLine(pen, p0, p1);
            if (extraLines != null)
            {
                g.DrawLine(pen, xCoor - 4, yCoor, xCoor + transform.XScale - 4, yCoor + transform.YScale);
                //PointF p0Left = new PointF(p0.X - (float) extraLines.Value, p0.Y);
                //PointF p1Left = new PointF(p1.X - (float) extraLines.Value, p1.Y);
                //g.DrawLine(pen, p0Left, p1Left);


                g.DrawLine(pen, xCoor + 4, yCoor, xCoor + transform.XScale + 4, yCoor + transform.YScale);
                //PointF p0Right = new PointF(p0.X + (float)extraLines.Value, p0.Y);
                //PointF p1Right = new PointF(p1.X + (float)extraLines.Value, p1.Y);
                //g.DrawLine(pen, p0Right, p1Right);

            }
        }

        public override void Paint2(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            Point? d0 = null;
            Point? d0Left = null, d0Right = null;

            Point d1;
            Point d1Left = new Point(0, 0), d1Right = new Point(0, 0);

            for (double t = 0.0; t <= 1.0; t += 0.1)
            {
                double x1 = start.Col + t;
                double y1 = start.Row + t;
                d1 = transform.Apply(x1, y1);

                if (extraLines != null)
                {
                    double x1Left = start.Col + t - extraLines.Value * Math.Sqrt(2);
                    double y1Left = start.Row + t;
                    d1Left = transform.Apply(x1Left, y1Left);

                    double x1Right = start.Col + t + extraLines.Value * Math.Sqrt(2);
                    double y1Right = start.Row + t;
                    d1Right = transform.Apply(x1Right, y1Right);
                }

                if (d0 != null)
                {
                    g.DrawLine(pen, d0.Value, d1);

                    if (extraLines != null)
                    {
                        g.DrawLine(pen, d0Left.Value, d1Left);
                        g.DrawLine(pen, d0Right.Value, d1Right);
                    }
                }

                d0 = d1;
                if (extraLines != null)
                {
                    d0Left = d1Left;
                    d0Right = d1Right;
                }
            }

        }

        public override String generateSVG(GridCoordinates start, SimpleTransform transform)
        {
            //return $"<line x1=\"{start.Col}\" y1=\"{start.Row}\" x2=\"{start.Col+1}\" y2=\"{start.Row+1}\" stroke=\"black\"/>";

            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(Target(start).Col, Target(start).Row);
            return $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\" stroke=\"black\"/>";
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

        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            int xCoor = transform.XOffset + transform.XScale * start.Col;
            int yCoor = transform.YOffset + transform.YScale * start.Row;

            g.DrawLine(pen, xCoor, yCoor, xCoor - transform.XScale, yCoor + transform.YScale);
            if (extraLines != null)
            {
                g.DrawLine(pen, xCoor - 4, yCoor, xCoor - transform.XScale - 4, yCoor + transform.YScale);
                g.DrawLine(pen, xCoor + 4, yCoor, xCoor - transform.XScale + 4, yCoor + transform.YScale);
            }
        }

        public override void Paint2(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            Point? d0 = null;
            Point? d0Left = null, d0Right = null;

            Point d1;
            Point d1Left = new Point(0, 0), d1Right = new Point(0, 0);

            for (double t = 0.0; t <= 1.0; t += 0.1)
            {
                double x1 = start.Col - t;
                double y1 = start.Row + t;
                d1 = transform.Apply(x1, y1);

                if (extraLines != null)
                {
                    double x1Left = start.Col - t - extraLines.Value * Math.Sqrt(2);
                    double y1Left = start.Row + t;
                    d1Left = transform.Apply(x1Left, y1Left);

                    double x1Right = start.Col - t + extraLines.Value * Math.Sqrt(2);
                    double y1Right = start.Row + t;
                    d1Right = transform.Apply(x1Right, y1Right);
                }

                if (d0 != null)
                {
                    g.DrawLine(pen, d0.Value, d1);
                    if (extraLines != null)
                    {
                        g.DrawLine(pen, d0Left.Value, d1Left);
                        g.DrawLine(pen, d0Right.Value, d1Right);
                    }
                }

                d0 = d1;
                if (extraLines != null)
                {
                    d0Left = d1Left;
                    d0Right = d1Right;
                }
            }
        }

        public override String generateSVG(GridCoordinates start, SimpleTransform transform)
        {
            //return $"<line x1=\"{start.Col}\" y1=\"{start.Row}\" x2=\"{start.Col - 1}\" y2=\"{start.Row + 1}\" stroke=\"black\"/>";

            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(Target(start).Col, Target(start).Row);
            return $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\" stroke=\"black\"/>";

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

        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
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

        public override void Paint2(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            GridCoordinates newStart = new GridCoordinates { Col = start.Col + 1, Row = start.Row + 1 };
            DrawQuarterCircle(g, pen, transform, newStart, 0.75, extraLines);
        }

        public override String generateSVG(GridCoordinates start, SimpleTransform transform)
        {
            return null; //TODO!~
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

        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
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

        public override void Paint2(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            GridCoordinates newStart = new GridCoordinates { Col = start.Col - 1, Row = start.Row + 1 };
            DrawQuarterCircle(g, pen, transform, newStart, 1.75, extraLines);
        }

        public override String generateSVG(GridCoordinates start, SimpleTransform transform)
        {
            return null; //TODO!~
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

        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
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

        public override void Paint2(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            DrawQuarterCircle(g, pen, transform, start, 1.25, extraLines);
        }

        public override String generateSVG(GridCoordinates start, SimpleTransform transform)
        {
            return null; //TODO!~
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

        public override void Paint(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
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

        public override void Paint2(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            DrawQuarterCircle(g, pen, transform, start, 0.25, extraLines);
        }

        public override String generateSVG(GridCoordinates start, SimpleTransform transform)
        {
            return null; //TODO!~
        }

        public override GridCoordinates Target(GridCoordinates source)
        {
            return new GridCoordinates { Row = source.Row, Col = source.Col + 2 };
        }
    }
}