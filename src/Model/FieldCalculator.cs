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
        private const float K = 8.988e9f;

        /// <summary>
        /// Calculate the direction of the force for the point with given particles.
        /// </summary>
        /// <param name="point">Point for which calculate the direction.</param>
        /// <param name="particles">List of every particle in the scene.</param>
        /// <returns>The direction vector.</returns>
        public static Vector2 CalculateFieldDirection(Vector2 point, List<Particle> particles)
        {
            Vector2 direction = new Vector2(0, 0);

            // follow the formula from the assignment for better understanding: https://courseware.zcu.cz/CoursewarePortlets2/DownloadDokumentu?id=238441
            // this is the SUMMA part
            foreach (Particle particle in particles)
            {
                float dx = particle.X - point.X;
                float dy = particle.Y - point.Y;
                //float dx = point.X - particle.X;
                //float dy = point.Y - particle.Y;
                float distanceSquared = dx * dx + dy * dy;
                float distance = (float)Math.Sqrt(distanceSquared);

                // won't divide by zero
                if (distanceSquared == 0)
                {
                    continue;
                }

                float fieldMagnitude = particle.Value / distanceSquared;

                float ex = dx / distance;
                float ey = dy / distance;

                direction.X += fieldMagnitude * ex;
                direction.Y += fieldMagnitude * ey;
            }

            // this is the multiplication of the summa
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
