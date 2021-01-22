using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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
        private double? m_extraLines = 0.2; // null;

        //private String svgFile = "C:\\Gebruikers\\Gebruiker\\Bureaublad\\knotwork.html";
        private String svgFile = "D:\\knotwork1.html";
        private StreamWriter sw;

        public Form1()
        {
            InitializeComponent();
            Graphics g = this.CreateGraphics();

            //TODO!~ Modify the culture to ONLY change the floating point separator. Also, see if this can be done in the project file instead of code.
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            //knotwork = KnotworkFactory.SampleKnotwork3(8);
            //knotwork = KnotworkFactory.SampleKnotwork2(47,25,3); // These parameters yield a border that consists of a single line.
            //knotwork = KnotworkFactory.SampleKnotwork2(25, 47, 3); // These parameters also yield a single line.
            knotwork = KnotworkFactory.SampleKnotwork2(15, 47, 3);// These parameters also yield a single line.
            transform = new SimpleTransform { XOffset = 50, XScale = 10, YOffset = 30, YScale = 10 };

            traversalPoint0 = originalPoint0;
            traversalPoint1 = originalPoint1;

            GenerateSvgInOrderOfTraversal();

            this.Paint += Form1_Paint;
            this.timer1.Interval = 100;// WAS: 125;
            this.timer1.Tick += Timer1_Tick;
            this.timer1.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool drawGrid = true;
            bool drawKnotwork = true;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.IndianRed, 1.0f);

            if (drawGrid)
            {
                DrawGrid(g, pen, knotwork.Rows, knotwork.Cols, transform);
            }
            if (drawKnotwork)
            {
                DrawKnotwork(g, pen, knotwork, transform, m_extraLines);
            }
        }

        void GenerateSvgInOrderOfTraversal()
        {
            if (File.Exists(svgFile))
            {
                File.Delete(svgFile);
            }
            sw = File.CreateText(svgFile);
            sw.WriteLine("<!DOCTYPE html>");
            sw.WriteLine("<html>");
            sw.WriteLine("  <head>");
            sw.WriteLine("    <meta charset=\"utf-8\">");
            sw.WriteLine("  </head>");
            sw.WriteLine("  <body>");
            sw.WriteLine("    <svg width=\"600\" height=\"800\" >"); //TODO!+  Parameterize the size...
                                                                     // I am deliberately NOT using SVG's "transform" option to apply the scaling and the translation.
                                                                     // While it might work for the translation, when scaling lines it scales both the size and the WIDTH of the lines - resulting in very broad lines.

            sw.WriteLine("      <g stroke=\"black\" fill=\"none\">");

            bool ready;
            traversalPoint0 = originalPoint0;
            traversalPoint1 = originalPoint1;

            do
            {
                GridCoordinates tmp = Traverse(traversalPoint0);
                traversalPoint0 = traversalPoint1;
                traversalPoint1 = tmp;

                GenerateSvgForConnection(transform, knotwork, traversalPoint0, traversalPoint1, m_extraLines);

                ready = traversalPoint0.Equals(originalPoint0) && traversalPoint1.Equals(traversalPoint1);
            } while (!ready);

            sw.WriteLine("      </g>");
            sw.WriteLine("    </svg>");
            sw.WriteLine("  </body>");
            sw.WriteLine("</html>");
            sw.Flush();
            sw.Close();
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            Graphics g = CreateGraphics();

            // Every time you find yourself at the starting line segment, determine which color to use.
            if (traversalPoint0.Equals(originalPoint0) && traversalPoint1.Equals(traversalPoint1))
            {
                altPen = colorFlipper ? new Pen(Color.Black) : new Pen(Color.DarkGoldenrod);
                colorFlipper = !colorFlipper;
            }

            GridCoordinates tmp = Traverse(traversalPoint0);
            traversalPoint0 = traversalPoint1;
            traversalPoint1 = tmp;


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

        List<GridCoordinates> RemoveDoubles(List<GridCoordinates> orig)
        {
            List<GridCoordinates> res = new List<GridCoordinates>();
            foreach (GridCoordinates p in orig)
            {
                if (!res.Contains(p))
                {
                    res.Add(p);
                }
            }
            return res;
        }

        GridCoordinates Traverse(GridCoordinates traversalPoint0)
        {
            // Find all points that are connected to the current point.
            List<GridCoordinates> connectedPoints = knotwork.GetConnectionsFor(traversalPoint1).ToList();
            // Remove the link to the point that you came from; this is the only one guaranteed not to be the one you need to visit next.
            connectedPoints = connectedPoints.FindAll(x => !x.Equals(traversalPoint0));

            // Remove doubles.
            connectedPoints = RemoveDoubles(connectedPoints);

            // If there's only one point left, then that is where we must go.
            if (connectedPoints.Count == 1)
            {
                return connectedPoints[0];
            }
            else
            {
                LineSegment lineSegment = knotwork.GetLine(traversalPoint0, traversalPoint1);

                // Find the direction that our Line Segment has at traversalPoint1.
                Direction? currentDirection = LineSegmentDirectionAtPointQ(knotwork, traversalPoint0, traversalPoint1);
                if (currentDirection == null)
                {
                    //TODO!+ Issue a warning.
                    return null;
                }

                // Then find the connected points that have a connection in the same/opposite direction at traversalPoint1.
                // There should be only 1, and that should be the next Line Segment.
                foreach (GridCoordinates connectedPoint in connectedPoints)
                {
                    LineSegment candidate = knotwork.GetLine(traversalPoint1, connectedPoint);
                    Direction? connectedDirection = LineSegmentDirectionAtPointQ(knotwork, connectedPoint, traversalPoint1);
                    if (connectedDirection == null)
                    {
                        //TODO!+ Issue a warning.
                        return null;
                    }

                    if (DirectionUtils.SameAxis(currentDirection.Value, connectedDirection.Value))
                    {
                        // Success!
                        return connectedPoint;
                    }
                }
            }

            return null;
        }

        //TODO!~ Find a way to do this using polymorphism.
        // We may have to specify the points as parameters to the LineSegment method to do this.
        /// <summary>
        /// Given a LineSegment from point P to point Q, give the direction that the LineSegment has at point Q.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns>The direction that the line segment has at <paramref name="q"/>; or <code>null</code> if there is no line segment from p to q.</returns>
        Direction? LineSegmentDirectionAtPointQ(Knotwork knotwork, GridCoordinates p, GridCoordinates q)
        {
            LineSegment l = knotwork.GetLine(p, q);
            if (l == null)
            {
                return null;
            }
            if (l is DiagonalForwardDown)
            {
                return Direction.SouthEast; // Note that it might also be NorthWest; what matters is the axis along which it goes.
            }
            else if (l is DiagonalBackwardDown)
            {
                return Direction.SouthWest; // Again, what matters is the axis along which the diagonal goes; so it may also be NorthEast.
            }
            else if (l is VerticalArcingLeft)
            {
                // In this case, the direction at q will always be somewhat to the East; but northeast or southeast?
                // That depends on whether q is higher or lower than p.
                if (p.Row < q.Row)
                {
                    return Direction.SouthEast;
                }
                else
                {
                    return Direction.NorthEast;
                }
            }
            else if (l is VerticalArcingRight)
            {
                // In this case, the direction at q will always to somwewhat to the west; but is it northwest or southwest? 
                // Again, that depends on whether q is higher or lower than p.
                if (p.Row < q.Row)
                {
                    return Direction.SouthWest;
                }
                else
                {
                    return Direction.NorthWest;
                }
            }
            else if (l is HorizontalArcingUp)
            {
                // In this case, the direction at q will always be somewhat to the south. But is it southeast or southwest?
                // That depends on whether p is to the left of q, or to the right.
                if (p.Col < q.Col)
                {
                    return Direction.SouthEast;
                }
                else
                {
                    return Direction.SouthWest;
                }
            }
            else if (l is HorizontalArcingDown)
            {
                // In this case, the direction at q will always be somewhat to the south. But is it northeast or northwest?
                // That depends on whether p is to the left of q, or to the right.
                if (p.Col < q.Col)
                {
                    return Direction.NorthEast;
                }
                else
                {
                    return Direction.NorthWest;
                }
            }

            // Should never reach here...
            return null;
        }

        /// <summary>
        /// Draw the line segment (or arc), that connects points p0 and p1, using the specified Pen.
        /// </summary>
        /// <param name="g">The graphics context to draw on.</param>
        /// <param name="pen">The pen (color) in which to draw the line(s).</param>
        /// <param name="transform">Transformation from grid coordinates to screen coordinates.</param>
        /// <param name="knotwork">The knotwork that the line segment or arc belongs to.</param>
        /// <param name="p0">Starting point of the line segment or arc to draw.</param>
        /// <param name="p1">End point of the line segment or arc to draw.</param>
        /// <param name="extraLines">The distance between the center line and the outer lines.
        /// When set to <code>null</code>, no outer lines are drawn.
        /// </param>
        void DrawConnection(Graphics g, Pen pen, SimpleTransform transform, Knotwork knotwork, GridCoordinates p0, GridCoordinates p1, double? extraLines)
        {
            LineSegment l = knotwork.GetLine(p0, p1);
            if (l == null)
            {
                return;
            }

            // If we are moving downward. "l" can be a vertical arc or a diagonal.
            if (p1.Row > p0.Row)
            {
                PaintVisitor v1 = new PaintVisitor(g, pen, p0, transform, extraLines);
                l.Accept(v1);
            }
            // If we are moving upward. "l" can be a vertical arc or a diagonal.
            else if (p1.Row < p0.Row)
            {
                PaintVisitor v1 = new PaintVisitor(g, pen, p1, transform, extraLines);
                l.Accept(v1);
            }
            else
            {
                // Staying at the same height, this can only be a horizontal arc.
                if (p1.Col > p0.Col)
                {
                    GridCoordinates p;
                    if (l is HorizontalArcingDown)
                    {
                        p = new GridCoordinates { Col = p0.Col + 1, Row = p0.Row - 1 };
                    }
                    else // l is HorizontalArcingUp
                    {
                        p = new GridCoordinates { Col = p0.Col + 1, Row = p0.Row + 1 };
                    }

                    PaintVisitor v1 = new PaintVisitor(g, pen, p, transform, extraLines);
                    l.Accept(v1);
                }
                else
                {
                    GridCoordinates p;

                    if (l is HorizontalArcingDown)
                    {
                        p = new GridCoordinates { Col = p0.Col - 1, Row = p0.Row - 1 };
                    }
                    else // l is HorizontalArcingUp
                    {
                        p = new GridCoordinates { Col = p0.Col - 1, Row = p0.Row + 1 };
                    }

                    PaintVisitor v1 = new PaintVisitor(g, pen, p, transform, extraLines);
                    l.Accept(v1);
                }
            }
        }

        void GenerateSvgForConnection(SimpleTransform transform, Knotwork knotwork, GridCoordinates p0, GridCoordinates p1, double? extraLines)
        {
            LineSegment l = knotwork.GetLine(p0, p1);
            if (l == null)
            {
                return;
            }

            // If we are moving downward. "l" can be a vertical arc or a diagonal.
            if (p1.Row > p0.Row)
            {
                SvgGeneratingVisitor v = new SvgGeneratingVisitor(p0, transform, extraLines);
                l.Accept(v);
                sw.WriteLine(v.GetResult());
            }
            // If we are moving upward. "l" can be a vertical arc or a diagonal.
            else if (p1.Row < p0.Row)
            {

                SvgGeneratingVisitor v = new SvgGeneratingVisitor(p1, transform, extraLines);
                l.Accept(v);
                sw.WriteLine(v.GetResult());
            }
            else
            {
                // Staying at the same height, this can only be a horizontal arc.
                if (p1.Col > p0.Col)
                {
                    GridCoordinates p;
                    if (l is HorizontalArcingDown)
                    {
                        p = new GridCoordinates { Col = p0.Col + 1, Row = p0.Row - 1 };
                    }
                    else // l is HorizontalArcingUp
                    {
                        p = new GridCoordinates { Col = p0.Col + 1, Row = p0.Row + 1 };
                    }

                    SvgGeneratingVisitor v = new SvgGeneratingVisitor(p, transform, extraLines);
                    l.Accept(v);
                    sw.WriteLine(v.GetResult());
                }
                else
                {
                    GridCoordinates p;

                    if (l is HorizontalArcingDown)
                    {
                        p = new GridCoordinates { Col = p0.Col - 1, Row = p0.Row - 1 };
                    }
                    else // l is HorizontalArcingUp
                    {
                        p = new GridCoordinates { Col = p0.Col - 1, Row = p0.Row + 1 };
                    }

                    SvgGeneratingVisitor v = new SvgGeneratingVisitor(p, transform, extraLines);
                    l.Accept(v);
                    sw.WriteLine(v.GetResult());
                }
            }

        }

        void DrawKnotwork(Graphics g, Pen pen, Knotwork knotwork, SimpleTransform transform, double? extraLines)
        {
            // Draw the actual knotwork.
            var connections = knotwork.GetAllLines();
            foreach (var connection in connections)
            {
                //TODO?~ Find out why this compensation is necessary.
                GridCoordinates start = connection.Item1;
                LineSegment l = connection.Item2;
                if (l is HorizontalArcingUp)
                {
                    start = new GridCoordinates { Col = start.Col + 1, Row = start.Row + 1 };
                } else if (l is HorizontalArcingDown)
                {
                    start = new GridCoordinates { Col = start.Col + 1, Row = start.Row - 1 };
                }

                PaintVisitor v = new PaintVisitor(g, pen, start, transform, extraLines);
                l.Accept(v);
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
