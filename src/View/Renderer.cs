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
        private List<Particle> _particles;
        private Probe _mainProbe;
        private CustomizerForm? _customizerForm;
        private bool _particleDynamicWidth = true;
        private Color _particlePositiveColor = Color.Red;
        private Color _particleNegativeColor = Color.Blue;
        private Size _curr_client_size = new Size(800, 600);

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
        public Renderer(List<Particle> particles, Probe probe, Size clientSize)
        {
            this._particles = particles;
            this._mainProbe = probe;

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
        }

        /// <summary>
        /// Translate real-life coordinates into drawing coordinates.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Vector2(X, Y)</returns>
        private Vector2 TranslateCoordinates(float x, float y)
        {
            return TranslateCoordinates(new Vector2(x, y));
        }

        /// <summary>
        /// Translate real-life coordinates into drawing coordinates.
        /// </summary>
        /// <param name="realWorldCoords">Vector2(X, Y)</param>
        /// <returns>Vector2(X, Y)</returns>
        private Vector2 TranslateCoordinates(Vector2 realWorldCoords)
        {
            float x = _origin.X + realWorldCoords.X * _scale;
            float y = _origin.Y - realWorldCoords.Y * _scale;
            
            Vector2 drawingCoords = new Vector2(x,y);

            return drawingCoords;
        }

        /// <summary>
        /// Calculate the radius of a particle in real-life units. To convert to drawing-units multiply by _scale.
        /// </summary>
        /// <param name="particle">The particle to calculate radius to.</param>
        /// <returns>The radius in real-life units.</returns>
        private float CalculateParticleRadius(Particle particle)
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
        public void Render(Graphics g, Size clientSize, Vector2 probeDirection)
        { 

            foreach (Particle particle in _particles)
            {
                DrawParticle(g, particle);
            }

            DrawProbe(g, _mainProbe, probeDirection);
        }
        /// <summary>
        /// Main rendering loop. Renders all Particles and Probe.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clientSize"></param>
        public void Render(Graphics g, Size clientSize)
        {
            foreach (Particle particle in _particles)
            {
                DrawParticle(g, particle);
            }

            DrawProbe(g, _mainProbe);

            DrawOrigin(g);
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
            
            float fontSize = 0.1f * _scale;

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
                string chargeLabel = $"{particle.Value:+0;-0} C";
                SizeF textSize = g.MeasureString(chargeLabel,font);
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

            // draw arrow
            using (Brush brush = new SolidBrush(probeColor))
            {
                Pen pen = new Pen(brush, _scale * 0.05f);
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                g.DrawLine(pen,new PointF(probeCoords),new PointF(probeCoords.X + direction.X, probeCoords.Y + direction.Y));
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
                g.FillRectangle(new SolidBrush(Color.FromArgb(128, 255, 255, 255)), rectangle);

                // draw text
                g.DrawString(chargeLabel, font, textBrush, probeCoords.X - textSize.Width / 2, probeCoords.Y + textSize.Height / 2);
            }
        }

        private void DrawOrigin(Graphics g)
        {
            int biggnes = (int)(_scale * 0.1);
            g.FillEllipse(Brushes.Brown,new Rectangle((int)(_origin.X - biggnes/2),(int)(_origin.Y - biggnes/2),biggnes,biggnes));
        }

        #endregion Draw


        #region CustomizerForm
        // Create customizer form and subscribe and handle to all its Events.

        public void ShowCustomizerForm()
        {
            if (_customizerForm == null || _customizerForm.IsDisposed)
            {
                _customizerForm = new CustomizerForm(_mainProbe.color,_particlePositiveColor,_particleNegativeColor, _particleDynamicWidth);

                // Subscribe to the ColorChanged event
                _customizerForm.ProbeColorChanged += UpdateProbeColor;
                _customizerForm.ParticleDynamicWidthChecked += UpdateParticleDynamicWidth;
                _customizerForm.ParticlePositiveColorChanged += UpdateParticlePositiveColor;
                _customizerForm.ParticleNegativeColorChanged += UpdateParticleNegativeColor;
                _customizerForm.ZoomLevelChanged += UpdateZoomLevel;


                _customizerForm.Show();
            }
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


    }
}
