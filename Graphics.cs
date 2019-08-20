/*
    Copyright 2019 Tamas Bolner
    
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at
    
      http://www.apache.org/licenses/LICENSE-2.0
    
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace RotationDecomposition {
    public class Graphics {
        public static void Draw(System.Drawing.Graphics graphics, double width, double height, double centerX, double centerY,
                double zoom, Vector<double>[] vectors, Color color) {

            PointF previous = new PointF(float.NegativeInfinity, float.NegativeInfinity);

            using (Pen pen = new Pen(color, 3.6f))
            {
                for (int i = 0; i < vectors.Length; i++) {
                    if (vectors[i][0] == double.NegativeInfinity) {
                        // Instruction for raising the pen
                        previous = new PointF(float.NegativeInfinity, float.NegativeInfinity);
                        continue;
                    }

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
                double zoom, Color color) {

            using (Pen originPen = new Pen(color, 1.1f)) {
                graphics.DrawLine(originPen, (float)(width * 0.5 - centerX * zoom), 0, (float)(width * 0.5 - centerX * zoom),
                    (float)height);
                graphics.DrawLine(originPen, 0, (float)(height * 0.5 - centerY * zoom), (float)width,
                    (float)(height * 0.5 - centerY * zoom));
            }
        }
    }
}
