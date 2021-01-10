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
                    knotwork.AddLine(i, 0, LineStyle.roundedDownArcingLeft);
                    knotwork.AddLine(i, 4, LineStyle.roundedDownArcingRight);
                }
                if (i > 0 && i < knotwork.Rows - 2 && i % 2 == 1)
                {
                    knotwork.AddLine(i, 0, LineStyle.diagonalForwardDown);
                    knotwork.AddLine(i, 4, LineStyle.diagonalBackwardDown);
                }

                if (i > 1 && i % 2 == 1)
                {
                    knotwork.AddLine(i, 0, LineStyle.diagonalForwardUp);
                    knotwork.AddLine(i, 4, LineStyle.diagonalBackwardUp);
                }

                // Second outermost columns.
                if (i > 1 && i < knotwork.Rows - 2)
                {
                    if ((i - 4) % 6 == 0)
                    {
                        knotwork.AddLine(i, 1, LineStyle.roundedDownArcingRight);
                        knotwork.AddLine(i, 3, LineStyle.roundedDownArcingLeft);
                    }
                    else if (i % 2 == 0)
                    {
                        knotwork.AddLine(i, 1, LineStyle.diagonalForwardDown);
                        knotwork.AddLine(i, 3, LineStyle.diagonalBackwardDown);
                    }

                    if (i % 2 == 0 && i % 6 != 0)
                    {
                        knotwork.AddLine(i, 1, LineStyle.diagonalForwardUp);
                        knotwork.AddLine(i, 3, LineStyle.diagonalBackwardUp);
                    }
                }

                // Top
                knotwork.AddLine(0, 1, LineStyle.roundedForwardArcingUp);
                knotwork.AddLine(1, 2, LineStyle.diagonalBackwardUp);
                knotwork.AddLine(1, 2, LineStyle.diagonalForwardUp);

                // Bottom
                knotwork.AddLine(knotwork.Rows - 1, 1, LineStyle.roundedForwardArcingDown);
                knotwork.AddLine(knotwork.Rows - 2, 2, LineStyle.diagonalBackwardDown);
                knotwork.AddLine(knotwork.Rows - 2, 2, LineStyle.diagonalForwardDown);

            }

            // Done! Return the sample knotwork.
            return knotwork;
        }
    }
}
