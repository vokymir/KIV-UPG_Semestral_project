using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    /// <summary>
    /// For storing all information about one Particle.
    /// </summary>
    public class Particle
    {
        public float X {  get; set; }
        public float Y { get; set; }
        public float Value { get; set; }

        public Particle(float x, float y, float value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }
}
