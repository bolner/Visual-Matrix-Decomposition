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
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace RotationDecomposition {
    public class Scene {
        private string folderName;
        private List<Actor> actors;
        private Color backgroundColor;
        private StatePair state;

        public class StatePair {
            public State Old { get; set; }
            public State Current { get; set; }

            public StatePair() {
                Old = new State();
                Current = new State();
            }

            public void Flip() {
                Old = Current;
                Current = new State();
            }
        }

        public class State {
            public Matrix<double> Rotate { get; set; }
            public Matrix<double> Translate { get; set; }
            public Matrix<double> Scale { get; set; }
            public Matrix<double> Shear { get; set; }
            public LU<double> LU { get; set; }
            public QR<double> QR { get; set; }
            public Matrix<double> M1 { get; set; }
            public Matrix<double> M2 { get; set; }
        }
        
        public Scene(string folderName, Color backgroundColor) {
            this.folderName = $"var/{folderName}";
            this.actors = new List<Actor>();
            this.backgroundColor = backgroundColor;
            this.state = new StatePair();
        }

        public void AddActor(double[,] points, Color color, Func<double, StatePair, Matrix<double>> role) {
            this.actors.Add(new Actor(points, color, role));
        }

        public void Play(int frameCount, double domain) {
            if (Directory.Exists(this.folderName)) {
                Console.WriteLine($" - {this.folderName}: Folder already exists. Skipping this render.");
                return;
            } else {
                Console.WriteLine($" - {this.folderName}: Starting render");
                Directory.CreateDirectory(this.folderName);
            }
            
            if (frameCount < 1) {
                throw new Exception("Invalid frameCount parameter.");
            }

            int shots = 5;
            int sm1 = shots - 1;
            double q = Math.Pow(1.0 / (double)shots, 1.0 / (double)(shots - 1));
            int width = 1080;
            int height = 1080;
            double zoom = 8;

            for (int frame = 0; frame < frameCount; frame++) {
                using (Bitmap mainImage = new Bitmap((int)width, (int)height))
                using (System.Drawing.Graphics mainGraphics = System.Drawing.Graphics.FromImage(mainImage))
                {
                    /*
                        Loop for temporal anti-aliasing
                    */
                    for(int shot = 0; shot < shots; shot++) {
                        double frameLength = domain / (double)frameCount;
                        double timeBase = frameLength * ((double)frame);
                        double time = timeBase - ((frameLength / 2) / sm1) * (sm1 - shot);

                        using (Bitmap image = new Bitmap((int)width, (int)height))
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image))
                        {
                            graphics.Clear(this.backgroundColor);
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphics.DrawAxes(graphics, width, height, 0, 0, zoom, Color.DimGray);

                            foreach(var actor in actors) {
                                var points = actor.GetTransformedPoints(time, this.state);

                                Graphics.Draw(graphics, width, height, 0, 0, zoom, points, actor.Color);
                            }

                            graphics.Flush();

                            /*
                                Blend to main (Temporal anti-aliasing)
                            */
                            if (shot == 0) {
                                mainGraphics.DrawImageUnscaled(image, 0, 0);
                            } else {
                                ColorMatrix matrix = new ColorMatrix();
                                matrix.Matrix33 = (float)Math.Pow(q, (double)shot);

                                ImageAttributes attributes = new ImageAttributes();
                                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                                mainGraphics.DrawImage(image, new Rectangle(0, 0, mainImage.Width, mainImage.Height), 0, 0,
                                    image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                            }
                        }

                        this.state.Flip();
                    }

                    mainGraphics.Flush();
                    mainImage.Save($"{this.folderName}/frame_{frame.ToString("D4")}.png", ImageFormat.Png);
                }
            }

            Console.WriteLine("   Finished.");
        }
    }
}
