using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CelticKnotwork1
{
    class SvgGeneratingVisitor : LineSegmentVisitor
    {
        private readonly GridCoordinates start;
        private readonly SimpleTransform transform;
        private readonly double? extraLines;
        private String result;

        public SvgGeneratingVisitor(GridCoordinates start, SimpleTransform transform, double? extraLines)
        {
            this.start = start;
            this.transform = transform;
            this.extraLines = extraLines;
        }

        public String GetResult() { return result; }

        public override void VisitDiagonalForwardDown(DiagonalForwardDown l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            result = $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\"/>";

            // Optional doubling of lines.
            if (extraLines != null)
            {
                PointF upperLeft = transform.Apply(start.Col - (float)extraLines.Value, start.Row);
                PointF lowerLeft = transform.Apply(target.Col - (float)extraLines.Value, target.Row);
                String leftLine = $"<line x1=\"{upperLeft.X}\" y1=\"{upperLeft.Y}\" x2=\"{lowerLeft.X}\" y2=\"{lowerLeft.Y}\"/>";
                result += leftLine;

                PointF upperRight = transform.Apply(start.Col + (float)extraLines.Value, start.Row);
                PointF lowerRight = transform.Apply(target.Col + (float)extraLines.Value, target.Row);
                String rightLine = $"<line x1=\"{upperRight.X}\" y1=\"{upperRight.Y}\" x2=\"{lowerRight.X}\" y2=\"{lowerRight.Y}\"/>";
                result += rightLine;
            }
        }

        public override void VisitDiagonalBackwardDown(DiagonalBackwardDown l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            result = $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\"/>";

            // Optional doubling of lines.
            if (extraLines != null)
            {
                PointF upperLeft = transform.Apply(start.Col - (float)extraLines.Value, start.Row);
                PointF lowerLeft = transform.Apply(target.Col - (float)extraLines.Value, target.Row);
                String leftLine = $"<line x1=\"{upperLeft.X}\" y1=\"{upperLeft.Y}\" x2=\"{lowerLeft.X}\" y2=\"{lowerLeft.Y}\"/>";
                result += leftLine;

                PointF upperRight = transform.Apply(start.Col + (float)extraLines.Value, start.Row);
                PointF lowerRight = transform.Apply(target.Col + (float)extraLines.Value, target.Row);
                String rightLine = $"<line x1=\"{upperRight.X}\" y1=\"{upperRight.Y}\" x2=\"{lowerRight.X}\" y2=\"{lowerRight.Y}\"/>";
                result += rightLine;
            }
        }

        public override void VisitVerticalArcingLeft(VerticalArcingLeft l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            int len = Math.Abs(p2.Y - p1.Y);
            Point control = new Point((int)(p1.X - 0.5 * len), (int)(p1.Y + 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\"/>";

            if (extraLines != null)
            {
                GridCoordinates upper, lower;
                if (start.Row > target.Row)
                {
                    upper = target; 
                    lower = start;
                }
                else
                { 
                    upper = start; 
                    lower = target; 
                }

                PointF p1outer = transform.Apply(upper.Col - (float)extraLines.Value, upper.Row - (float)extraLines.Value);
                PointF p2outer = transform.Apply(lower.Col - (float)extraLines.Value, lower.Row + (float)extraLines.Value);
                PointF controlOuter = new PointF(control.X - transform.XScale * (float) extraLines.Value, control.Y);
                String res2 = $"<path d=\"M {p1outer.X} {p1outer.Y} Q {controlOuter.X} {controlOuter.Y} {p2outer.X} {p2outer.Y}\"/>";

                result += res2;

                PointF p1inner = transform.Apply(upper.Col + (float)extraLines.Value, upper.Row + (float)extraLines.Value);
                PointF p2inner = transform.Apply(lower.Col + (float)extraLines.Value, lower.Row - (float)extraLines.Value);
                PointF controlInner = new PointF(control.X + transform.XScale * (float)extraLines.Value, control.Y);
                String res3 = $"<path d=\"M {p1inner.X} {p1inner.Y} Q {controlInner.X} {controlInner.Y} {p2inner.X} {p2inner.Y}\"/>";

                result += res3;
            }
        }

        public override void VisitVerticalArcingRight(VerticalArcingRight l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            int len = Math.Abs(p2.Y - p1.Y);
            Point control = new Point((int)(p1.X + 0.5 * len), (int)(p1.Y + 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\"/>";

            if (extraLines != null)
            {
                GridCoordinates upper, lower;
                if (start.Row > target.Row)
                {
                    upper = target;
                    lower = start;
                }
                else
                {
                    upper = start;
                    lower = target;
                }

                
                PointF p1outer = transform.Apply(upper.Col + (float)extraLines.Value, upper.Row - (float)extraLines.Value);
                PointF p2outer = transform.Apply(lower.Col + (float)extraLines.Value, lower.Row + (float)extraLines.Value);
                PointF controlOuter = new PointF(control.X + transform.XScale * (float)extraLines.Value, control.Y);
                String res2 = $"<path d=\"M {p1outer.X} {p1outer.Y} Q {controlOuter.X} {controlOuter.Y} {p2outer.X} {p2outer.Y}\"/>";

                result += res2;

                PointF p1inner = transform.Apply(upper.Col - (float)extraLines.Value, upper.Row + (float)extraLines.Value);
                PointF p2inner = transform.Apply(lower.Col - (float)extraLines.Value, lower.Row - (float)extraLines.Value);
                PointF controlInner = new PointF(control.X - transform.XScale * (float)extraLines.Value, control.Y);
                String res3 = $"<path d=\"M {p1inner.X} {p1inner.Y} Q {controlInner.X} {controlInner.Y} {p2inner.X} {p2inner.Y}\"/>";

                result += res3;
            }
        }

        public override void VisitHorizontalArcingUp(HorizontalArcingUp l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col - 1, start.Row - 1);
            Point p2 = transform.Apply(target.Col - 1, target.Row - 1);
            int len = Math.Abs(p2.X - p1.X);
            Point control = new Point((int)(p1.X + 0.5 * len), (int)(p1.Y - 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\"/>";

            if (extraLines != null)
            {
                GridCoordinates leftmost, rightmost;
                if (start.Col > target.Col)
                {
                    leftmost = target;
                    rightmost = start;
                }
                else
                {
                    leftmost = start;
                    rightmost = target;
                }

                // Generate SVG for the extra arc above
                PointF upperLeft = transform.Apply(leftmost.Col - 1 - extraLines.Value, leftmost.Row - 1 - extraLines.Value);
                PointF upperRight = transform.Apply(rightmost.Col - 1 + extraLines.Value, rightmost.Row - 1 - extraLines.Value);
                PointF controlUpper = new PointF(control.X, control.Y - transform.YScale * (float) extraLines.Value);
                String resUpper = $"<path d=\"M {upperLeft.X} {upperLeft.Y} Q {controlUpper.X} {controlUpper.Y} {upperRight.X} {upperRight.Y}\"/>";
                result += resUpper;

                // Generate SVG for the extra arc below
                PointF lowerLeft = transform.Apply(leftmost.Col - 1 + extraLines.Value, leftmost.Row - 1 + extraLines.Value);
                PointF lowerRight = transform.Apply(rightmost.Col - 1 - extraLines.Value, rightmost.Row - 1 + extraLines.Value);
                PointF controlLower = new PointF(control.X, control.Y + transform.YScale * (float)extraLines.Value);
                String resLower = $"<path d=\"M {lowerLeft.X} {lowerLeft.Y} Q {controlLower.X} {controlLower.Y} {lowerRight.X} {lowerRight.Y}\"/>";
                result += resLower;
            }
        }

        public override void VisitHorizontalArcingDown(HorizontalArcingDown l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col - 1, start.Row + 1);
            Point p2 = transform.Apply(target.Col - 1, target.Row + 1);
            int len = Math.Abs(p2.X - p1.X);
            Point control = new Point((int)(p1.X + 0.5 * len), (int)(p1.Y + 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\"/>";

            if (extraLines != null)
            {
                GridCoordinates leftmost, rightmost;
                if (start.Col > target.Col)
                {
                    leftmost = target;
                    rightmost = start;
                }
                else
                {
                    leftmost = start;
                    rightmost = target;
                }

                // Generate SVG for the extra arc below
                PointF lowerLeft = transform.Apply(leftmost.Col - 1 - extraLines.Value, leftmost.Row + 1 + extraLines.Value);
                PointF lowerRight = transform.Apply(rightmost.Col - 1 + extraLines.Value, rightmost.Row + 1 + extraLines.Value);
                PointF controlLower = new PointF(control.X, control.Y + transform.YScale * (float) extraLines.Value);
                String resLower = $"<path d=\"M {lowerLeft.X} {lowerLeft.Y} Q {controlLower.X} {controlLower.Y} {lowerRight.X} {lowerRight.Y}\"/>";
                result += resLower;

                // Generate SVG for the extra arc above
                PointF upperLeft = transform.Apply(leftmost.Col - 1 + extraLines.Value, leftmost.Row + 1 - extraLines.Value);
                PointF upperRight = transform.Apply(rightmost.Col - 1 - extraLines.Value, rightmost.Row + 1 - extraLines.Value);
                PointF controlUpper = new PointF(control.X, control.Y - transform.YOffset * (float)extraLines.Value);
                String resUpper = $"<path d=\"M {upperLeft.X} {upperLeft.Y} Q {controlUpper.X} {controlUpper.Y} {upperRight.X} {upperRight.Y}\"/>";
                result += resUpper;
            }
        }

    }
}
