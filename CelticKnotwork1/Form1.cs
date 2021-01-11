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

        public Form1()
        {
            InitializeComponent();
            Graphics g = this.CreateGraphics();

            knotwork = KnotworkFactory.SampleKnotwork1(1);
            transform = new SimpleTransform { XOffset = 50, XScale = 20, YOffset = 30, YScale = 20 };

            this.Paint += Form1_Paint;
            this.timer1.Interval = 1000;
            this.timer1.Tick += Timer1_Tick;
            this.timer1.Start();
        }

        /// <summary>
        /// Just a flip bit, used to create a blinking square.
        /// The purpose of the blinking square is to show that the Timer is working as expected.
        /// In other words, just a debugging tool.
        /// </summary>
        private bool ColorFlipper = true;

        GridCoordinates traversalPoint0 = new GridCoordinates { Row = 1, Col = 0 };
        GridCoordinates traversalPoint1 = new GridCoordinates { Row = 2, Col = 1 };
        //GridCoordinates traversalPoint0 = new GridCoordinates { Row = 0, Col = 1 };
        //GridCoordinates traversalPoint1 = new GridCoordinates { Row = 0, Col = 3 };



        private void Timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(traversalPoint1);

            Graphics g = CreateGraphics();

            //TODO!- For debugging. Draw a blinking little square, to see if the timer is working properly.
            Pen colorFlippingPen = new Pen(ColorFlipper ? Color.Red : Color.Green);
            g.DrawRectangle(colorFlippingPen, new Rectangle(new Point(10, 10), new Size(20, 20)));
            ColorFlipper = !ColorFlipper;
            Brush brush = new SolidBrush(Color.Red);


            // First, find all points that are connected to the current point.
            List<GridCoordinates> connectedPoints = knotwork.GetConnectionsFor(traversalPoint1).ToList();

            // Second, if there is only one connected point, then that is where we must go.
            if (connectedPoints.Count == 1)
            {
                traversalPoint0 = traversalPoint1;
                traversalPoint1 = connectedPoints[0];
            }
            else
            {
                // Third, if there are multiple possible connections, find the one that starts in the same direction.
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
                else if (rowDirection == +2 && colDirection == 0)
                {
                    LineSegment connector = knotwork.GetLine(traversalPoint0, traversalPoint1);

                    // Was the last line segment a downards arc, arcing towards the left?
                    // Then our next line segment should be a diagonal downwards and rightwards; or a downwards arc arcing towards the right;
                    // failing both, it's probably a line going up.
                    if (connector is VerticalArcingLeft)
                    {
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col + 1 };
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
                    }

                    // Was the last line segment a downwards arc, arcing towards the right?
                    // Then our next line segment should be a diagonal downwards and leftwards; or a downwards arc arcing towards the left;
                    // failing both, it's probably a line going up.
                    else if (connector is VerticalArcingRight)
                    {
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col - 1 };
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
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



            //TODO!- Temporary, to prevent crash until the bug is properly solved.
            if (traversalPoint1 == null)
                traversalPoint1 = new GridCoordinates { Row = 0, Col = 0 };

            //TODO!~ Draw the right line segment.
            // Until we got that, let's just draw the point...
            int x = transform.XOffset + transform.XScale * traversalPoint1.Col;
            int y = transform.YOffset + transform.YScale * traversalPoint1.Row;
            g.FillEllipse(brush, new Rectangle(new Point(x-2,y-2), new Size(5,5)));
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool drawGrid = true;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 1.0f);

            if (drawGrid)
            {
                DrawGrid(g, pen, knotwork.Rows, knotwork.Cols, transform);
            }
            DrawKnotwork(g, pen, knotwork, transform);
        }

        void DrawKnotwork(Graphics g, Pen pen, Knotwork knotwork, SimpleTransform transform)
        {
            bool extraLines = true;

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
