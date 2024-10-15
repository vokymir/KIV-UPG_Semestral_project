using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    /// <summary>
    /// Probe has it's current Position,
    /// Radius on which it orbit and
    /// "speed" angularVelocity.
    /// </summary>
    public class Probe
    {
        public Vector2 Position { get; private set; }
        public float Radius {  get; private set; }
        private float _angularVelocity = (float)(Math.PI / 6); // specified in assignment

        public Probe()
        {
            Radius = 1;
            Position = new Vector2(Radius,0f);
        }

        /// <summary>
        /// Updates it's position over time, orbiting around the origin.
        /// </summary>
        /// <param name="time">Time elapsed since the beginning.</param>
        public void UpdatePosition(float time)
        {
            float angle = _angularVelocity * time;
            Position = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
        }

    }
}
