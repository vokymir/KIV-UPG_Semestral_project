using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ElectricFieldVis.Controller
{
    public class Scenario
    {
        public List<Particle>? particles { get; set; }
        public Scenario() { }
        public static List<Particle> LoadScenario(int scenarioNumber)
        {
            string filename = $"Scenarios/{scenarioNumber}.json";
            string jsonString = File.ReadAllText(filename);
            Scenario scenario = new Scenario();
            try
            {
                scenario = JsonSerializer.Deserialize<Scenario>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Application.Exit();
            }


            return scenario.particles;
        }
    }
}
