using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectricFieldVis.View
{
    public partial class StatsForm : Form
    {
        


        public StatsForm()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(StatsForm_KeyDown);
        }
        private void StatsForm_KeyDown(object? sender, KeyEventArgs e)
        {
            Utils.HandleCtrlW(this, e);
        }

    }
}
