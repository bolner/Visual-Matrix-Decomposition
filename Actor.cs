using System;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace RotationDecomposition {
    public class Actor {
        public Color Color { get; }
        public Func<double, Matrix<double>> Role { get; }
        private Vector<double>[] Points;

        public Actor(double[,] points, Color color, Func<double, Matrix<double>> role) {
            this.Color = color;
            this.Role = role;
            this.Points = ToVectorArray(points);
        }

        public Matrix<double> GetMatrix(double time) {
            return Role(time);
        }

        public Vector<double>[] GetTransformedPoints(double time) {
            var result = new Vector<double>[Points.GetLength(0)];
            var M = Role(time);

            for(int i = 0; i < Points.GetLength(0); i++) {
                result[i] = M * Points[i];
            }

            return result;
        }

        private Vector<double>[] ToVectorArray(double[,] vectors) {
            var items = new Vector<double>[vectors.GetLength(0)];
            
            for(int i = 0; i < vectors.GetLength(0); i++) {
                var v3 = Vector<double>.Build.Dense(3);
                
                v3[0] = vectors[i, 0];
                v3[1] = vectors[i, 1];
                v3[2] = 1;
                
                items[i] = v3;
            }
            
            return items;
        }
    }
}
