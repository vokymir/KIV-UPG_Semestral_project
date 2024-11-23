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
    public partial class CustomizerForm : Form
    {
        // Declare events
        public event Action<Color> ProbeColorChanged;
        public event Action<Boolean> ParticleDynamicWidthChecked;
        public event Action<Color> ParticlePositiveColorChanged;
        public event Action<Color> ParticleNegativeColorChanged;
        public event Action<int> ZoomLevelChanged;
        public event Action<bool> ShowGridChanged;
        public event Action<bool> ShowStaticProbesChanged;

        private Color _probeColor;
        private Color _particlePositiveColor = Color.Red;
        private Color _particleNegativeColor = Color.Blue;



        /// <summary>
        /// Customize the visual aspect of an app.
        /// </summary>
        /// <param name="probeColor">Color of the Probe, to set it.</param>
        public CustomizerForm(Color probeColor, Color particlePositiveColor, Color particleNegativeColor, bool particleDynamicWidth, Point startLoc)
        {
            _probeColor = probeColor;
            _particlePositiveColor = particlePositiveColor;
            _particleNegativeColor = particleNegativeColor;

            InitializeComponent();

            ParticleDynamicWidth.Checked = particleDynamicWidth;
            showGrid.Checked = true;
            showStaticProbes.Checked = true;

            this.Size = new Size(300, 300);
            this.StartPosition = FormStartPosition.Manual;
            startLoc.Y += 300;
            this.Location = startLoc;

            set_btnColor(ProbeColor, _probeColor);
            set_btnColor(ParticlePositiveColor, _particlePositiveColor);
            set_btnColor(ParticleNegativeColor, _particleNegativeColor);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(CustomizerForm_KeyDown);
        }
        private void CustomizerForm_KeyDown(object? sender, KeyEventArgs e)
        {
            Utils.HandleCtrlW(this, e);
        }

        private void btnProbeColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Update the selected color and display it
                    _probeColor = colorDialog.Color;

                    set_btnColor(ProbeColor, _probeColor);

                    // Raise the event to notify subscribers of the color change
                    ProbeColorChanged?.Invoke(_probeColor);
                }
            }
        }

        private void set_btnColor(Button obj, Color clr)
        {
            obj.BackColor = clr;
        }

        private void ParticleDynamicWidth_CheckedChanged(object sender, EventArgs e)
        {
            ParticleDynamicWidthChecked?.Invoke(ParticleDynamicWidth.Checked);
        }

        private void ParticlePositiveColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Update the selected color and display it
                    _particlePositiveColor = colorDialog.Color;

                    set_btnColor(ParticlePositiveColor, _particlePositiveColor);

                    // Raise the event to notify subscribers of the color change
                    ParticlePositiveColorChanged?.Invoke(_particlePositiveColor);
                }
            }
        }

        private void ParticleNegativeColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Update the selected color and display it
                    _particleNegativeColor = colorDialog.Color;

                    set_btnColor(ParticleNegativeColor, _particleNegativeColor);

                    // Raise the event to notify subscribers of the color change
                    ParticleNegativeColorChanged?.Invoke(_particleNegativeColor);
                }
            }
        }

        private void ZoomFactorBar_ValueChanged(object sender, EventArgs e)
        {
            int level = ZoomFactorBar.Value;
            ZoomLevelChanged?.Invoke(level);
        }

        private void showGrid_CheckedChanged(object sender, EventArgs e)
        {
            ShowGridChanged?.Invoke(showGrid.Checked);
        }

        private void showStaticProbes_CheckedChanged(object sender, EventArgs e)
        {
            ShowStaticProbesChanged?.Invoke(showStaticProbes.Checked);
        }
    }
}
