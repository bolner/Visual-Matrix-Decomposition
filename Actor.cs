using System;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace RotationDecomposition {
    public class Actor {
        public Color Color { get; }
        public Func<double, Matrix<double>> Role { get; }

        public Actor(Color color, Func<double, Matrix<double>> role) {
            this.Color = color;
            this.Role = role;
        }

        public Matrix<double> GetMatrix(double time) {
            return Role(time);
        }
    }
}
