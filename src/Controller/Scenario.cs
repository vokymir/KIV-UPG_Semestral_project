using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Controller
{
    public class Scenario
    {
        public static List<Particle> LoadScenario(int scenarioNumber)
        {
            switch (scenarioNumber)
            {
                case 1:
                    return new List<Particle>
                    {
                        new Particle(-1f, 0f, 1f),
                    new Particle(1f, 0f, 1f)
                };
                case 2:
                    return new List<Particle>
                    {
                    new Particle(-1f, 0f, -1f),
                    new Particle(1f, 0f, 2f)
                };
                case 3:
                    return new List<Particle>
                    {
                    new Particle(-1f, -1f, 1f),
                    new Particle(1f, -1f, 2f),
                    new Particle(1f, 1f, -3f),
                    new Particle(-1f, 1f, -4f)
                };
                default:
                    return new List<Particle>
                {
                    new Particle(0f, 0f, 1f)
                };
            }
        }
    }
}
