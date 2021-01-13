using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CelticKnotwork1
{
    //TODO!+ Check if it would be mathematically correct, to rename this "LinearTransformation2D".
    class SimpleTransform
    {
        public int XOffset { get; set; }
        public int XScale { get; set; }
        public int YOffset { get; set; }
        public int YScale { get; set; }

        public Point Apply(double x, double y)
        {
            Point p = new Point();
            p.X = (int)(XOffset + x * XScale);
            p.Y = (int)(YOffset + y * YScale);
            return p;
        }
    }
}
