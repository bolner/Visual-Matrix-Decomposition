using System;
using System.Collections.Generic;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace RotationDecomposition {
    public class Graphics {
        public static void Draw(System.Drawing.Graphics graphics, double width, double height, double centerX, double centerY,
                double zoom, Vector<double>[] vectors, Color color) {

            PointF previous = new PointF(float.NegativeInfinity, float.NegativeInfinity);

            using (Pen pen = new Pen(color, 4.2f))
            {
                for (int i = 0; i < vectors.Length; i++) {
                    PointF next = new PointF(
                        (float)((width * 0.5) + (vectors[i][0] - centerX) * zoom),
                        (float)((height * 0.5) - (vectors[i][1] - centerY) * zoom)
                    );

                    if (previous.X != float.NegativeInfinity) {
                        graphics.DrawLine(pen, previous, next);
                    }

                    previous = next;
                }
            }
        }

        public static void DrawAxes(System.Drawing.Graphics graphics, double width, double height, double centerX, double centerY,
                double zoom, Vector<double>[] vectors, Color color) {

            using (Pen originPen = new Pen(color, 1.1f)) {
                graphics.DrawLine(originPen, (float)(width * 0.5 - centerX * zoom), 0, (float)(width * 0.5 - centerX * zoom),
                    (float)height);
                graphics.DrawLine(originPen, 0, (float)(height * 0.5 - centerY * zoom), (float)width,
                    (float)(height * 0.5 - centerY * zoom));
            }
        }
    }
}
