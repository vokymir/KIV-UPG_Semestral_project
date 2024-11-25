using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectricFieldVis.Model;
using ScottPlot.Plottables;
using ScottPlot.WinForms;

namespace ElectricFieldVis.View
{
    public partial class GraphForm : Form
    {
        private Probe _probe;
        private Renderer _renderer;
        private DataStreamer _streamer;
        public GraphForm(Probe probe, Renderer rnd)
        {
            InitializeComponent();

            _probe = probe;
            _renderer = rnd;

            formsPlot1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlot1.Dock = DockStyle.Fill;
            
            _streamer = formsPlot1.Plot.Add.DataStreamer(60);
            _streamer.ViewScrollLeft();

            UpdateGraph();
            Show();
        }

        public void UpdateGraph()
        {
            float val = FieldCalculator.CalculateFieldIntensity(FieldCalculator.CalculateFieldDirection(_probe.position, _renderer._particles));
            _streamer.Add(val);
            formsPlot1.Refresh();
        }
    }
}
