using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    public class FieldCalculator
    {
        private const float K = 8.988e9f;

        public static Vector2 CalculateField(Vector2 point, List<Particle> particles)
        {
            Vector2 field = new Vector2(0, 0);

            foreach (Particle particle in particles)
            {
                float dx = particle.X - point.X;
                float dy = particle.Y - point.Y;
                float distanceSquared = dx * dx + dy * dy;
                float distance = (float)Math.Sqrt(distanceSquared);

                if (distanceSquared == 0)
                {
                    continue;
                }

                float fieldMagnitude = particle.Value / distanceSquared;

                float ex = dx / distance;
                float ey = dy / distance;

                field.X += fieldMagnitude * ex;
                field.Y += fieldMagnitude * ey;
            }

            field *= K;

            return field;
        }
    }
}
