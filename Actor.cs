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
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace RotationDecomposition {
    public class Actor {
        public Color Color { get; }
        public Func<double, Scene.StatePair, Matrix<double>> Role { get; }
        private Vector<double>[] Points;

        public Actor(double[,] points, Color color, Func<double, Scene.StatePair, Matrix<double>> role) {
            this.Color = color;
            this.Role = role;
            this.Points = ToVectorArray(points);
        }

        public Matrix<double> GetMatrix(double time, Scene.StatePair state) {
            return Role(time, state);
        }

        public Vector<double>[] GetTransformedPoints(double time, Scene.StatePair state) {
            var result = new Vector<double>[Points.GetLength(0)];
            var M = Role(time, state);

            for(int i = 0; i < Points.GetLength(0); i++) {
                if (Points[i][0] == double.NegativeInfinity) {
                    // This is just an instruction to raise the pen
                    result[i] = Points[i];
                } else {
                    result[i] = M * Points[i];
                }
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
