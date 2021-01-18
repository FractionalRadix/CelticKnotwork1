﻿using System;
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
        private double? m_extraLines = null; // WAS: 0.2;

        public Form1()
        {
            InitializeComponent();
            Graphics g = this.CreateGraphics();

            //knotwork = KnotworkFactory.SampleKnotwork1(9);
            knotwork = KnotworkFactory.SampleKnotwork2(47,25,3); //TODO!+ These parameters work pretty well, but traversal stops in the top left corner??
            transform = new SimpleTransform { XOffset = 50, XScale = 10, YOffset = 30, YScale = 10 };

            traversalPoint0 = originalPoint0;
            traversalPoint1 = originalPoint1;

            this.Paint += Form1_Paint;
            this.timer1.Interval = 25;// WAS: 125;
            this.timer1.Tick += Timer1_Tick;
            this.timer1.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool drawGrid = false;
            bool drawKnotwork = false;
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

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Graphics g = CreateGraphics();

            // Every time you find yourself at the starting line segment, determine which color to use.
            if (traversalPoint0.Equals(originalPoint0) && traversalPoint1.Equals(traversalPoint1))
            {
                altPen = colorFlipper ? new Pen(Color.Black) : new Pen(Color.DarkGoldenrod);
                colorFlipper = !colorFlipper;
            }

            //Original_Traversal(g);

            New_Traversal(g);
        }

        List<GridCoordinates> removeDoubles(List<GridCoordinates> orig)
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

        void New_Traversal(Graphics g)
        {
            // Find all points that are connected to the current point.
            List<GridCoordinates> connectedPoints = knotwork.GetConnectionsFor(traversalPoint1).ToList();
            // Remove the link to the point that you came from; this is the only one guaranteed not to be the one you need to visit next.
            connectedPoints = connectedPoints.FindAll(x => !x.Equals(traversalPoint0));

            // Remove doubles.
            //TODO?~ See if there isn't a nice lambda expression for this.
            connectedPoints = removeDoubles(connectedPoints);

            // If there's only one point left, then that is where we must go.
            if (connectedPoints.Count == 1)
            {
                traversalPoint0 = traversalPoint1;
                traversalPoint1 = connectedPoints[0];
            }
            else
            {
                LineSegment lineSegment = knotwork.GetLine(traversalPoint0, traversalPoint1);

                // Find the direction that our Line Segment has at traversalPoint1.
                Direction? currentDirection = LineSegmentDirectionAtPointQ(knotwork, traversalPoint0, traversalPoint1);
                if (currentDirection == null)
                {
                    //TODO!+ Issue a warning.
                    return;
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
                        return;
                    }

                    if (sameAxis(currentDirection.Value, connectedDirection.Value))
                    {
                        // Success!
                        traversalPoint0 = traversalPoint1;
                        traversalPoint1 = connectedPoint;
                        break;
                    }
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

        bool sameAxis(Direction dir1, Direction dir2)
        {
            if (dir1.Equals(dir2))
            {
                return true;
            }

            switch (dir1)
            {
                case Direction.North:
                    return dir2.Equals(Direction.South);
                case Direction.NorthEast:
                    return dir2.Equals(Direction.SouthWest);
                case Direction.East:
                    return dir2.Equals(Direction.West);
                case Direction.SouthEast:
                    return dir2.Equals(Direction.NorthWest);
                case Direction.South:
                    return dir2.Equals(Direction.North);
                case Direction.SouthWest:
                    return dir2.Equals(Direction.NorthEast);
                case Direction.West:
                    return dir2.Equals(Direction.East);
                case Direction.NorthWest:
                    return dir2.Equals(Direction.SouthEast);
            }

            // Should never reach here...
            //TODO?+ Issue an error
            return false;
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

        //TODO!- Replaced with the new, more intuitive algorithm.
        void Original_Traversal(Graphics g)
        {
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

                GridCoordinates preferredNext1, preferredNext2, preferredNext3;
                CalculatePreferredNextPoints(out preferredNext1, out preferredNext2, out preferredNext3);

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
                else if (connectedPoints.Contains(preferredNext3))
                {
                    traversalPoint0 = traversalPoint1;
                    traversalPoint1 = preferredNext3;
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

        //TODO!- Caller is replaced by a new, more intuitive algorithm.
        // Note that that requires making traverselPoint0 and traversalPoint1 parameters of this method.
        private void CalculatePreferredNextPoints(out GridCoordinates preferredNext1, out GridCoordinates preferredNext2, out GridCoordinates preferredNext3)
        {
            int rowDirection = traversalPoint1.Row - traversalPoint0.Row;
            int colDirection = traversalPoint1.Col - traversalPoint0.Col;

            preferredNext1 = null;
            preferredNext2 = null;
            preferredNext3 = null;


            if (rowDirection == -2 && colDirection == 0)
            {
                // Direction is upwards. But is it upwards to the left, or upwards to the right?

                LineSegment connector = knotwork.GetLine(traversalPoint0, traversalPoint1);

                // Was the last line segment a vertical arc, arcing towards the left?
                // Then our direction is towards the left.
                if (connector is VerticalArcingLeft)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col + 1 }; // Diagonal
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col }; // VerticalArcingRight
                    preferredNext3 = new GridCoordinates { Row = traversalPoint1.Row, Col = traversalPoint1.Col + 2 }; // HorizontalArcingUp
                }

                // Was the last line segment a vertical arc, arcing towards the right?
                // Then our direction is towards the right.
                else if (connector is VerticalArcingRight)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col - 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col };
                    //TODO?+ preferredNext3 ?
                }
            }
            else if (rowDirection == -1)
            {
                if (colDirection == -1)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col - 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col };
                }
                else if (colDirection == +1)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col + 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col };
                }
            }
            else if (rowDirection == 0)
            {
                if (colDirection == -2)
                {
                    LineSegment l = knotwork.GetLine(traversalPoint0, traversalPoint1);
                    if (l is HorizontalArcingDown)
                    {
                        // We're headed backwards and upwards.
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col - 1 }; // Diagonal
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row, Col = traversalPoint1.Col - 2 }; // HorizontalArcingUp
                        preferredNext3 = new GridCoordinates { Row = traversalPoint1.Row - 2, Col = traversalPoint1.Col }; // VerticalArcingLeft
                    }
                    else if (l is HorizontalArcingUp)
                    {
                        // We're headed backwards and downwards.
                        preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col - 1 }; // Diagonal
                        preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row, Col = traversalPoint1.Col - 2}; // HorizontalArcingDown
                        preferredNext3 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col }; // VerticalArcingRight
                    }
                }
                else if (colDirection == +2)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col + 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row, Col = traversalPoint1.Col + 2 };
                    //TODO!+ The next line should have a conditional: the LineSegment that you came from, should be a downwards facing arc.
                    preferredNext3 = new GridCoordinates { Row = traversalPoint1.Row - 1, Col = traversalPoint1.Col + 1 };
                }
            }
            else if (rowDirection == 1)
            {
                if (colDirection == -1)
                {
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col - 1 };
                    //TODO!+ The following is only possible if the connected line is a vertical arc arcing left. Not if it is a vertical arc arcing right.
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
                    //TODO!+ The following is only possible if the connected line is a horizontal arc arcing down. Not if it is a horizontal arc arcing up.
                    preferredNext3 = new GridCoordinates { Row = traversalPoint1.Row, Col = traversalPoint1.Col - 2 };
                }
                else if (colDirection == 1)
                {
                    //TODO!~ The second and third case are when the connected line is an arc, either vertical or horizontal.
                    // Note however, that the direction in which the arc bends, determines if it is a viable second point or not.
                    // And you should not have both possible arcs connected.
                    preferredNext1 = new GridCoordinates { Row = traversalPoint1.Row + 1, Col = traversalPoint1.Col + 1 };
                    preferredNext2 = new GridCoordinates { Row = traversalPoint1.Row + 2, Col = traversalPoint1.Col };
                    preferredNext3 = new GridCoordinates { Row = traversalPoint1.Row, Col = traversalPoint1.Col + 2 };
                }
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
                l.Paint2(g, pen, p0, transform, extraLines);
            }
            // If we are moving upward. "l" can be a vertical arc or a diagonal.
            else if (p1.Row < p0.Row)
            {
                l.Paint2(g, pen, p1, transform, extraLines);
            }
            else
            {
                // Staying at the same height, this can only be a horizontal arc.
                if (p1.Col > p0.Col)
                {
                    GridCoordinates p = new GridCoordinates { Col = p0.Col + 1, Row = p0.Row + 1 };
                    //GridCoordinates p = new GridCoordinates { Col = p0.Col + 1, Row = p0.Row - 1 };
                    l.Paint2(g, pen, p, transform, extraLines);
                }
                else
                {
                    GridCoordinates p = new GridCoordinates { Col = p0.Col - 1, Row = p0.Row - 1 };
                    l.Paint2(g, pen, p, transform, extraLines);
                }
            }
        }

        void DrawKnotwork(Graphics g, Pen pen, Knotwork knotwork, SimpleTransform transform, double? extraLines)
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
