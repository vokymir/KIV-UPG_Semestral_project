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
        private System.Drawing.Color _color;

        public GraphForm(Probe probe, Renderer rnd)
        {
            Width = 600;
            Height = 400;
            MinimumSize = new Size(Width, Height);
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

            _probe = probe;
            _renderer = rnd;
            _color = probe.color;
            ScottPlot.Color color = ScottPlot.Color.FromColor(_color);

            formsPlot1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            formsPlot1.Dock = DockStyle.Fill;

            formsPlot1.Plot.Title($"Graph for probe {probe.ID.ToString()}");
            formsPlot1.Plot.Axes.Title.Label.ForeColor = color;
            formsPlot1.Plot.Axes.Title.Label.FontSize = 32;

            // color could be with low constrast
            //formsPlot1.Plot.Add.Palette = new ScottPlot.Palettes.Custom([color],"clr");

            _streamer = formsPlot1.Plot.Add.DataStreamer(100, -0.1);
            
            formsPlot1.Plot.XLabel("Time (s)");
            formsPlot1.Plot.YLabel("Intensity (N·C)");
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
            float val = FieldCalculator.CalculateFieldIntensity(FieldCalculator.CalculateFieldDirection(_probe.position, _renderer.particles));
            _streamer.Add(val);
            formsPlot1.Refresh();
        }

    }
}
