using System;
using System.Collections.Generic;
using System.Text;

namespace CelticKnotwork1
{
    static class KnotworkFactory
    {
        public static Knotwork SampleKnotwork1(int n)
        {
            //TODO!+ Assert that n>= 1

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

        public static Knotwork SampleKnotwork2()
        {
            Knotwork knotwork = new Knotwork(11,15); //TODO!~

            for (int col = 0; col < knotwork.Cols - 1; col += 2)
            {
                knotwork.AddLine(0, col, new HorizontalArcingUp());
                knotwork.AddLine(knotwork.Rows - 1, col, new HorizontalArcingDown());
            }

            for (int row = 0; row < knotwork.Rows - 1; row += 2)
            {
                knotwork.AddLine(row, 0, new VerticalArcingLeft());
                knotwork.AddLine(row, knotwork.Cols - 1, new VerticalArcingRight());
            }

            //TODO!+

            return knotwork;
        }
    }
}
