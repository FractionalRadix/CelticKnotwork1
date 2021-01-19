using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CelticKnotwork1
{
    abstract class LineSegmentVisitor
    {
        abstract public void VisitDiagonalForwardDown(DiagonalForwardDown l);
        abstract public void VisitDiagonalBackwardDown(DiagonalBackwardDown l);
        abstract public void VisitVerticalArcingLeft(VerticalArcingLeft l);
        abstract public void VisitVerticalArcingRight(VerticalArcingRight l);
        abstract public void VisitHorizontalArcingUp(HorizontalArcingUp l);
        abstract public void VisitHorizontalArcingDown(HorizontalArcingDown l);
    }

}
