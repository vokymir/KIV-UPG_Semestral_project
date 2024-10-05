using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    public class Utility
    {
        public static float Interpolate(float x, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            float scaledValue = ((x - minInput) * (maxOutput - minOutput) + minOutput) / (maxInput  - minInput);

            return scaledValue;
        }
    }
}
