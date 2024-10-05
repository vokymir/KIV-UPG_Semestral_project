using System.Windows.Forms;
using ElectricFieldVis.Model;
using ElectricFieldVis.View;
using ElectricFieldVis.Controller;

namespace ElectricFieldVis.Controller
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            int scenarioNumber = 3;

            if (args.Length > 0 && int.TryParse(args[0], out int parsedScenario))
            {
                scenarioNumber = parsedScenario;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(scenarioNumber));
        }
    }
}