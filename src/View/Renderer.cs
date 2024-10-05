using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.View
{
    public class Renderer
    {
        private List<Particle> _particles;
        private Probe _mainProbe;
        private float scale;
        private Vector2 origin;
        System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

        public Renderer(List<Particle> particles, Probe probe, Size clientSize)
        {
            this._particles = particles;
            this._mainProbe = probe;
            this.scale = 1.0f;
            this.origin = Vector2.Zero;
            UpdateOnResize(clientSize);
        }

        public void UpdateOnResize(Size clientSize)
        {
            float minX = 0;
            float minY = 0;
            float maxX = 0;
            float maxY = 0;

            foreach (Particle particle in _particles)
            {
                if (particle.X < minX) minX = particle.X;
                if (particle.Y < minY) minY = particle.Y;
                if (particle.X > maxX) maxX = particle.X;
                if (particle.Y > maxY) maxY = particle.Y;
            }

            maxX = Math.Max(_mainProbe.Radius, maxX);
            minX = Math.Min(-1 * _mainProbe.Radius, minX);
            maxY = Math.Max(_mainProbe.Radius, maxY);
            minY = Math.Min(-1 * _mainProbe.Radius, minY);

            float padding = 2;

            float scaleX = clientSize.Width / (maxX - minX + padding);
            float scaleY = clientSize.Height / (maxY - minY + padding);
            scale = Math.Min(scaleX, scaleY);

            origin = new Vector2((clientSize.Width / 2), (clientSize.Height / 2));
        }

        public void Render(Graphics g, Size clientSize)
        {
            foreach (Particle particle in _particles)
            {
                DrawParticle(g, particle);
            }

            DrawProbe(g, _mainProbe);
        }

        private void DrawParticle(Graphics g, Particle particle)
        {
            float screenX = origin.X + particle.X * scale;
            float screenY = origin.Y - particle.Y * scale;

            float baseRadius = 0.1f * scale;
            float radius = baseRadius * (float)(1 + Math.Log( Math.Abs(particle.Value)));
            float fontSize = baseRadius;

            Color particleColor = particle.Value > 0 ? Color.Red : Color.Blue;

            using (Brush brush = new SolidBrush(particleColor))
            {
                g.FillEllipse(brush,screenX - radius, screenY - radius, radius * 2, radius * 2);
            }

            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Serif", fontSize))
            {
                string chargeLabel = $"{particle.Value:+0;-0} C";
                SizeF textSize = g.MeasureString(chargeLabel,font);
                g.DrawString(chargeLabel, font, textBrush, screenX - textSize.Width / 2, screenY - radius - textSize.Height);
            }

        }

        private void DrawProbe(Graphics g, Probe probe)
        {
            float screenX = origin.X + probe.Position.X * scale;
            float screenY = origin.Y - probe.Position.Y * scale;

            Color probeColor = Color.Black;

            Vector2 direction = FieldCalculator.CalculateFieldDirection(probe.Position, _particles);
            float energy = FieldCalculator.CalculateFieldIntensity(direction);

            float arrowLength = 1 / (scale * 10000000);
            direction.X *= arrowLength;
            direction.Y *= arrowLength * -1;

            using (Brush brush = new SolidBrush(probeColor))
            {
                Pen pen = new Pen(brush, scale * 0.05f);
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                g.DrawLine(pen,new PointF(screenX,screenY),new PointF(screenX + direction.X, screenY + direction.Y));
            }

            
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font font = new Font("Serif", scale * 0.1f))
            {
                string chargeLabel = $"{energy.ToString():+0;-0} C";
                SizeF textSize = g.MeasureString(chargeLabel, font);
                g.DrawString(chargeLabel, font, textBrush, screenX - textSize.Width / 2, screenY - textSize.Height);
            }
        }

    }
}
