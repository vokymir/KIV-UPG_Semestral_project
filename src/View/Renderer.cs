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
        private float _scale;
        private Vector2 _origin;

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
            this._scale = 1.0f;
            this._origin = Vector2.Zero;
            UpdateOnResize(clientSize);
        }

        /// <summary>
        /// Updating the scaling parameters on resize
        /// </summary>
        /// <param name="clientSize"></param>
        public void UpdateOnResize(Size clientSize)
        {
            // finding the min and max values in both axis
            float minX = 0;
            float minY = 0;
            float maxX = 0;
            float maxY = 0;

            foreach (Particle particle in _particles)
            {   // not considering the particle radius
                if (particle.X < minX) minX = particle.X;
                if (particle.Y < minY) minY = particle.Y;
                if (particle.X > maxX) maxX = particle.X;
                if (particle.Y > maxY) maxY = particle.Y;
            }

            maxX = Math.Max(_mainProbe.radius, maxX);
            minX = Math.Min(-1 * _mainProbe.radius, minX);
            maxY = Math.Max(_mainProbe.radius, maxY);
            minY = Math.Min(-1 * _mainProbe.radius, minY);

            // add padding to edges of screen, so the biggest particles don't touch the edge
            float padding = 2;

            // get the correct scale
            float scaleX = clientSize.Width / (maxX - minX + padding);
            float scaleY = clientSize.Height / (maxY - minY + padding);
            _scale = Math.Min(scaleX, scaleY);

            // set origin to the middle of the screen
            _origin = new Vector2((clientSize.Width / 2), (clientSize.Height / 2));

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
            float baseRadius = 0.1f * _scale;
            float radius = baseRadius * (float)(1 + Math.Log( Math.Abs(particle.Value)));
            float fontSize = baseRadius;

            // set color, blue if negative value, red if positive
            Color particleColor = particle.Value > 0 ? Color.Red : Color.Blue;

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
            // translating into render coordinates
            Vector2 probeCoords = TranslateCoordinates(probe.position);

            Color probeColor = probe.color;

            // find the current force direction and energy of the Probe (arrow)
            Vector2 direction = FieldCalculator.CalculateFieldDirection(probe.position, _particles);
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

                direction.X = (direction.X / energy) * arrowLength * -1;
                direction.Y = (direction.Y / energy) * arrowLength;

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

       

    }
}
