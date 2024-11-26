using ElectricFieldVis.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    /// <summary>
    /// For many things I do not know where to put.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Close the specified form via CTRL + W.
        /// </summary>
        /// <param name="form">Form to close.</param>
        /// <param name="e"></param>
        public static void HandleCtrlW(Form form, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.W || e.KeyCode == Keys.Escape)
            {
                form.Close(); // Close the passed form when Ctrl+W is pressed
            }
        }

        public static void HandleKeyboard(Form form, Renderer rend, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Right)
            {
                rend.Origin = new Vector2(rend.Origin.X + 50, rend.Origin.Y);
            }
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
