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
        ///  Parses arguments from command line and starts the application.
        /// </summary>
        static void Main(string[] args)
        {
            string scenarioName = "0";

            if (args.Length > 0)
            {
                scenarioName = args[0];
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(scenarioName));
        }
    }
}