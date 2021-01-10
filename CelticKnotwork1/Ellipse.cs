using System;
using System.Collections.Generic;
using System.Text;

namespace CelticKnotwork1
{
    /// <summary>
    /// The data for an ellipse, where the focii are on the same axis (either the X-axis or the Y-axis).
    /// </summary>
    class Ellipse
    {
        public double XCenter { get; private set; }
        public double YCenter { get; private set; }
        public double XRadius { get; private set; }
        public double YRadius { get; private set; }

        public Ellipse(double xCenter, double yCenter, double xRadius, double yRadius)
        {
            this.XCenter = xCenter;
            this.YCenter = yCenter;
            this.XRadius = xRadius;
            this.YRadius = yRadius;
        }

        /// <summary>
        /// Create a variant of the current ellipse. It's center is the same, but its radii are increased by <paramref name="delta">delta<delta>.</delta>
        /// You are allowed to specify a negative value, _decreasing_ the radii.
        /// </summary>
        /// <param name="delta">Value by which the x-radius and y-radius are changed.</param>
        /// <returns>A modified version of the present ellipse, where the x- and y-radii are changed by the specified value.</returns>
        public Ellipse increaseRadius(double delta)
        {
            return new Ellipse(XCenter, YCenter, XRadius + delta, YRadius + delta);
        }
    }
}
