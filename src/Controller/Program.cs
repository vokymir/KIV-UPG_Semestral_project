using System.Windows.Forms;
using ElectricFieldVis.Model;
using ElectricFieldVis.View;
using ElectricFieldVis.Controller;
using System.Text.RegularExpressions;

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
            int grid_w = 30;
            int grid_h = 30;

            if (args.Length > 0)
            {
                scenarioName = args[0];

                for (int i = 1; i < args.Length; i++)
                {
                    Match match = Regex.Match(args[i], @"-g(?<width>\d+)x(?<height>\d+)");

                    if (match.Success)
                    {
                        grid_w = int.Parse(match.Groups["width"].Value);
                        grid_h = int.Parse(match.Groups["height"].Value);
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(scenarioName,grid_w,grid_h));
        }
    }
}