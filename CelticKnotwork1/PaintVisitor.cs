using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CelticKnotwork1
{
    class PaintVisitor : LineSegmentVisitor
    {
        private Graphics g;
        private Pen pen;
        private GridCoordinates start;
        private SimpleTransform transform;
        private double? extraLines;

        public PaintVisitor(Graphics g, Pen pen, GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            this.g = g;
            this.pen = pen;
            this.start = start;
            this.transform = transform;
            this.extraLines = extraLines;
        }

        private static void DrawQuarterCircle(Graphics g, Pen pen, SimpleTransform transform, GridCoordinates p0, double startRadians, double? extraLines)
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

        public override void VisitDiagonalForwardDown(DiagonalForwardDown l)
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

        public override void VisitDiagonalBackwardDown(DiagonalBackwardDown l)
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

        public override void VisitVerticalArcingLeft(VerticalArcingLeft l)
        {
            GridCoordinates newStart = new GridCoordinates { Col = start.Col + 1, Row = start.Row + 1 };
            DrawQuarterCircle(g, pen, transform, newStart, 0.75, extraLines);
        }

        public override void VisitVerticalArcingRight(VerticalArcingRight l)
        {
            GridCoordinates newStart = new GridCoordinates { Col = start.Col - 1, Row = start.Row + 1 };
            DrawQuarterCircle(g, pen, transform, newStart, 1.75, extraLines);
        }

        public override void VisitHorizontalArcingUp(HorizontalArcingUp l)
        {
            DrawQuarterCircle(g, pen, transform, start, 1.25, extraLines);
        }

        public override void VisitHorizontalArcingDown(HorizontalArcingDown l)
        {
            DrawQuarterCircle(g, pen, transform, start, 0.25, extraLines);
        }
    }
}
