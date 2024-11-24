using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private int _bitmap_chunk_size = 30;

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

        private void DrawBitmap(Graphics g)
        {

        }

        private void DrawBitmapLegend(Graphics g)
        {
            // TODO
        }

        private Color ConvertIntensityToColor(float intensity)
        {


            return Color.Wheat;
        }

        Point[,]? _bitmap_points = null;

        Point[,] Bitmap_points
        {
            get
            {
                if (_bitmap_points == null)
                {
                    _bitmap_points = CalculateGridPoints();
                }
                return _bitmap_points;
            }
            set
            {
                _bitmap_points = CalculateGridPoints();
            }
        }

        #endregion Bitmap
    }
}
