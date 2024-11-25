using ScottPlot.Panels;
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
    public partial class LegendForm : Form
    {
        private Renderer _renderer;
        public LegendForm(Renderer rnd)
        {
            MinimumSize = new Size(150, 360);
            MaximumSize = new Size(150, 360);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "Legend";
            
            InitializeComponent();

            _renderer = rnd;

            // Subscribe to the ColorScaleChanged event
            _renderer.ColorScaleChanged += () => panel1.Invalidate();

            // Attach the Paint event to the panel
            panel1.Paint += Panel1_Paint;

            // close on Ctrl+W
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.W)
                {
                    this.Close();
                }
            };
        }

        private void Panel1_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawLegend(g);

        }

        public void DrawLegend(Graphics g)
        {
            FieldVisualizationLegend fvl = new FieldVisualizationLegend(_renderer.FCM);

            Rectangle rect = new Rectangle(
                20,
                10,
                50,
                0
                );

            fvl.DrawLegend(g, rect);

            Invalidate();
        }

    }
}
