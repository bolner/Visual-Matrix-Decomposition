using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace RotationDecomposition {
    public class Scene {
        private string folderName;
        private List<Actor> actors;
        private Color backgroundColor;

        public Scene(string folderName, Color backgroundColor) {
            this.folderName = $"var/{folderName}";
            this.actors = new List<Actor>();
            this.backgroundColor = backgroundColor;
        }

        public void AddActor(double[,] points, Color color, Func<double, Matrix<double>> role) {
            this.actors.Add(new Actor(points, color, role));
        }

        public void Play(int frameCount, double speed) {
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

            // We have 3 shots per frame as temporal anti-aliasing (first has alpha=1)
            double q = Math.Pow(1.0 / 3.0, 1.0 / 2.0);
            int width = 1080;
            int height = 1080;
            double zoom = 25;

            for (int frame = 0; frame < frameCount; frame++) {
                using (Bitmap mainImage = new Bitmap((int)width, (int)height))
                using (System.Drawing.Graphics mainGraphics = System.Drawing.Graphics.FromImage(mainImage))
                {
                    /*
                        Temporal anti-aliasing
                    */
                    for(int shot = 0; shot < 3; shot++) {
                        double frameLength = speed / (double)frameCount;
                        double timeBase = frameLength * ((double)frame);
                        double time = timeBase - (frameLength / 4) * (2 - shot);

                        using (Bitmap image = new Bitmap((int)width, (int)height))
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image))
                        {
                            graphics.Clear(this.backgroundColor);
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphics.DrawAxes(graphics, width, height, 0, 0, zoom, Color.DimGray);

                            foreach(var actor in actors) {
                                var points = actor.GetTransformedPoints(time);

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
                    }

                    mainGraphics.Flush();
                    mainImage.Save($"{this.folderName}/frame_{frame.ToString("D4")}.png", ImageFormat.Png);
                }
            }

            Console.WriteLine("   Finished.");
        }
    }
}
