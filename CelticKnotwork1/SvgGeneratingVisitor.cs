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
        private String result;

        public SvgGeneratingVisitor(GridCoordinates start, SimpleTransform transform)
        {
            this.start = start;
            this.transform = transform;
        }

        public String GetResult() { return result; }

        public override void VisitDiagonalForwardDown(DiagonalForwardDown l)
        {
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(l.Target(start).Col, l.Target(start).Row);
            result = $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\" stroke=\"black\"/>";
        }

        public override void VisitDiagonalBackwardDown(DiagonalBackwardDown l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            result = $"<line x1=\"{p1.X}\" y1=\"{p1.Y}\" x2=\"{p2.X}\" y2=\"{p2.Y}\" stroke=\"black\"/>";
        }

        public override void VisitVerticalArcingLeft(VerticalArcingLeft l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            int len = Math.Abs(p2.Y - p1.Y);
            Point control = new Point((int)(p1.X - 0.5 * len), (int)(p1.Y + 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\" stroke=\"black\" fill=\"none\"/>";
        }

        public override void VisitVerticalArcingRight(VerticalArcingRight l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col, start.Row);
            Point p2 = transform.Apply(target.Col, target.Row);
            int len = Math.Abs(p2.Y - p1.Y);
            Point control = new Point((int)(p1.X + 0.5 * len), (int)(p1.Y + 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\" stroke=\"black\" fill=\"none\"/>";
        }

        public override void VisitHorizontalArcingUp(HorizontalArcingUp l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col - 1, start.Row - 1);
            Point p2 = transform.Apply(target.Col - 1, target.Row - 1);
            int len = Math.Abs(p2.X - p1.X);
            Point control = new Point((int)(p1.X + 0.5 * len), (int)(p1.Y - 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\" stroke=\"black\" fill=\"none\"/>";
        }

        public override void VisitHorizontalArcingDown(HorizontalArcingDown l)
        {
            GridCoordinates target = l.Target(start);
            Point p1 = transform.Apply(start.Col - 1, start.Row + 1);
            Point p2 = transform.Apply(target.Col - 1, target.Row + 1);
            int len = Math.Abs(p2.X - p1.X);
            Point control = new Point((int)(p1.X + 0.5 * len), (int)(p1.Y + 0.5 * len));
            result = $"<path d=\"M {p1.X} {p1.Y} Q {control.X} {control.Y} {p2.X} {p2.Y}\" stroke=\"black\" fill=\"none\"/>";
        }

    }
}
