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
        #region declarations

        public List<Particle> particles;
        public Probe mainProbe;

        private CustomizerForm? _customizerForm;
        private bool _particleDynamicWidth = true;
        private Color _particlePositiveColor = Color.Red;
        private Color _particleNegativeColor = Color.Blue;
        private Size _curr_client_size = new Size(800, 600);
        private FieldColorMapper _fcm = new FieldColorMapper(0, 1E+2, FieldColorMapper.ColorScale.Thermal);
        private float[,]? _bmp_pts_intensity = null;
        private Point[,]? _bitmap_points = null;
        private int _bitmap_chunk_size = 4;
        private Image _particle_image;
        private bool _showGrid = true;
        private bool _showStaticProbes = true;
        private float _scale = 1f;
        private float _zooming_factor = 1f;
        private Vector2 _origin = Vector2.Zero;

        Point[,]? _grid_points = null;

        public bool funMode = false;
        public event Action ColorScaleChanged;
        public FieldColorMapper.ColorScale CS
        {
            get { return _fcm.Color_scale; }
            set
            {
                _fcm.Color_scale = value;
            }
        }
        public FieldColorMapper FCM { get { return _fcm; } }
        public HashSet<(Probe, GraphForm)> otherProbes = new HashSet<(Probe, GraphForm)> { };
        public int grid_w;
        public int grid_h;
        public Point[,] Bitmap_points
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
        public float[,] Bitmap_points_intensity
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
        public Point[,] Grid_points
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
        public float ZoomingFactor
        {
            get { return _zooming_factor; }
        }

        #endregion declarations

        /// <summary>
        /// Init the rendere and also set correct scaling, size and grid.
        /// </summary>
        /// <param name="particles">List of particles to render.</param>
        /// <param name="probe">Probe to render.</param>
        /// <param name="clientSize">Client size.</param>
        public Renderer(List<Particle> particles, Probe probe, Size clientSize, int grid_w, int grid_h)
        {
            this.particles = particles;
            this.mainProbe = probe;
            this.grid_w = grid_w;
            this.grid_h = grid_h;

            _curr_client_size = clientSize;

            // for FunMode
            _particle_image = Image.FromFile("Images/kohout.png");

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

            foreach (Particle particle in particles)
            {
                float particleRadius = CalculateParticleRadius(particle);

                if (particle.X - particleRadius < minX) { minX = particle.X - particleRadius; };
                if (particle.Y - particleRadius < minY) { minY = particle.Y - particleRadius; };
                if (particle.X + particleRadius > maxX) { maxX = particle.X + particleRadius; };
                if (particle.Y + particleRadius > maxY) { maxY = particle.Y + particleRadius; };
            }

            maxX = Math.Max(mainProbe.radius, maxX);
            minX = Math.Min(-1 * mainProbe.radius, minX);
            maxY = Math.Max(mainProbe.radius, maxY);
            minY = Math.Min(-1 * mainProbe.radius, minY);

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

            foreach (Particle particle in particles)
            {
                DrawParticle(g, particle);
            }

            foreach ((Probe, GraphForm) probeTwin in otherProbes)
            {
                DrawProbe(g, probeTwin.Item1);
            }

            DrawProbe(g, mainProbe);
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

            #region particle
            // set sizes
            float radius = CalculateParticleRadius(particle) * _scale;
            
            float fontSize = 0.08f * _scale;

            // set color, blue if negative value, red if positive
            Color particleColor = particle.Value > 0 ? _particlePositiveColor : _particleNegativeColor;

            // draw the particle
            using (Brush brush = new SolidBrush(particleColor))
            {
                if (funMode)
                {
                    g.DrawImage(_particle_image, particleCoords.X - radius, particleCoords.Y - radius, radius * 2, radius * 2);
                }
                else
                {
                    g.FillEllipse(brush, particleCoords.X - radius, particleCoords.Y - radius, radius * 2, radius * 2);
                }
            }

            #endregion particle

            #region text value

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

            #endregion text value
        }

        /// <summary>
        /// Draw any Probe as an arrow and text with it's value.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="probe">Probe to draw.</param>
        private void DrawProbe(Graphics g, Probe probe)
        {
            Vector2 probeDir = FieldCalculator.CalculateFieldDirection(probe.position, particles);
            DrawProbe(g, probe, probeDir);
        }

        private void DrawProbe(Graphics g, Probe probe, Vector2 direction)
        {
            #region math

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

            #endregion math

            #region text

            // draw value text
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Serif", _scale * 0.1f))
            {
                string chargeLabel = $"{(probe != mainProbe ? probe.ID.ToString()+": " : "")}{energy.ToString("G3")} N/C";
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

            #endregion text

            #region arrow

            // draw arrow
            using (Brush brush = new SolidBrush(probeColor))
            {
                Pen pen = new Pen(brush, _scale * 0.05f);
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                if (probe != mainProbe) 
                { // for other probes than main
                    pen.StartCap = LineCap.RoundAnchor;
                }
                g.DrawLine(pen, new PointF(probeCoords), new PointF(probeCoords.X + direction.X, probeCoords.Y + direction.Y));
            }

            #endregion arrow
        }

        #endregion Draw

        #region CustomizerForm

        // Create customizer form and subscribe and handle to all its Events.
        public void ShowCustomizerForm(Point where)
        {
            if (_customizerForm == null || _customizerForm.IsDisposed)
            {
                _customizerForm = new CustomizerForm(mainProbe.color,_particlePositiveColor,_particleNegativeColor, _particleDynamicWidth, where, _fcm.Color_scale);

                // Subscribe to the ColorChanged event
                _customizerForm.ProbeColorChanged += UpdateProbeColor;
                _customizerForm.ParticleDynamicWidthChecked += UpdateParticleDynamicWidth;
                _customizerForm.ParticlePositiveColorChanged += UpdateParticlePositiveColor;
                _customizerForm.ParticleNegativeColorChanged += UpdateParticleNegativeColor;
                _customizerForm.ZoomLevelChanged += UpdateZoomLevel;
                _customizerForm.ShowGridChanged += UpdateGridVisibility;
                _customizerForm.ShowStaticProbesChanged += UpdateStaticProbesVisibility;
                _customizerForm.ColorScaleChanged += UpdateColorScale;

                _customizerForm.Show();
            }
            _customizerForm.Activate();
            _customizerForm.Focus();
        }

        private void UpdateColorScale(FieldColorMapper.ColorScale scale)
        {
            _fcm.Color_scale = scale;
            ColorScaleChanged?.Invoke();
        }

        private void UpdateStaticProbesVisibility(bool obj)
        {
            _showStaticProbes = obj;
        }

        private void UpdateGridVisibility(bool obj)
        {
            _showGrid = obj;
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
            mainProbe.color = color;
        }

        #endregion CustomizerForm

        #region Grid

        // Calculates canvas/simulation grid points IN DRAWING COORDS - not real coordinates
        public Point[,] CalculateGridPoints(int override_value = 0)
        {
            int vertical_offset = 0;// width_px / 2;
            int horizontal_offset = 0;// height_px / 2;

            int wid = grid_w;
            int hig = grid_h;
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

            Vector2 vect = FieldCalculator.CalculateFieldDirection(real_world_here, particles);
            float intensity = FieldCalculator.CalculateFieldIntensity(vect);

            Pen pen = new Pen(Color.DarkGray, 5);
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            float len = Math.Min(grid_w, grid_h);

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

        private void DrawBitmap(Graphics g)
        {
            int width = Bitmap_points.GetLength(0);
            int height = Bitmap_points.GetLength(1);

            int canvas_w = this._curr_client_size.Width;
            int canvas_h = this._curr_client_size.Height;

            // bitmap correct dimensions
            Bitmap bitmap = new Bitmap(canvas_w, canvas_h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // lock for unsafe 
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, canvas_w, canvas_h),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            unsafe
            { // vey C-like, ain't it?
                byte* ptr = (byte*)bmpData.Scan0;
                _bmp_pts_intensity = CalculateBitmapGridIntensity(Bitmap_points); // so it is actual in every frame

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color clr = _fcm.ConvertIntensityToColor(_bmp_pts_intensity[x, y]);

                        // jump by four
                        int offset = (x * bmpData.Stride) + (y * 4);

                        // set pixel (BGRA format)
                        ptr[offset] = clr.B;     // B
                        ptr[offset + 1] = clr.G; // G
                        ptr[offset + 2] = clr.R; // R
                        ptr[offset + 3] = clr.A; // Alpha
                    }
                }
            }

            // unlock
            bitmap.UnlockBits(bmpData);

            // draw on graphics
            g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width * _bitmap_chunk_size, bitmap.Height * _bitmap_chunk_size));

            // better get rid of
            bitmap.Dispose();
        }

        // for given points calculate intensity - from 2D array to 1D
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
                            particles
                        );

                    res[x, y] = FieldCalculator.CalculateFieldIntensity(dir) / 1E+09f;
                }
            }

            return res;
        }

        #endregion Bitmap
    }
}
