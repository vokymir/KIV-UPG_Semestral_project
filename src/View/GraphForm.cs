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
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;

namespace ElectricFieldVis.View
{
    public partial class GraphForm : Form
    {
        private Probe _probe;
        private Renderer _renderer;
        private DataStreamer _streamer;

        // TODO:
        // X axe what is there? tiks, so make it seconds
        public GraphForm(Probe probe, Renderer rnd)
        {
            InitializeComponent();

            _probe = probe;
            _renderer = rnd;

            formsPlot1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlot1.Dock = DockStyle.Fill;
            
            _streamer = formsPlot1.Plot.Add.DataStreamer(100, -0.1);

            formsPlot1.Plot.XLabel("Time (s)");
            formsPlot1.Plot.YLabel("Electric Field Intensity (N·C)");
            formsPlot1.Plot.Axes.ContinuouslyAutoscale = true;
            _streamer.ViewScrollRight();

            formsPlot1.KeyDown += OnKeyDown;

            _streamer.Add(0);
            UpdateGraph();
            Show();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.Control && e.KeyCode == Keys.W)
            {
                this.Dispose();
            }
        }

        public void UpdateGraph()
        {
            float val = FieldCalculator.CalculateFieldIntensity(FieldCalculator.CalculateFieldDirection(_probe.position, _renderer._particles));
            _streamer.Add(val);
            formsPlot1.Refresh();
        }
    }
}
