using System;
using System.Collections.Generic;
using System.Text;

namespace CelticKnotwork1
{
    static class KnotworkFactory
    {
        public static Knotwork Translate(Knotwork orig, int delta_rows, int delta_cols)
        {
            Knotwork res = new Knotwork(orig.Rows + delta_rows, orig.Cols + delta_cols); //TODO?~ What if rows/cols is negative?
            var elements = orig.GetAllLines();
            foreach (var elt in elements)
            {
                GridCoordinates coor = elt.Item1;
                LineSegment l = elt.Item2;
                res.AddLine(coor.Row + delta_rows, coor.Col + delta_cols, l); //TODO?~ Use a COPY of l? ...why not make this a method of Knotwork and do this in-place....
            }
            return res;
        }

        public static Knotwork SampleKnotwork3(int n)
        {
            // For robustness: n should be at least 1.
            if (n < 1)
            {
                n = 1;
            }

            Knotwork knotwork = new Knotwork(5, 5 + n * 6);

            // Upper and lower rows.
            for (int i = 1; i < 6 * n + 2; i += 2)
            {
                // Draw the top row.
                knotwork.AddLine(0, i, new HorizontalArcingUp());
                knotwork.AddLine(0, i, new DiagonalForwardDown());
                knotwork.AddLine(0, i + 2, new DiagonalBackwardDown());

                // Draw the bottom row.
                knotwork.AddLine(4, i, new HorizontalArcingDown());
                knotwork.AddLine(3, i + 1, new DiagonalBackwardDown());
                knotwork.AddLine(3, i + 1, new DiagonalForwardDown());
            }

            for (int i = 0; i < 6 * n + 3; i+= 2)
            { 

                // Draw the inner part of the bookmark.
                if ((i-4)%6==0)
                {
                    knotwork.AddLine(1, i, new HorizontalArcingDown());
                    knotwork.AddLine(3, i, new HorizontalArcingUp());
                }
                else
                {
                    knotwork.AddLine(1, i, new DiagonalForwardDown());
                    knotwork.AddLine(2, i + 1, new DiagonalBackwardDown());
                }

                if (i%6 != 0)
                {
                    knotwork.AddLine(1, i, new DiagonalBackwardDown());
                    knotwork.AddLine(2, i-1, new DiagonalForwardDown());
                }
                    
            }

            // Stright-lined part of the knot at the righthand side of the screen.
            knotwork.AddLine(1, 6 * n + 4, new DiagonalBackwardDown());
            knotwork.AddLine(2, 6 * n + 3, new DiagonalForwardDown());

            // Add the arcs at the beginning and the end.
            knotwork.AddLine(1, 0, new VerticalArcingLeft());
            knotwork.AddLine(1, 6 * n + 4, new VerticalArcingRight());


            return knotwork;
        }

        public static Knotwork SampleKnotwork1(int n)
        {
            // For robustness: n should be at least 1.
            if (n < 1)
            {
                n = 1;
            }

            Knotwork knotwork = new Knotwork(5 + n * 6, 5);
            for (int i = 0; i < knotwork.Rows; i++)
            {
                // Outermost columns.
                if (i > 0 && i < knotwork.Rows - 2 && i % 2 == 1)
                {
                    knotwork.AddLine(i, 0, new VerticalArcingLeft());
                    knotwork.AddLine(i, 4, new VerticalArcingRight());

                }
                if (i > 0 && i < knotwork.Rows - 2 && i % 2 == 1)
                {
                    knotwork.AddLine(i, 0, new DiagonalForwardDown());
                    knotwork.AddLine(i, 4, new DiagonalBackwardDown());
                }

                if (i > 1 && i % 2 == 1)
                {
                    knotwork.AddLine(i - 1, 0 + 1, new DiagonalBackwardDown());
                    knotwork.AddLine(i - 1, 4 - 1, new DiagonalForwardDown());
                }

                // Second outermost columns.
                if (i > 1 && i < knotwork.Rows - 2)
                {
                    if ((i - 4) % 6 == 0)
                    {
                        knotwork.AddLine(i, 1, new VerticalArcingRight());
                        knotwork.AddLine(i, 3, new VerticalArcingLeft());
                    }
                    else if (i % 2 == 0)
                    {
                        knotwork.AddLine(i, 1, new DiagonalForwardDown());
                        knotwork.AddLine(i, 3, new DiagonalBackwardDown());
                    }

                    if (i % 2 == 0 && i % 6 != 0)
                    {
                        knotwork.AddLine(i - 1, 1 + 1, new DiagonalBackwardDown());
                        knotwork.AddLine(i - 1, 3 - 1, new DiagonalForwardDown());
                    }
                }
            }

            // Top
            knotwork.AddLine(0, 1, new HorizontalArcingUp());
            knotwork.AddLine(1 - 1, 2 - 1, new DiagonalForwardDown());
            knotwork.AddLine(1 - 1, 2 + 1, new DiagonalBackwardDown());

            // Bottom
            knotwork.AddLine(knotwork.Rows - 1, 1, new HorizontalArcingDown());
            knotwork.AddLine(knotwork.Rows - 2, 2, new DiagonalBackwardDown());
            knotwork.AddLine(knotwork.Rows - 2, 2, new DiagonalForwardDown());

            // Done! Return the sample knotwork.
            return knotwork;
        }

        /// <summary>
        /// Draws a border.
        /// The border will only work proper if "rows" and "cols" are odd numbers.
        /// </summary>
        /// <param name="rows">An odd number specifying the number of rows the border has.</param>
        /// <param name="cols">An odd number specifying the number of columns the border has.</param>
        /// <param name="borderWidth">The width of the border, in grid points; should be at least 1.</param>
        /// <returns></returns>
        public static Knotwork SampleKnotwork2(int rows, int cols, int borderWidth)
        {
            Knotwork knotwork = new Knotwork(rows, cols);
            if (borderWidth <= 0)
            {
                borderWidth = 1;
            }

            // Draw the outer arcs on the top and bottom border.
            for (int col = 1; col < knotwork.Cols - 2; col += 2)
            {
                knotwork.AddLine(0, col, new HorizontalArcingUp());
                knotwork.AddLine(knotwork.Rows - 1, col, new HorizontalArcingDown());
            }

            // Draw the outer arcs on the left and right border.
            for (int row = 1; row < knotwork.Rows - 2; row += 2)
            {
                knotwork.AddLine(row, 0, new VerticalArcingLeft());
                knotwork.AddLine(row, knotwork.Cols - 1, new VerticalArcingRight());
            }

            // Draw the downward-going diagonals on the righthand side and the lefthand side.
            for (int row = 1; row < knotwork.Rows - 2; row += 2)
            {
                for (int i = 0; i < borderWidth; i++)
                {
                    if (row + i >= knotwork.Rows - i - 2)
                        break;

                    // Draw the diagonals on the lefthand side.
                    knotwork.AddLine(row + i, i, new DiagonalForwardDown());
                    // Draw the diagonals on the righthand side.
                    knotwork.AddLine(row + i, knotwork.Cols - 1 - i, new DiagonalBackwardDown());
                }
            }

            // Draw the upward-going diagonals on the righthand side and the lefthand side.
            for (int row = knotwork.Rows - 2; row > 1; row -= 2)
            {
                for (int i = 0; i < borderWidth; i++)
                {
                    if (row - 1 - i < i + 1)
                        break;

                    // Draw the diagonals on the lefthand side.
                    knotwork.AddLine(row - 1 - i, 1 + i, new DiagonalBackwardDown());
                    // Draw the diagonals on the righthand side.
                    knotwork.AddLine(row - 1 - i, knotwork.Cols - 2 - i, new DiagonalForwardDown());
                }
            }

            // Draw the arcs that close the lefthandside and righthandside borders.
            for (int i = borderWidth; i < knotwork.Rows - borderWidth - 3; i += 2)
            {
                knotwork.AddLine(i + 1, borderWidth, new VerticalArcingRight());
                knotwork.AddLine(i + 1, knotwork.Cols - borderWidth - 1, new VerticalArcingLeft());
            }

            // Draw the forward-going diagonals for the top and the bottom.
            for (int col = 1; col < knotwork.Cols - 2; col += 2)
            {
                for (int i = 0; i < borderWidth; i++)
                {
                    if (col + i >= knotwork.Cols - i - 2)
                        break;

                    knotwork.AddLine(i, col + i, new DiagonalForwardDown());
                    knotwork.AddLine(knotwork.Rows - i - 2, col + i + 1, new DiagonalBackwardDown());
                }
            }

            // Draw the backward-going diagonals for the top and the bottom.
            for (int col = 3; col < knotwork.Cols - 1; col += 2)
            {
                for (int i = 0; i < borderWidth; i++)
                {
                    if (col + i - 1 > knotwork.Cols - borderWidth - i + 2)
                        break;

                    knotwork.AddLine(0 + i, col + i, new DiagonalBackwardDown());
                    knotwork.AddLine(knotwork.Rows - 2 - i, col + i - 1, new DiagonalForwardDown());
                }
            }


            // Draw the arcs at the inner end of the top and bottom borders.
            for (int col = borderWidth + 2; col < knotwork.Cols - borderWidth - 2; col += 2)
            {
                knotwork.AddLine(borderWidth, col - 1, new HorizontalArcingDown());
                knotwork.AddLine(knotwork.Rows - borderWidth - 1, col - 1, new HorizontalArcingUp());
            }

            // Draw the line segments that connect the four borders.
            for (int i = 0; i < borderWidth; i++)
            {
                // Left top corner.
                knotwork.AddLine(i + 1, i + 2, new DiagonalBackwardDown());
                // Top right corner.
                knotwork.AddLine(i + 1, knotwork.Cols - 3 - i, new DiagonalForwardDown());
                //TODO!+ Lower left corner. Not done yet because some of these lines are already drawn by the bordering code - and that part of the code shouldn't.
                knotwork.AddLine(knotwork.Rows - 3 - i, i + 1, new DiagonalForwardDown());
                // Lower right corner
                knotwork.AddLine(knotwork.Rows - 3 - i, knotwork.Cols - 2 - i, new DiagonalBackwardDown());
            }

            return knotwork;
        }
    }
}
