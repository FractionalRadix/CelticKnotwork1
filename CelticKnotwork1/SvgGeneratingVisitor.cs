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
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(l.Target(start).Col, l.Target(start).Row);
            result = $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\"/>";

            //TODO!+ Add optional doubling of lines.
        }

        public override void VisitDiagonalBackwardDown(DiagonalBackwardDown l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            result = $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\"/>";

            //TODO!+ Add optional doubling of lines.
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

                //TODO!+ Generate SVG for the extra arc above
                PointF upperLeft = transform.Apply(leftmost.Col - extraLines.Value, leftmost.Row - extraLines.Value);
                PointF upperRight = transform.Apply(rightmost.Col + extraLines.Value, rightmost.Row - extraLines.Value);
                PointF controlUpper = new PointF(control.X, control.Y - transform.YScale * (float) extraLines.Value);
                String resUpper = $"<path d=\"M {upperLeft.X} {upperLeft.Y} Q {controlUpper.X} {controlUpper.Y} {upperRight.X} {upperRight.Y}\"/>";
                result += resUpper;

                //TODO!+ Generate SVG for the extra arc below
                PointF lowerLeft = transform.Apply(leftmost.Col + extraLines.Value, leftmost.Row + extraLines.Value);
                PointF lowerRight = transform.Apply(rightmost.Col - extraLines.Value, rightmost.Row + extraLines.Value);
                PointF controlLower = new PointF(control.X, control.Y + transform.YScale * (float)extraLines.Value);

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

                //TODO!+ Generate SVG for the extra arc below
                //TODO!+ Generate SVG for the extra arc above
            }
        }

    }
}
