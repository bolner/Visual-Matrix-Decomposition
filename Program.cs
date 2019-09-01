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
            var clock = new double[,] { {0, 0}, {200, 0} };
            var line = new double[,] { {-200, 0}, {200, 0} };
            var cross = new double[,] { {1.5, 1.5}, {-1.5, -1.5}, {double.NegativeInfinity, double.NegativeInfinity}, {-1.5, 1.5}, {1.5, -1.5} };
            var shape = new double[,] {
                {6, 14}, {6, 6}, {20, 6}, {20, 14}, {double.NegativeInfinity, double.NegativeInfinity},
                {4, 14}, {22, 14}, {13, 20}, {4, 14}, {double.NegativeInfinity, double.NegativeInfinity}, // Roof
                {16, 18}, {16, 21}, {17.5, 21}, {17.5, 17}, {double.NegativeInfinity, double.NegativeInfinity}, // Chimney
                {8, 8}, {13, 8}, {13, 12}, {8, 12}, {8, 8}, {double.NegativeInfinity, double.NegativeInfinity}, // Window
                {15, 6}, {15, 12}, {18, 12}, {18, 6} // Door
            };
            var identity = Matrix<double>.Build.DenseIdentity(3);
            var e = Vector<double>.Build.DenseOfArray(new double[] {1, 1, 1});
            var rot180 = getRotationMatrix(Math.PI);

            scene = new Scene("LU_translation", Color.Black);
            scene.AddActor(cross, Color.Gray, (double time, Scene.StatePair state)
                => (state.Current.Translate = getTranslationMatrix(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.Translate);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.LU = state.Current.Translate.LU()).L);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.LU.U);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("LU_rotation", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.Rotate = getRotationMatrix(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.Rotate);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.LU = state.Current.Rotate.LU()).L);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.LU.U);
            scene.Play(120, Math.PI * 2);

            scene = new Scene("LU_composite_RT", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RT(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.LU = state.Current.M1.LU()).L);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.LU.U);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("LU_composite_RTS", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.Rotate = getRotationMatrix(time)));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RTS(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.LU = state.Current.M1.LU()).L);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.LU.U);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("LU_composite_RTH", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RTH(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.LU = state.Current.M1.LU()).L);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.LU.U);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("QR_rotation", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.Rotate = getRotationMatrix(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.Rotate);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.QR = state.Current.Rotate.QR()).Q);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.QR.R);
            scene.Play(120, Math.PI * 2);

            scene = new Scene("QR_translation", Color.Black);
            scene.AddActor(cross, Color.Gray, (double time, Scene.StatePair state)
                => (state.Current.Translate = getTranslationMatrix(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.Translate);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.QR = state.Current.Translate.QR()).Q);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.QR.R);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("QR_composite_RT", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RT(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.QR = state.Current.M1.QR()).Q);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.QR.R);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("QR_composite_RTS", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RTS(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.QR = state.Current.M1.QR()).Q);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.QR.R);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("QR_composite_RTH", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RTH(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => (state.Current.QR = state.Current.M1.QR()).Q);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.QR.R);
            scene.Play(200, Math.PI * 2);

            scene = new Scene("QR_composite_RTS_continuous", Color.Black);
            scene.AddActor(line, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.Rotate = getRotationMatrix(time)));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RTS2(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state) => {
                var qr = state.Current.M1.QR();
                double angle;

                if (state.Old.M1 == null) {
                    angle = Math.PI;
                } else {
                    angle = getAngle2D(state.Old.M1 * e, qr.Q * e);
                }
                
                if (angle > Math.PI / 2.0) {
                    /*
                        In case the rotation would jump PI (compared to the previous frame),
                        rotate both Q and R with PI, to cancel the jump. This works, because:
                            - Rotation matrices around the same point commute with each other,
                                so M*Q*M*R = Q*M*M*R = Q*R
                            - R remains upper triangular after the PI rotation, because the PI
                                rotation matrix is also upper triangular (UT), and the product of
                                2 UT matrices will always remain UT.
                            - Q remains orthogonal, since all rotation matrices are orthogonal.
                    */
                    state.Current.M1 = rot180 * qr.Q;
                    state.Current.M2 = rot180 * qr.R;
                } else {
                    state.Current.M1 = qr.Q;
                    state.Current.M2 = qr.R;
                }

                return state.Current.M1;
            });
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state) => state.Current.M2);
            scene.Play(300, Math.PI * 2);

            scene = new Scene("Eigen_composite_RTS", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RTS(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.FromArgb(255, 120, 180, 255), (double time, Scene.StatePair state)
                => (state.Current.Evd = state.Current.M1.Evd()).EigenVectors);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state)
                => state.Current.Evd.D);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => state.Current.Evd.EigenVectors.Inverse());
            scene.Play(200, Math.PI * 2);

            scene = new Scene("Eigen_composite_RTH", Color.Black);
            scene.AddActor(clock, Color.DimGray, (double time, Scene.StatePair state) => getRotationMatrix(time));
            scene.AddActor(cross, Color.DimGray, (double time, Scene.StatePair state)
                => (state.Current.M1 = getCompositeMatrix_RTH(time)));
            scene.AddActor(shape, Color.FromArgb(255, 40, 40, 40), (double time, Scene.StatePair state) => identity);
            scene.AddActor(shape, Color.White, (double time, Scene.StatePair state) => state.Current.M1);
            scene.AddActor(shape, Color.FromArgb(255, 120, 180, 255), (double time, Scene.StatePair state)
                => (state.Current.Evd = state.Current.M1.Evd()).EigenVectors);
            scene.AddActor(shape, Color.Orange, (double time, Scene.StatePair state)
                => state.Current.Evd.D);
            scene.AddActor(shape, Color.LightGreen, (double time, Scene.StatePair state)
                => state.Current.Evd.EigenVectors.Inverse());
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
            double x = Math.Cos(time) * 20 * (1 - Math.Cos(time * 2));
            double y = Math.Sin(time) * 20 * (1 - Math.Cos(time * 2));

            var T = Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 0, x},
                {0, 1, y},
                {0, 0, 1}
            });

            return T;
        }

        private static Matrix<double> getTranslationMatrix2(double time) {
            double x = Math.Cos(time) * 40 * Math.Sin(time * 2);
            double y = Math.Sin(time) * 40 * Math.Sin(time * 2);

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

        /// <summary>
        /// This has to be applied first (right-most in the chain)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static Matrix<double> getScalingMatrix(double time) {
            double x = 1 + Math.Sin(time) / 2.5;

            var ToCenter = Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 0, 13},
                {0, 1, 12},
                {0, 0, 1}
            });

            var GoBack = Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 0, -13 * x},
                {0, 1, -12 * x},
                {0, 0, 1}
            });

            var T = Matrix<double>.Build.DenseOfArray(new double[,] {
                {x, 0, 0},
                {0, x, 0},
                {0, 0, 1}
            });

            return GoBack * T * ToCenter;
        }

        private static Matrix<double> getCompositeMatrix_RTS(double time) {
            return getTranslationMatrix(time) * getRotationMatrix(time) * getScalingMatrix(time);
        }

        private static Matrix<double> getCompositeMatrix_RTS2(double time) {
            return getTranslationMatrix2(time) * getRotationMatrix(time) * getScalingMatrix(time);
        }

        private static Matrix<double> getShearMatrix(double time) {
            double x = (1 - Math.Cos(time)) / 2.0;

            var T = Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, x, 0},
                {0, 1, 0},
                {0, 0, 1}
            });

            return T;
        }

        private static Matrix<double> getCompositeMatrix_RTH(double time) {
            return getTranslationMatrix(time) * getRotationMatrix(time) * getShearMatrix(time);
        }

        private static double getAngle2D(Vector<double> v1, Vector<double> v2) {
            return Math.Acos(
                (v1[0] * v2[0] + v1[1] * v2[1])
                / Math.Sqrt((v1[0] * v1[0] + v1[1] * v1[1]) * (v2[0] * v2[0] + v2[1] * v2[1]))
            );
        }
    }
}
