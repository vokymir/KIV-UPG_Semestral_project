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
        // Declare the event. It will notify subscribers when the color changes.
        public event Action<Color> ColorChanged;

        private Color _probeColor;


        public StatsForm(Color probeColor)
        {
            _probeColor = probeColor;

            InitializeComponent();

            set_btnProbeColor();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(StatsForm_KeyDown);
        }

        private void StatsForm_KeyDown(object? sender, KeyEventArgs e)
        {
            Utils.HandleCtrlW(this,e);
        }

        private void btnProbeColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Update the selected color and display it
                    _probeColor = colorDialog.Color;

                    set_btnProbeColor();

                    // Raise the event to notify subscribers of the color change
                    ColorChanged?.Invoke(_probeColor);
                }
            }
        }

        private void set_btnProbeColor()
        {
            ProbeColor.BackColor = _probeColor;
        }
    }
}
