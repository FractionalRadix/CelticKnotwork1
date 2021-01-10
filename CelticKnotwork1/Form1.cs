using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CelticKnotwork1
{
    enum LineStyle
    {
        diagonalForwardUp, diagonalForwardDown,
        diagonalBackwardUp, diagonalBackwardDown,
        roundedDownArcingLeft, roundedDownArcingRight, 
        roundedForwardArcingUp, roundedForwardArcingDown
    };

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

        GridCoordinates traversalPoint = new GridCoordinates { Row = 0, Col = 1 };
        private void Timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(traversalPoint);

            Graphics g = CreateGraphics();

            //TODO!- For debugging. Draw a blinking little square, to see if the timer is working properly.
            Pen colorFlippingPen = new Pen(ColorFlipper ? Color.Red : Color.Green);
            g.DrawRectangle(colorFlippingPen, new Rectangle(new Point(10, 10), new Size(20, 20)));
            ColorFlipper = !ColorFlipper;
            Brush brush = new SolidBrush(Color.Black);
            g.DrawString($"({traversalPoint.Row},{traversalPoint.Col})", DefaultFont, brush, new PointF(10.0f, 10.0f));

            IEnumerable<LineStyle> linesIterator = knotwork.GetAllLines(traversalPoint);
            IEnumerator<LineStyle> lines = linesIterator.GetEnumerator();
            if (lines.MoveNext())
            {
                Pen p = new Pen(Color.Red); // new Pen(Color.Goldenrod);

                //TODO!~ Also check the incoming lines....
                //  And, you need to have a direction. The current/previous line affects the direction.
                LineStyle cur = lines.Current;

                DrawLine(g, p, cur, transform, traversalPoint.Row, traversalPoint.Col);

                switch (cur)
                {
                    case LineStyle.diagonalForwardUp:
                        traversalPoint.Col++;
                        traversalPoint.Row--;
                        break;
                    case LineStyle.diagonalForwardDown:
                        traversalPoint.Col++;
                        traversalPoint.Row++;
                        break;
                    case LineStyle.diagonalBackwardUp:
                        traversalPoint.Col--;
                        traversalPoint.Row--;
                        break;
                    case LineStyle.diagonalBackwardDown:
                        traversalPoint.Col--;
                        traversalPoint.Row++;
                        break;
                    case LineStyle.roundedDownArcingLeft:
                        traversalPoint.Row++;
                        break;
                    case LineStyle.roundedDownArcingRight:
                        traversalPoint.Row++;
                        break;
                    case LineStyle.roundedForwardArcingUp:
                        traversalPoint.Col++;
                        break;
                    case LineStyle.roundedForwardArcingDown:
                        traversalPoint.Col++;
                        break;
                    default:
                        break;
                }
                //TODO!+ Hop on to the next segment...
            }

        }

        void DrawKnotwork(Graphics g, Pen altPen, Knotwork knotwork, SimpleTransform transform)
        {
            bool drawGrid = false;

            for (int col = 0; col < knotwork.Cols; col++)
            {
                for (int row = 0; row < knotwork.Rows; row++)
                {
                    // Draw the current grid point.
                    if (drawGrid)
                    {
                        int xCoor = transform.XOffset + transform.XScale * col;
                        int yCoor = transform.YOffset + transform.YScale * row;
                        g.DrawRectangle(altPen, new Rectangle(new Point(xCoor, yCoor), new Size(1, 1)));
                    }

                    // Draw the lines (both arced and straight), if there are any.
                    IEnumerable<LineStyle> lines = knotwork.GetOutgoingLines(row, col);
                    foreach (LineStyle currentLine in lines)
                    {
                        DrawLine(g, altPen, currentLine, transform, row, col);
                    }
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen altPen = new Pen(Color.Black, 1.0f);

            DrawKnotwork(g, altPen, knotwork, transform);
        }

        private Rectangle CalculateBoundingRectangle(Ellipse ellipse)
        {
            Size sizeOfBoundingRect = new Size((int)(2.0 * ellipse.XRadius), (int)(2.0 * ellipse.YRadius));
            Point startOfBoundingRect = new Point((int)(ellipse.XCenter - ellipse.XRadius), (int)(ellipse.YCenter - ellipse.YRadius));
            return new Rectangle(startOfBoundingRect, sizeOfBoundingRect);
        }

        private void DrawArcs(Graphics g, Pen myPen, bool extraLines,
            Ellipse ellipse, float startAngle, float sweepAngle
        )
        {
            Rectangle boundingRect = CalculateBoundingRectangle(ellipse);
            g.DrawArc(myPen, boundingRect, startAngle, sweepAngle);

            if (extraLines)
            {
                Rectangle outerBoundingRect = CalculateBoundingRectangle(ellipse.increaseRadius(+4));
                g.DrawArc(myPen, outerBoundingRect, startAngle, sweepAngle);

                Rectangle innerBoundingRect = CalculateBoundingRectangle(ellipse.increaseRadius(-4));
                g.DrawArc(myPen, innerBoundingRect, startAngle, sweepAngle);
            }
        }

        private void DrawLine(Graphics g, Pen myPen, LineStyle current, SimpleTransform tf, int row, int col)
        {
            bool extraLines = true;

            //TODO!~ We're in the process of using the LineSegment class and its children, instead of the enum.
            LineSegment tmp = null;
            switch (current)
            {
                case LineStyle.roundedDownArcingLeft:
                    {
                        tmp = new DownwardArcingLeft();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                case LineStyle.roundedDownArcingRight:
                    {
                        tmp = new DownwardArcingRight();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                case LineStyle.roundedForwardArcingUp:
                    {
                        tmp = new ForwardArcingUp();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                case LineStyle.roundedForwardArcingDown:
                    {
                        tmp = new ForwardArcingDown();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                case LineStyle.diagonalForwardDown:
                    {
                        tmp = new DiagonalForwardDown();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                case LineStyle.diagonalForwardUp:
                    {
                        tmp = new DiagonalForwardUp();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                case LineStyle.diagonalBackwardDown:
                    {
                        tmp = new DiagonalBackwardDown();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                case LineStyle.diagonalBackwardUp:
                    {
                        tmp = new DiagonalBackwardUp();
                        //tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
                    }
                    break;
                default:
                    //TODO?+ Issue a warning?
                    break;
            }
            if (tmp != null)
            {
                tmp.Paint(g, myPen, new GridCoordinates { Col = col, Row = row }, tf, extraLines);
            }
        }
    }
}
