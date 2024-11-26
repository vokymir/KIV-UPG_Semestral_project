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
    /// <summary>
    /// In charge of loading and saving scenarios. The attributes depends on the extensions I will make.
    /// </summary>
    public class Scenario
    {
        public bool isDefault {  get; set; }
        public List<Particle>? particles { get; set; }
        public Scenario() { }

        /// <summary>
        /// Load the desired scenario, or default scenario 0 if there is any problem. Message the problem.
        /// </summary>
        /// <param name="scenarioName">Name of the desired scenario</param>
        /// <returns>Complete scenario.</returns>
        public static Scenario LoadScenario(string scenarioName)
        {
            string filename = $"Scenarios/{scenarioName}.json";
            string jsonString;

            // catch invalid filename
            try
            {
                jsonString = File.ReadAllText(filename);
            }
            catch
            {
                
                return Scenario.DefaultLoad($"File for desired scenario not found.\n> Scenario name:\n{scenarioName}");
            }
            
            Scenario scenario = new Scenario();

            // catch invalid JSON
            try
            {
                scenario = JsonSerializer.Deserialize<Scenario>(jsonString);
            }
            catch (Exception ex)
            {
                scenario = Scenario.DefaultLoad($"Failed to parse the scenario file.\n> Filename:\n{filename}\n> Content of scenario file:\n{jsonString}\n> Error message:\n{ex.Message}");
            }

            // check if scenario is valid
            if (scenario == null)
            {
                scenario = Scenario.DefaultLoad("Scenario wasn't loaded properly (null).");
            }

            // check if scenario has all attributes. If not, DEFAULT load.
            if (scenario.particles == null)
            {
                scenario = Scenario.DefaultLoad("Scenario wasn't loaded properly (particles null).");
            }

            foreach (Particle particle in scenario.particles)
            {
                particle.trueInit();
            }

            return scenario;
        }

        /// <summary>
        /// Load the default scenario. Call this function if any error.
        /// </summary>
        /// <param name="message">Message explaining why loading default.</param>
        /// <param name="showMessage">Should show the message?</param>
        /// <returns>The whole scenario, no matter how it will be structured in the future.</returns>
        private static Scenario DefaultLoad(string message, bool showMessage = true)
        {
            if (showMessage)
            {
                MessageBox.Show($"{message}\nRunning default scenario instead.");
            }
            string defaultScenarioName = "0";

            // catch any weird errors and exit
            try
            {
                string filename = $"Scenarios/{defaultScenarioName}.json";
                string jsonString = File.ReadAllText(filename);
                Scenario scenario = JsonSerializer.Deserialize<Scenario>(jsonString);
                return scenario;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error while trying DefaultLoad. Exiting now.\n> Error message:\n{ex.Message}");
                Environment.Exit(1);
                return null;
            }
        }

        //return true if success
        internal static bool SaveScenario(Scenario s, string name)
        {
            string filePath = $"Scenarios/{name}.json";

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                {
                    using(StreamWriter sw = new StreamWriter(fs))
                    {
                        string jsonStr = JsonSerializer.Serialize<Scenario>(s);
                        sw.Write(jsonStr);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Scenario with this name already exists. Please choose another name and try again.");
                return false;
            }            
            return true;
        }
    }
}
