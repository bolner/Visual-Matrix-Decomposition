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
using MathNet.Numerics.LinearAlgebra;
using System.Drawing;

namespace RotationDecomposition {
    public class Program {
        public static void Main(string[] args) {
            Scene scene;
            var clock = new double[,] { {10, 0}, {20, 0} };
            var cross = new double[,] { {0.5, 0.5}, {-0.5, -0.5}, {0, 0}, {-0.5, 0.5}, {0.5, -0.5} };
            var shape = new double[,] { {2, 2}, {4, 4}, {3, 5}, {4, 6}, {2, 6}, {0, 6}, {1, 5}, {0, 4}, {2, 2}, {3, 2} };

            scene = new Scene("LU_rotation", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time) => getRotationMatrix(time));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time) => getRotationMatrix(0));
            scene.AddActor(shape, Color.White, (double time) => getRotationMatrix(time));
            scene.AddActor(shape, Color.LightGreen, (double time) => getRotationMatrix(time).LU().L);
            scene.AddActor(shape, Color.Orange, (double time) => getRotationMatrix(time).LU().U);
            scene.Play(120, Math.PI * 2);

            scene = new Scene("LU_translation", Color.Black);
            scene.AddActor(cross, Color.DimGray, (double time) => getTranslationMatrix(time));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time) => getTranslationMatrix(0));
            scene.AddActor(shape, Color.White, (double time) => getTranslationMatrix(time));
            scene.AddActor(shape, Color.LightGreen, (double time) => getTranslationMatrix(time).LU().L);
            scene.AddActor(shape, Color.Orange, (double time) => getTranslationMatrix(time).LU().U);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("LU_composite_RT", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time) => getCompositeMatrix_RT(time));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time) => getCompositeMatrix_RT(0));
            scene.AddActor(shape, Color.White, (double time) => getCompositeMatrix_RT(time));
            scene.AddActor(shape, Color.LightGreen, (double time) => getCompositeMatrix_RT(time).LU().L);
            scene.AddActor(shape, Color.Orange, (double time) => getCompositeMatrix_RT(time).LU().U);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("QR_rotation", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time) => getRotationMatrix(time));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time) => getRotationMatrix(0));
            scene.AddActor(shape, Color.White, (double time) => getRotationMatrix(time));
            scene.AddActor(shape, Color.LightGreen, (double time) => getRotationMatrix(time).QR().Q);
            scene.AddActor(shape, Color.Orange, (double time) => getRotationMatrix(time).QR().R);
            scene.Play(120, Math.PI * 2);

            scene = new Scene("QR_translation", Color.Black);
            scene.AddActor(cross, Color.Gray, (double time) => getTranslationMatrix(time));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time) => getTranslationMatrix(0));
            scene.AddActor(shape, Color.White, (double time) => getTranslationMatrix(time));
            scene.AddActor(shape, Color.LightGreen, (double time) => getTranslationMatrix(time).QR().Q);
            scene.AddActor(shape, Color.Orange, (double time) => getTranslationMatrix(time).QR().R);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("QR_composite_RT", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time) => getCompositeMatrix_RT(time));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time) => getCompositeMatrix_RT(0));
            scene.AddActor(shape, Color.White, (double time) => getCompositeMatrix_RT(time));
            scene.AddActor(shape, Color.LightGreen, (double time) => getCompositeMatrix_RT(time).QR().Q);
            scene.AddActor(shape, Color.Orange, (double time) => getCompositeMatrix_RT(time).QR().R);
            scene.Play(200, Math.PI * 2);
        }

        private static Matrix<double> getRotationMatrix(double time) {
            var R = Matrix<double>.Build.DenseOfArray(new double[,] {
                {Math.Cos(time), -Math.Sin(time), 0},
                {Math.Sin(time), Math.Cos(time), 0},
                {0, 0, 1}
            });

            return R;
        }

        private static Matrix<double> getTranslationMatrix(double time) {
            double x = Math.Cos(time) * 7 * (1 - Math.Cos(time * 2));
            double y = Math.Sin(time) * 7 * (1 - Math.Cos(time * 2));

            var T = Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 0, x},
                {0, 1, y},
                {0, 0, 1}
            });

            return T;
        }

        private static Matrix<double> getCompositeMatrix_RT(double time) {
            return getTranslationMatrix(time) * getRotationMatrix(time);
        }
    }
}
