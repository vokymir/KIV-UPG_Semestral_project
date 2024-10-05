using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.View
{
    public class Renderer
    {
        private List<Particle> _particles;
        private Probe _mainProbe;

        public Renderer(List<Particle> particles, Probe probe)
        {
            this._particles = particles;
            this._mainProbe = probe;
        }

        public void Render(Graphics g, Size clientSize)
        {
            float pixl = clientSize.Width / 100;
            Pen pen = new Pen(Color.Red, 16f);
            g.DrawEllipse(pen, 8, 8, 40 * pixl, 40 * pixl);
        }

    }
}
