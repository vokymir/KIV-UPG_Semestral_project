using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    public class Probe
    {
        public Vector2 Position { get; private set; }

        public Probe()
        {
            Position = new Vector2(1f,0f);
        }

        public void UpdatePosition(float time)
        {
            float angularVelocity = (float)(Math.PI / 6); // specified in assignment
            float angle = angularVelocity * time;
            Position = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

    }
}
