﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            if (e.Control && e.KeyCode == Keys.W)
            {
                form.Close(); // Close the passed form when Ctrl+W is pressed
            }
        }
    }
}