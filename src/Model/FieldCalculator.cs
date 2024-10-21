using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    /// <summary>
    /// Calculating the force of the field.
    /// </summary>
    public class FieldCalculator
    {
        // the constant when multiplying the summa in CalculateFieldDirection function
        // 1/(4 * PI * e_0)
        private const float K = (float)(1f / ( 4f * (float)Math.PI * 8.854E-12));

        /// <summary>
        /// Calculate the direction of the force for the point with given particles.
        /// </summary>
        /// <param name="point">Point for which calculate the direction.</param>
        /// <param name="particles">List of every particle in the scene.</param>
        /// <returns>The direction vector.</returns>
        public static Vector2 CalculateFieldDirection(Vector2 point, List<Particle> particles)
        {
            Vector2 direction = new Vector2(0, 0);

            // follow the formula from the assignment for better understanding: Project_folder > doc > Zadani_semestralni_prace_KIV_UPG_2024_2025.pdf
            // this is the SUMMA part
            foreach (Particle particle in particles)
            {
                // x - x_i
                float dx = point.X - particle.X;
                float dy = point.Y - particle.Y;
                
                // || x - x_i ||^2
                float distanceSquared = dx * dx + dy * dy;

                // || x - x_i ||
                float distance = (float)Math.Sqrt(distanceSquared);

                // won't divide by zero
                if (distanceSquared == 0)
                {
                    continue;
                }

                // q_i / || x - x_i ||^2
                float fieldMagnitude = particle.Value / distanceSquared;

                // (x - x_i) / || x - x_i ||
                float ex = dx / distance;
                float ey = dy / distance;

                // add to summa: [ q_i / || x - x_i ||^2 ] * [ (x - x_i) / || x - x_i || ]
                // which is what we want
                direction.X += fieldMagnitude * ex;
                direction.Y += fieldMagnitude * ey;
            }

            // this is the multiplication of the summa
            // 1/(4 * PI * e_0) * Summa
            direction *= K;

            return direction;
        }

        /// <summary>
        /// When you have the direction, you want the intensity.
        /// </summary>
        /// <param name="direction">Direction of the field in some point.</param>
        /// <returns>The intensity in that point.</returns>
        public static float CalculateFieldIntensity(Vector2 direction)
        {
            float intensitySquared = direction.X * direction.X + direction.Y * direction.Y;
            float intensity = (float)Math.Sqrt(intensitySquared);

            return intensity;
        }
    }
}
