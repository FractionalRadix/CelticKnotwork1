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
            LineSegment.DrawQuarterCircle(g, pen, transform, newStart, 0.75, extraLines);
        }

        public override void VisitVerticalArcingRight(VerticalArcingRight l)
        {
            GridCoordinates newStart = new GridCoordinates { Col = start.Col - 1, Row = start.Row + 1 };
            LineSegment.DrawQuarterCircle(g, pen, transform, newStart, 1.75, extraLines);
        }

        public override void VisitHorizontalArcingUp(HorizontalArcingUp l)
        {
            LineSegment.DrawQuarterCircle(g, pen, transform, start, 1.25, extraLines);
        }

        public override void VisitHorizontalArcingDown(HorizontalArcingDown l)
        {
            LineSegment.DrawQuarterCircle(g, pen, transform, start, 0.25, extraLines);
        }
    }
}
