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
            float screenX = _origin.X + particle.X * _scale;
            float screenY = _origin.Y - particle.Y * _scale;

            // set sizes
            float baseRadius = 0.1f * _scale;
            float radius = baseRadius * (float)(1 + Math.Log( Math.Abs(particle.Value)));
            float fontSize = baseRadius;

            // set color, blue if negative value, red if positive
            Color particleColor = particle.Value > 0 ? Color.Red : Color.Blue;

            // draw the particle
            using (Brush brush = new SolidBrush(particleColor))
            {
                g.FillEllipse(brush,screenX - radius, screenY - radius, radius * 2, radius * 2);
            }

            // write a value next to (currently on the top) the particle
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Serif", fontSize))
            {
                string chargeLabel = $"{particle.Value:+0;-0} C";
                SizeF textSize = g.MeasureString(chargeLabel,font);
                g.DrawString(chargeLabel, font, textBrush, screenX - textSize.Width / 2, screenY - radius - textSize.Height);
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
            float screenX = _origin.X + probe.position.X * _scale;
            float screenY = _origin.Y - probe.position.Y * _scale;

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
                g.DrawLine(pen,new PointF(screenX,screenY),new PointF(screenX + direction.X, screenY + direction.Y));
            }

            // draw value text
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Serif", _scale * 0.1f))
            {
                string chargeLabel = $"{energy.ToString("G3")} N/C";
                SizeF textSize = g.MeasureString(chargeLabel, font);

                // draw value background
                Rectangle rectangle = new Rectangle(
                    (int)(screenX - textSize.Width / 2),
                    (int)(screenY + textSize.Height / 2),
                    (int)(textSize.Width),
                    (int)(textSize.Height)
                    );
                g.FillRectangle(new SolidBrush(Color.FromArgb(128, 255, 255, 255)), rectangle);

                // draw text
                g.DrawString(chargeLabel, font, textBrush, screenX - textSize.Width / 2, screenY + textSize.Height / 2);
            }
        }

       

    }
}
