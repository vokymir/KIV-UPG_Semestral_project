using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ElectricFieldVis.View
{
    /// <summary>
    /// In charge of rendering the actual frame.
    /// </summary>
    public class Renderer
    {
        public List<Particle> _particles;
        public Probe _mainProbe;
        private CustomizerForm? _customizerForm;
        private bool _particleDynamicWidth = true;
        private Color _particlePositiveColor = Color.Red;
        private Color _particleNegativeColor = Color.Blue;
        private Size _curr_client_size = new Size(800, 600);
        public Probe? _secondProbe = null;
        private int _bitmap_chunk_size = 10;

        private bool _showGrid = true;
        private bool _showStaticProbes = true;

        private Vector2 _origin = Vector2.Zero;
        public Vector2 Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
            }
        }


        private float _scale = 1f;
        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                float new_scale = _scale + value;
                if (new_scale > 0)
                {
                    _scale = new_scale;
                }
            }
        }

        /// <summary>
        /// Init the rendere and also set correct scaling.
        /// </summary>
        /// <param name="particles">List of particles to render.</param>
        /// <param name="probe">Probe to render.</param>
        /// <param name="clientSize">Client size.</param>
        public Renderer(List<Particle> particles, Probe probe, Size clientSize, int grid_w, int grid_h)
        {
            this._particles = particles;
            this._mainProbe = probe;
            this._grid_w = grid_w;
            this._grid_h = grid_h;

            _curr_client_size = clientSize;

            InitWindow(clientSize);
        }

        #region CalculateAndTranslate

        /// <summary>
        /// Updating the scaling parameters on resize
        /// </summary>
        /// <param name="clientSize"></param>
        public void InitWindow(Size clientSize)
        {
            ScaleToFill(clientSize);

            CenterOrigin(clientSize);
        }
        public void ScaleToFill(Size? clientSize = null)
        {
            if (clientSize == null)
            {
                clientSize = _curr_client_size;
            }

            Size clSize = (Size)clientSize;

            // finding the min and max values in both axis
            float minX = 0;
            float minY = 0;
            float maxX = 0;
            float maxY = 0;

            foreach (Particle particle in _particles)
            {
                float particleRadius = CalculateParticleRadius(particle);

                if (particle.X - particleRadius < minX) { minX = particle.X - particleRadius; };
                if (particle.Y - particleRadius < minY) { minY = particle.Y - particleRadius; };
                if (particle.X + particleRadius > maxX) { maxX = particle.X + particleRadius; };
                if (particle.Y + particleRadius > maxY) { maxY = particle.Y + particleRadius; };
            }

            maxX = Math.Max(_mainProbe.radius, maxX);
            minX = Math.Min(-1 * _mainProbe.radius, minX);
            maxY = Math.Max(_mainProbe.radius, maxY);
            minY = Math.Min(-1 * _mainProbe.radius, minY);

            // add padding to edges of screen, because of labels and toolbars
            float padding = 1;

            // get the correct scale
            float scaleX = clSize.Width / (maxX - minX + padding);
            float scaleY = clSize.Height / (maxY - minY + padding);
            _scale = Math.Min(scaleX, scaleY);
        }
        public void CenterOrigin(Size? clientSize = null)
        {
            if (clientSize == null)
            {
                clientSize = _curr_client_size;
            }

            Size clSize = (Size)clientSize;

            // set origin to the middle of the screen
            _origin = new Vector2((clSize.Width / 2), (clSize.Height / 2));
        }

        public void ResizeWindow(Size newClientSize)
        {
            float change_ratio_W = (float)newClientSize.Width / _curr_client_size.Width;
            float change_ratio_H = (float)newClientSize.Height / _curr_client_size.Height;

            _origin.X *= change_ratio_W;
            _origin.Y *= change_ratio_H;

            /*
            // the scaling should probably stay the same, this were my tries to get it done
            float scale_ratio = 1f;

            if (change_ratio_H > 1 && change_ratio_W > 1)
            {
                scale_ratio = Math.Min(change_ratio_W, change_ratio_H);
            }
            else if (change_ratio_H < 1 && change_ratio_W < 1)
            {
                scale_ratio = Math.Max(change_ratio_W, change_ratio_H);
            }
            // comment from here
            else if (change_ratio_H > 1 && change_ratio_W < 1)
            {
                scale_ratio = change_ratio_W;
            }
            else if (change_ratio_H < 1 && change_ratio_W > 1)
            {
                scale_ratio = change_ratio_W;
            }
            // to here - this was my last try, but it got bigger scale overtime
            _scale *= scale_ratio;
            */

            _curr_client_size = newClientSize;

            _grid_points = CalculateGridPoints();
            _bitmap_points = CalculateGridPoints(_bitmap_chunk_size);
        }

        /// <summary>
        /// Translate real-life coordinates into drawing coordinates.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Vector2(X, Y)</returns>
        public Vector2 TranslateCoordinates(float x, float y)
        {
            return TranslateCoordinates(new Vector2(x, y));
        }

        /// <summary>
        /// Translate real-life coordinates into drawing coordinates.
        /// </summary>
        /// <param name="realWorldCoords">Vector2(X, Y)</param>
        /// <returns>Vector2(X, Y)</returns>
        public Vector2 TranslateCoordinates(Vector2 realWorldCoords)
        {
            float x = _origin.X + realWorldCoords.X * _scale;
            float y = _origin.Y - realWorldCoords.Y * _scale;
            
            Vector2 drawingCoords = new Vector2(x,y);

            return drawingCoords;
        }

        public Vector2 GetRealWorldCoords(Vector2 drawingCoords)
        {
            float x = (drawingCoords.X - _origin.X) / _scale;
            float y = -1 * (drawingCoords.Y - _origin.Y) / _scale;

            return new Vector2(x,y);
        }

        /// <summary>
        /// Calculate the radius of a particle in real-life units. To convert to drawing-units multiply by _scale.
        /// </summary>
        /// <param name="particle">The particle to calculate radius to.</param>
        /// <returns>The radius in real-life units.</returns>
        public float CalculateParticleRadius(Particle particle)
        {
            float baseRadius = 0.1f;

            if (_particleDynamicWidth)
            {
                return baseRadius * (float)(1 + Math.Log(1 + Math.Abs(particle.Value)));
            }
            else
            {
                return baseRadius;
            }
        }


        #endregion CalculateAndTranslate


        #region Draw

        
        /// <summary>
        /// Main rendering loop. Renders all Particles and Probe.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clientSize"></param>
        public void Render(Graphics g, Size clientSize)
        {

            DrawBitmap(g);

            if (_showGrid)
            {
                DrawGrid(g, Grid_points);
            }
            if (_showStaticProbes)
            {
                DrawStaticProbes(g, Grid_points);
            }

            foreach (Particle particle in _particles)
            {
                DrawParticle(g, particle);
            }

            if (_secondProbe != null)
            {
                DrawProbe(g, _secondProbe);
            }

            DrawProbe(g, _mainProbe);

            DrawBitmapLegend(g);
        }

        



        /// <summary>
        /// Draw one particle as filled elipse with text value and color distinction for plus and minus.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="particle">Particle to draw.</param>
        private void DrawParticle(Graphics g, Particle particle)
        {
            // translate coordinates to drawing coordinates
            Vector2 particleCoords = TranslateCoordinates(particle.X, particle.Y);

            // set sizes
            float radius = CalculateParticleRadius(particle) * _scale;
            
            float fontSize = 0.08f * _scale;

            // set color, blue if negative value, red if positive
            Color particleColor = particle.Value > 0 ? _particlePositiveColor : _particleNegativeColor;

            // draw the particle
            using (Brush brush = new SolidBrush(particleColor))
            {
                g.FillEllipse(brush, particleCoords.X - radius, particleCoords.Y - radius, radius * 2, radius * 2);
            }

            // write a value next to (currently on the top) the particle
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Serif", fontSize))
            {
                string chargeLabel = $"{particle.Value:+0.00;-0.00} C";
                SizeF textSize = g.MeasureString(chargeLabel,font);

                // draw value background
                Rectangle rectangle = new Rectangle(
                    (int)(particleCoords.X - textSize.Width / 2),
                    (int)(particleCoords.Y - radius - textSize.Height),
                    (int)(textSize.Width),
                    (int)(textSize.Height)
                    );
                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 255, 255, 255)), rectangle);

                g.DrawString(chargeLabel, font, textBrush, particleCoords.X - textSize.Width / 2, particleCoords.Y - radius - textSize.Height);
            }

        }

        /// <summary>
        /// Draw the main Probe as an arrow and text with it's value.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="probe">Probe to draw.</param>
        private void DrawProbe(Graphics g, Probe probe)
        {
            Vector2 probeDir = FieldCalculator.CalculateFieldDirection(probe.position, _particles);
            DrawProbe(g, probe, probeDir);
        }

        /// <summary>
        /// Draw the main Probe as an arrow and text with it's value.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="probe">Probe to draw.</param>
        /// <param name="direction">Direction of the probe.</param>
        private void DrawProbe(Graphics g, Probe probe, Vector2 direction)
        {
            // translating into render coordinates
            Vector2 probeCoords = TranslateCoordinates(probe.position);

            Color probeColor = probe.color;

            // find the current energy of the Probe (arrow)
            float energy = FieldCalculator.CalculateFieldIntensity(direction);

            // dynamic arrow length
            if (energy > 0)
            {

                // linear scaling
                float divisor = 1E+09f;
                float minArrowLength = _scale * 0.15f;
                float maxArrowLength = _scale * 1f;
                float arrowLength = energy / divisor;

                arrowLength = Math.Clamp(arrowLength, minArrowLength, maxArrowLength);

                direction.X = (direction.X / energy) * arrowLength;
                direction.Y = (direction.Y / energy) * arrowLength * -1;

            }


            // draw value text
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Serif", _scale * 0.1f))
            {
                string chargeLabel = $"{energy.ToString("G3")} N/C";
                SizeF textSize = g.MeasureString(chargeLabel, font);

                // draw value background
                Rectangle rectangle = new Rectangle(
                    (int)(probeCoords.X - textSize.Width / 2),
                    (int)(probeCoords.Y + textSize.Height / 2),
                    (int)(textSize.Width),
                    (int)(textSize.Height)
                    );
                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 255, 255, 255)), rectangle);

                // draw text
                g.DrawString(chargeLabel, font, textBrush, probeCoords.X - textSize.Width / 2, probeCoords.Y + textSize.Height / 2);
            }


            // draw arrow
            using (Brush brush = new SolidBrush(probeColor))
            {
                Pen pen = new Pen(brush, _scale * 0.05f);
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                g.DrawLine(pen, new PointF(probeCoords), new PointF(probeCoords.X + direction.X, probeCoords.Y + direction.Y));
            }
        }

        /// <summary>
        /// OBSOLETE - only for debug.
        /// </summary>
        /// <param name="g"></param>
        private void DrawOrigin(Graphics g)
        {
            int biggnes = (int)(_scale * 0.1);
            g.FillEllipse(Brushes.Brown,new Rectangle((int)(_origin.X - biggnes/2),(int)(_origin.Y - biggnes/2),biggnes,biggnes));
        }

        #endregion Draw


        #region CustomizerForm
        // Create customizer form and subscribe and handle to all its Events.

        public void ShowCustomizerForm(Point where)
        {
            if (_customizerForm == null || _customizerForm.IsDisposed)
            {
                _customizerForm = new CustomizerForm(_mainProbe.color,_particlePositiveColor,_particleNegativeColor, _particleDynamicWidth, where);

                // Subscribe to the ColorChanged event
                _customizerForm.ProbeColorChanged += UpdateProbeColor;
                _customizerForm.ParticleDynamicWidthChecked += UpdateParticleDynamicWidth;
                _customizerForm.ParticlePositiveColorChanged += UpdateParticlePositiveColor;
                _customizerForm.ParticleNegativeColorChanged += UpdateParticleNegativeColor;
                _customizerForm.ZoomLevelChanged += UpdateZoomLevel;
                _customizerForm.ShowGridChanged += UpdateGridVisibility;
                _customizerForm.ShowStaticProbesChanged += UpdateStaticProbesVisibility;

                _customizerForm.Show();
            }
            _customizerForm.Activate();
            _customizerForm.Focus();
        }

        private void UpdateStaticProbesVisibility(bool obj)
        {
            _showStaticProbes = obj;
        }

        private void UpdateGridVisibility(bool obj)
        {
            _showGrid = obj;
        }

        private float _zooming_factor = 1f;
        public float ZoomingFactor
        {
            get { return _zooming_factor; }
        }
        private void UpdateZoomLevel(int new_zoom)
        {
            _zooming_factor = 1f * new_zoom;
        }

        private void UpdateParticleNegativeColor(Color color)
        {
            _particleNegativeColor = color;
        }

        private void UpdateParticlePositiveColor(Color color)
        {
            _particlePositiveColor = color;
        }

        private void UpdateParticleDynamicWidth(bool obj)
        {
            _particleDynamicWidth = obj;
        }

        private void UpdateProbeColor(Color color)
        {
            _mainProbe.color = color;
        }

        #endregion CustomizerForm


        #region Grid
        Point[,]? _grid_points = null;

        Point[,] Grid_points
        {
            get
            {
                if (_grid_points == null)
                {
                    _grid_points = CalculateGridPoints();
                }
                return _grid_points;
            }
            set
            {
                _grid_points = CalculateGridPoints();
            }
        }
        public int _grid_w;
        public int _grid_h;

        // Calculates canvas/simulation - not real coordinates
        public Point[,] CalculateGridPoints(int override_value = 0)
        {
            int vertical_offset = 0;// width_px / 2;
            int horizontal_offset = 0;// height_px / 2;

            int wid = _grid_w;
            int hig = _grid_h;
            if (override_value != 0)
            {
                wid = override_value;
                hig = override_value;
            }

            int vertical_count = (this._curr_client_size.Width + vertical_offset) / hig + 1 + 1; // +1 for integer division CEILING
            int horizontal_count = (this._curr_client_size.Height + horizontal_offset) / wid + 1 + 1; // second +1 for overflow - too have grid slightly bigger than canvas

            int max_w = this._curr_client_size.Width;
            int max_h = this._curr_client_size.Height;

            Point[,] points = new Point[horizontal_count,vertical_count];
            
            for (int i = 0; i < horizontal_count; i++)
            {
                for (int j = 0; j < vertical_count; j++)
                {
                    points[i, j] = new Point(j * wid + vertical_offset, i * hig + horizontal_offset);
                }
            }

            return points;
        }
        public void DrawGrid(Graphics g, Point[,] points)
        {
            Pen pen = new Pen(Brushes.Gray, 1f);

            int width = points.GetLength(0);
            int height = points.GetLength(1);
            for (int i = 0;i < width; i++)
            {
                g.DrawLine(pen, points[i, 0], points[i, height - 1]);
            }

            for (int i = 0; i < height; i++)
            {
                g.DrawLine(pen, points[0, i], points[width - 1, i]);
            }
        }

        public void DrawStaticProbes(Graphics g, Point[,] points)
        {
            int width = points.GetLength(0);
            int height = points.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    DrawStaticProbe(g, points[i, j]);
                }
            }
        }

        public void DrawStaticProbe(Graphics g, Point here)
        {
            Vector2 real_world_here = GetRealWorldCoords(new Vector2(here.X, here.Y));

            Vector2 vect = FieldCalculator.CalculateFieldDirection(real_world_here, _particles);
            float intensity = FieldCalculator.CalculateFieldIntensity(vect);

            Pen pen = new Pen(Color.DarkGray, 5);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            float len = Math.Min(_grid_w, _grid_h);

            Point endHere = new Point((int)(here.X + vect.X / intensity * len), (int)(here.Y - vect.Y / intensity * len));

            try
            {
                g.DrawLine(pen, here, endHere);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unreasonably big or small inputs.");
                Environment.Exit(1);
            }
            
        }


        #endregion Grid

        #region Bitmap

        /*private void DrawBitmap(Graphics g)
        {
            _bmp_pts_intensity = CalculateBitmapGridIntensity(Bitmap_points);

            int width = Bitmap_points.GetLength(0);
            int height = Bitmap_points.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color clr = ConvertIntensityToColor(_bmp_pts_intensity[x, y]);
                    Brush brush = new SolidBrush(clr);
                    Point pt = Bitmap_points[x, y];
                    g.FillRectangle(brush, pt.X, pt.Y, _bitmap_chunk_size, _bitmap_chunk_size);
                }
            }
        }*/

        /*private void DrawBitmap(Graphics g)
        {
            _bmp_pts_intensity = CalculateBitmapGridIntensity(Bitmap_points);

            int finalWidth = this._curr_client_size.Width;
            int finalHeight = this._curr_client_size.Height;
            float size = (float)_bitmap_chunk_size;

            using (Bitmap bmp = new Bitmap(finalWidth, finalHeight))
            using (Graphics bmpG = Graphics.FromImage(bmp))
            {
                bmpG.SmoothingMode = SmoothingMode.AntiAlias;

                for (int x = 0; x < Bitmap_points.GetLength(0); x++)
                {
                    for (int y = 0; y < Bitmap_points.GetLength(1); y++)
                    {
                        Color centerColor = ConvertIntensityToColor(_bmp_pts_intensity[x, y]);
                        Color transparentColor = Color.FromArgb(0, centerColor);
                        Point pt = Bitmap_points[x, y];

                        using (var gradient = new PathGradientBrush(new PointF[] {
                            new PointF(pt.X - size/2, pt.Y - size/2),
                            new PointF(pt.X + size/2, pt.Y - size/2),
                            new PointF(pt.X + size/2, pt.Y + size/2),
                            new PointF(pt.X - size/2, pt.Y + size/2)
                            }))
                        {
                            gradient.CenterColor = centerColor;
                            //gradient.SurroundColors = new Color[] { transparentColor };
                            gradient.FocusScales = new PointF(1f, 1f);
                            bmpG.FillRectangle(gradient,
                            pt.X - size / 2,
                            pt.Y - size / 2,
                                size,
                                size);
                        }
                    }
                }

                g.DrawImage(bmp, 0, 0);
            }
        }*/

        ////////////////////////////////////

        private void DrawBitmap(Graphics g)
        {
            _bmp_pts_intensity = CalculateBitmapGridIntensity(Bitmap_points);

            // Create a bitmap at a higher resolution than the grid
            int scaleFactor = 2; // Increase this for smoother output
            int finalWidth = this._curr_client_size.Width;
            int finalHeight = this._curr_client_size.Height;

            using (Bitmap bmp = new Bitmap(finalWidth, finalHeight))
            using (Graphics bmpG = Graphics.FromImage(bmp))
            {
                // Enable anti-aliasing
                bmpG.SmoothingMode = SmoothingMode.AntiAlias;
                bmpG.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Option 1: Draw smooth circles instead of rectangles
                for (int x = 0; x < Bitmap_points.GetLength(0); x++)
                {
                    for (int y = 0; y < Bitmap_points.GetLength(1); y++)
                    {
                        Color clr = ConvertIntensityToColor(_bmp_pts_intensity[x, y]);
                        using (Brush brush = new SolidBrush(Color.FromArgb(200, clr))) // Add some transparency
                        {
                            Point pt = Bitmap_points[x, y];
                            float size = _bitmap_chunk_size * 1.2f; // Slightly larger than chunk size for overlap
                            bmpG.FillRectangle(brush,
                                pt.X - size / 2,
                                pt.Y - size / 2,
                                size,
                                size);
                        }
                    }
                }
                /*
                // Option 2: Apply gaussian blur
                using (var blur = new GaussianBlur(bmp))
                {
                    blur.Sigma = 3; // Adjust blur amount
                    blur.Apply();
                }*/

                // Draw the final bitmap to the graphics object
                g.DrawImage(bmp, 0, 0);
            }
        }

        // Helper class for Gaussian Blur
        public class GaussianBlur
        {
            private readonly Bitmap _source;
            public float Sigma { get; set; } = 3.0f;

            public GaussianBlur(Bitmap source)
            {
                _source = source;
            }

            public void Apply()
            {
                int radius = (int)Math.Ceiling(Sigma * 3);
                var effect = new ColorMatrix();
                
                    var kernel = CreateGaussianKernel(radius, Sigma);

                    // Apply horizontal blur
                    ApplyConvolution(_source, kernel, true);

                    // Apply vertical blur
                    ApplyConvolution(_source, kernel, false);
                
            }

            private float[] CreateGaussianKernel(int radius, float sigma)
            {
                float[] kernel = new float[radius * 2 + 1];
                float sum = 0;

                for (int i = -radius; i <= radius; i++)
                {
                    kernel[i + radius] = (float)Math.Exp(-(i * i) / (2 * sigma * sigma));
                    sum += kernel[i + radius];
                }

                // Normalize
                for (int i = 0; i < kernel.Length; i++)
                {
                    kernel[i] /= sum;
                }

                return kernel;
            }

            private void ApplyConvolution(Bitmap bmp, float[] kernel, bool horizontal)
            {
                var bmpData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                int stride = bmpData.Stride;
                int bytes = Math.Abs(stride) * bmp.Height;
                byte[] rgba = new byte[bytes];
                byte[] result = new byte[bytes];

                Marshal.Copy(bmpData.Scan0, rgba, 0, bytes);

                // Apply convolution
                Parallel.For(0, bmp.Height, y =>
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        float r = 0, g = 0, b = 0, a = 0;
                        int index = y * stride + x * 4;

                        for (int k = -kernel.Length / 2; k <= kernel.Length / 2; k++)
                        {
                            int px = horizontal ? Math.Min(Math.Max(x + k, 0), bmp.Width - 1) : x;
                            int py = horizontal ? y : Math.Min(Math.Max(y + k, 0), bmp.Height - 1);
                            int idx = py * stride + px * 4;
                            float weight = kernel[k + kernel.Length / 2];

                            b += rgba[idx] * weight;
                            g += rgba[idx + 1] * weight;
                            r += rgba[idx + 2] * weight;
                            a += rgba[idx + 3] * weight;
                        }

                        result[index] = (byte)Math.Min(255, Math.Max(0, b));
                        result[index + 1] = (byte)Math.Min(255, Math.Max(0, g));
                        result[index + 2] = (byte)Math.Min(255, Math.Max(0, r));
                        result[index + 3] = (byte)Math.Min(255, Math.Max(0, a));
                    }
                });

                Marshal.Copy(result, 0, bmpData.Scan0, bytes);
                bmp.UnlockBits(bmpData);
            }
        }










        ////////////////////////////////////

        private void DrawBitmapLegend(Graphics g)
        {
            // TODO
        }

        private Color ConvertIntensityToColor(float intensity)
        {
            double midpoint = 0.0;
            double scale = 10.0;
            double value = (double) intensity;
            double normalized = 1.0 / (1.0 + Math.Exp(-(value - midpoint) / scale));

            double hue = 240 - (normalized * 240);
            double saturation = 1.0;
            double value_hsv = 1.0;

            return HSVToRGB(hue, saturation, value_hsv);
        }


        private static Color HSVToRGB(double hue, double saturation, double value)
        {
            double c = value * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = value - c;

            double r, g, b;
            if (hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return Color.FromArgb(
                (int)((r + m) * 255),
                (int)((g + m) * 255),
                (int)((b + m) * 255));
        }

        Point[,]? _bitmap_points = null;

        Point[,] Bitmap_points
        {
            get
            {
                if (_bitmap_points == null)
                {
                    _bitmap_points = CalculateGridPoints(_bitmap_chunk_size);
                }
                return _bitmap_points;
            }
            set
            {
                _bitmap_points = CalculateGridPoints(_bitmap_chunk_size);
            }
        }

        float[,]? _bmp_pts_intensity = null;

        float[,] Bitmap_points_intensity
        {
            get
            {
                if (_bmp_pts_intensity == null)
                {
                    _bmp_pts_intensity = CalculateBitmapGridIntensity(Bitmap_points);
                }
                return _bmp_pts_intensity;
            }
            set
            {
                _bmp_pts_intensity = CalculateBitmapGridIntensity(Bitmap_points);
            }
        }

        float[,] CalculateBitmapGridIntensity(Point[,] points)
        {
            int width = points.GetLength(0);
            int height = points.GetLength(1);

            float[,] res = new float[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pt = points[x, y];
                    Vector2 sim_coords = new Vector2(pt.X, pt.Y);
                    var real_coords = GetRealWorldCoords(sim_coords);

                    Vector2 dir = FieldCalculator.CalculateFieldDirection(
                            real_coords,
                            _particles
                        );

                    res[x, y] = FieldCalculator.CalculateFieldIntensity(dir) / 1E+09f;

                    Particle? nearest_particle = GetNearestParticle(real_coords);

                    if (nearest_particle == null)
                    {
                        continue;
                    }
                    res[x,y] *= nearest_particle.Value > 0 ? 1f : -1f;
                }
            }

            return res;
        }

        Particle? GetNearestParticle(Vector2 realLifeCoords)
        {
            Particle res = null;
            double distance = double.MaxValue;
            foreach (var particle in _particles)
            {
                double curr_dist = Math.Sqrt(
                    Math.Pow(realLifeCoords.X - particle.X, 2) +
                    Math.Pow(realLifeCoords.Y - particle.Y, 2)
                );

                if (curr_dist < distance)
                {
                    distance = curr_dist;
                    res = particle;
                }
            }

            

            return res;
        }
    
        #endregion Bitmap
    }
}
