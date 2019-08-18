using System;
using MathNet.Numerics.LinearAlgebra;
using System.Drawing;

namespace RotationDecomposition {
    public class Program {
        public static void Main(string[] args) {
            Scene scene;
            var shape = new double[,] { {2, 2}, {4, 4}, {3, 5}, {4, 6}, {2, 6}, {0, 6}, {1, 5}, {0, 4}, {2, 2}, {3, 2} };

            scene = new Scene("LU_rotation", shape, Color.Black);
            scene.AddActor(Color.White, (double time) => getRotationMatrix(time));
            scene.AddActor(Color.LightGreen, (double time) => getRotationMatrix(time).LU().L);
            scene.AddActor(Color.Orange, (double time) => getRotationMatrix(time).LU().U);
            scene.Play(120, Math.PI * 2);

            scene = new Scene("QR_rotation", shape, Color.Black);
            scene.AddActor(Color.White, (double time) => getRotationMatrix(time));
            scene.AddActor(Color.LightGreen, (double time) => getRotationMatrix(time).QR().Q);
            scene.AddActor(Color.Orange, (double time) => getRotationMatrix(time).QR().R);
            scene.Play(120, Math.PI * 2);
        }

        private static Matrix<double> getRotationMatrix(double alpha) {
            var R = Matrix<double>.Build.DenseOfArray(new double[,] {
                {Math.Cos(alpha), -Math.Sin(alpha), 0},
                {Math.Sin(alpha), Math.Cos(alpha), 0},
                {0, 0, 1}
            });

            return R;
        }

        private static Matrix<double> getTranslationMatrix(double x, double y) {
            var T = Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 0, x},
                {0, 1, y},
                {0, 0, 1}
            });

            return T;
        }
    }
}
