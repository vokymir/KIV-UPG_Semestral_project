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
        public float Radius {  get; private set; }
        private float angularVelocity = (float)(Math.PI / 6); // specified in assignment

        public Probe()
        {
            Radius = 1;
            Position = new Vector2(Radius,0f);
        }

        public void UpdatePosition(float time)
        {
            float angle = angularVelocity * time;
            Position = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
        }

    }
}
