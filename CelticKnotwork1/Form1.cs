using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CelticKnotwork1
{
    public partial class Form1 : Form
    {
        private readonly Knotwork knotwork;
        private readonly SimpleTransform transform;

        private readonly GridCoordinates originalPoint0 = new GridCoordinates { Row = 0, Col = 1 };
        private readonly GridCoordinates originalPoint1 = new GridCoordinates { Row = 0, Col = 3 };
        private GridCoordinates traversalPoint0;
        private GridCoordinates traversalPoint1;
        private bool colorFlipper = true;
        private Pen altPen;
        private bool m_extraLines = true;

        public Form1()
        {
            InitializeComponent();
            Graphics g = this.CreateGraphics();

            knotwork = KnotworkFactory.SampleKnotwork1(4);
            //knotwork = KnotworkFactory.SampleKnotwork2();
            transform = new SimpleTransform { XOffset = 50, XScale = 20, YOffset = 30, YScale = 20 };

            traversalPoint0 = originalPoint0;
            traversalPoint1 = originalPoint1;

            this.Paint += Form1_Paint;
            this.timer1.Interval = 125;
            this.timer1.Tick += Timer1_Tick;
            this.timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Graphics g = CreateGraphics();

            // Every time you find yourself at the starting line segment, determine which color to use.
            if (traversalPoint0.Equals(originalPoint0) && traversalPoint1.Equals(traversalPoint1))
            {
                altPen = colorFlipper ? new Pen(Color.Red) : new Pen(Color.Black);
                colorFlipper = !colorFlipper;
            }


            // First, find all points that are connected to the current point.
            List<GridCoordinates> connectedPoints = knotwork.GetConnectionsFor(traversalPoint1).ToList();

            // Second, the point that you came from, is the only point GUARANTEED not to be the one you need to visit next.
            connectedPoints = connectedPoints.FindAll(x => !x.Equals(traversalPoint0));

            // Third, if there is only one connected point, then that is where we must go.
            if (connectedPoints.Count == 1)
            {
                traversalPoint0 = traversalPoint1;
                traversalPoint1 = connectedPoints[0];
            }
            else
            {
                // Fourth, if there are multiple possible connections, find the one that starts in the same direction.
                // There's probably a simpler way, that is about checking the direction of the line segments.
                // Like: "the direction in which the previous line segment ends, is the one in which the next should start".
                // Note that "start" and "end" are somewhat relative towards your current direction. 
                // Either way, though, it's a property of the line segments. So our LineSegment class should have that added.
                
                // Also note that at the end of each knotwork, a sharp change of direction is possible.
                // So rather than requiring to follow in the current direction, you look at all connected line segments, and PREFER 
                // the one that continues the current direction.
                // Perhaps we should sort the list in order of "desirableness of direction".
                // However, there is one point that is ALWAYS ruled out, and that is the point that we came from.

                int rowDirection = traversalPoint1.Row - traversalPoint0.Row;
                int colDirection = traversalPoint1.Col - traversalPoint0.Col;

                GridCoordinates preferredNext1 = null, preferredNext2 = null;


                if (rowDirection == 1 && colDirection == 1)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col + 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
                }
                else if (rowDirection == 1 && colDirection == -1)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col - 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
                }
                else if (rowDirection == -1 && colDirection == -1)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col - 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col };
                }
                else if (rowDirection == -1 && colDirection == +1 )
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col + 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col };
                }
                else if (rowDirection == +2 && colDirection == 0)
                {
                    // Direction is downwards. But is it downwards to the left, or downwards to the right?

                    LineSegment connector = knotwork.GetLine(traversalPoint0, traversalPoint1);

                    // Was the last line segment a vertical arc, arcing towards the left?
                    // Then our next line segment should be a diagonal downwards and rightwards; or a downwards arc arcing towards the right;
                    // failing both, it's probably a line going up.
                    if (connector is VerticalArcingLeft)
                    {
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col + 1 };
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
                    }

                    // Was the last line segment a vertical arc, arcing towards the right?
                    // Then our next line segment should be a diagonal downwards and leftwards; or a downwards arc arcing towards the left;
                    // failing both, it's probably a line going up.
                    else if (connector is VerticalArcingRight)
                    {
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col - 1 };
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
                    }
                }
                else if (rowDirection == -2 && colDirection == 0)
                {
                    // Direction is upwards. But is it upwards to the left, or upwards to the right?

                    LineSegment connector = knotwork.GetLine(traversalPoint0, traversalPoint1);

                    // Was the last line segment a vertical arc, arcing towards the left?
                    // Then our direction is towards the left.
                    if (connector is VerticalArcingLeft)
                    {
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col + 1 };
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col };
                    }

                    // Was the last line segment a vertical arc, arcing towards the right?
                    // Then our direction is towards the right.
                    else if (connector is VerticalArcingRight)
                    {
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col - 1 };
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col };
                    }
                }


                if (connectedPoints.Contains(preferredNext1))
                {
                    traversalPoint0 = traversalPoint1;
                    traversalPoint1 = preferredNext1;
                }
                else if (connectedPoints.Contains(preferredNext2))
                {
                    traversalPoint0 = traversalPoint1;
                    traversalPoint1 = preferredNext2;
                }
            }



            // For robustness... we'd rather stop the animation, than crash.
            if (traversalPoint1 == null)
            {
                traversalPoint1 = new GridCoordinates { Row = -1, Col = -1 };
            }
            else
            {
                DrawConnection(g, altPen, transform, knotwork, traversalPoint0, traversalPoint1, m_extraLines);
            }
        }

        /// <summary>
        /// Draw the line segment (or arc), that connects points p0 and p1, using the specified Pen.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="transform"></param>
        /// <param name="knotwork"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="extraLines"></param>
        void DrawConnection(Graphics g, Pen pen, SimpleTransform transform, Knotwork knotwork, GridCoordinates p0, GridCoordinates p1, bool extraLines)
        {
            LineSegment l = knotwork.GetLine(p0, p1);
            if (l == null)
            {
                return;
            }

            // If we are moving downward
            if (p1.Row > p0.Row)
            {
                //TODO!~ In time, replace "l.Paint" with the code for drawing using a parametric function.
                if (l is DiagonalForwardDown || l is DiagonalBackwardDown)
                {
                    l.Paint(g, pen, p0, transform, extraLines);
                }

                //TODO!~ Move to the appropriate LineSegment child class.
                Pen pen2 = new Pen(Color.DarkBlue);
                if (l is VerticalArcingRight)
                {
                    //PointF p = new PointF { X = p0.Col - 1, Y = p0.Row + 0.5f };
                    PointF p = new PointF { X = p0.Col - 1, Y = p0.Row + 1 };
                    DrawVerticalRightwardsArc(g, pen2, transform, p, extraLines);
                }
                else if (l is VerticalArcingLeft)
                {
                    //PointF p = new PointF { X = p0.Col + 1, Y = p0.Row + 0.5f };
                    PointF p = new PointF { X = p0.Col + 1, Y = p0.Row + 1 };
                    DrawVerticalLeftwardsArc(g, pen2, transform, p, extraLines);
                }
            }
            // If we are moving upward
            else if (p1.Row < p0.Row)
            {
                Pen pen2 = new Pen(Color.DarkBlue);

                if (l is DiagonalForwardDown)                    
                {
                    //TODO!~ In time, replace "l.Paint" with the code for drawing using a parametric representation of the function.
                    // After that, move it to the appropriate LineSegment subclass.
                    l.Paint(g, pen, p1, transform, extraLines);
                }
                else if (l is DiagonalBackwardDown)
                {
                    //TODO!~ In time, replace "l.Paint" with the code for drawing using a parametric representation of the function.
                    // After that, move it to the appropriate LineSegment subclass.
                    l.Paint(g, pen, p1, transform, extraLines);
                }
                else if (l is VerticalArcingRight)
                {
                    //TODO!~ Add the option to add extra lines to this parametric representation of the function.
                    // After that, move it to the appropriate LineSegment subclass.
                    PointF p = new PointF { X = p1.Col - 1, Y = p1.Row + 1 };
                    DrawVerticalRightwardsArc(g, pen2, transform, p, extraLines);
                }
                else if (l is VerticalArcingLeft)
                {
                    //TODO!~ Add the option to add extra lines to this parametric representation of the function.
                    // After that, move it to the appropriate LineSegment subclass.
                    PointF p = new PointF { X = p1.Col + 1, Y = p1.Row + 1 };
                    DrawVerticalLeftwardsArc(g, pen2, transform, p, extraLines);
                }
            }
            else
            {
                // Staying at the same height, this can only be a horizontal arc.
                if (p1.Col > p0.Col)
                {
                    //TODO!~ In time, replace "l.Paint" with the code for drawing using a parametric function.
                    //l.Paint(g, pen, p0, transform, extraLines);

                    //TODO!~ Move to the appropriate LineSegment child class.
                    Pen pen2 = new Pen(Color.DarkBlue);
                    PointF p = new PointF { X = p0.Col + 1, Y = p0.Row + 1 };
                    DrawHorizontalUpwardsArc(g, pen2, transform, p, extraLines);
                }
                else
                {
                    //TODO!~ In time, replace "l.Paint" with the code for drawing using a parametric function.
                    //l.Paint(g, pen, p1, transform, extraLines);

                    //TODO!~ Move to the appropriate LineSegment child class.
                    Pen pen2 = new Pen(Color.DarkBlue);
                    PointF p = new PointF { X = p0.Col - 1, Y = p0.Row - 1 };
                    DrawHorizontalDownwardsArc(g, pen2, transform, p, extraLines);
                }
            }
        }

        private void DrawHorizontalUpwardsArc(Graphics g, Pen pen, SimpleTransform transform, PointF p0, bool extraLines)
        {
            DrawQuarterCircle(g, pen, transform, p0, 1.25, extraLines);
        }

        private void DrawHorizontalDownwardsArc(Graphics g, Pen pen, SimpleTransform transform, PointF p0, bool extraLines)
        {
            DrawQuarterCircle(g, pen, transform, p0, 0.25, extraLines);
        }

        private void DrawVerticalLeftwardsArc(Graphics g, Pen pen, SimpleTransform transform, PointF p0, bool extraLines)
        {
            DrawQuarterCircle(g, pen, transform, p0, 0.75, extraLines);
        }

        private void DrawVerticalRightwardsArc(Graphics g, Pen pen, SimpleTransform transform, PointF p0, bool extraLines)
        {
            DrawQuarterCircle(g, pen, transform, p0, 1.75, extraLines);
        }

        private void DrawQuarterCircle(Graphics g, Pen pen, SimpleTransform transform, PointF p0, double startRadians, bool extraLines)
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
            double innerRadius = radius - 0.2;
            double outerRadius = radius + 0.2;
            
            for (double t = 0.0; t <= 1.0; t += 0.1)
            {
                double angle = (startRadians + 0.5*t) * Math.PI;
                double ca = Math.Cos(angle);
                double sa = Math.Sin(angle);

                double x1 = p0.X + radius * ca;
                double y1 = p0.Y + radius * sa;
                d1 = transform.Apply(x1, y1);

                if (extraLines)
                {
                    double x1Inner = p0.X + innerRadius * ca;
                    double y1Inner = p0.Y + innerRadius * sa;
                    d1Inner = transform.Apply(x1Inner, y1Inner);

                    double x1Outer = p0.X + outerRadius * ca;
                    double y1Outer = p0.Y + outerRadius * sa;
                    d1Outer = transform.Apply(x1Outer, y1Outer);
                }

                if (d0 != null)
                {
                    g.DrawLine(pen, d0.Value, d1);
                    if (extraLines)
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool drawGrid = true;
            bool drawKnotwork = false;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 1.0f);

            if (drawGrid)
            {
                DrawGrid(g, pen, knotwork.Rows, knotwork.Cols, transform);
            }
            if (drawKnotwork)
            {
                DrawKnotwork(g, pen, knotwork, transform, m_extraLines);
            }
        }


        void DrawKnotwork(Graphics g, Pen pen, Knotwork knotwork, SimpleTransform transform, bool extraLines)
        {
            // Draw the actual knotwork.
            var connections = knotwork.GetAllLines();
            foreach (var connection in connections)
            {
                connection.Item2.Paint(g, pen, connection.Item1, transform, extraLines);
            }

        }

        private static void DrawGrid(Graphics g, Pen pen, int rows, int cols, SimpleTransform transform)
        {
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    // Draw the current grid point.
                    int xCoor = transform.XOffset + transform.XScale * col;
                    int yCoor = transform.YOffset + transform.YScale * row;
                    g.DrawRectangle(pen, new Rectangle(new Point(xCoor, yCoor), new Size(1, 1)));
                }
            }
        }
    }
}
