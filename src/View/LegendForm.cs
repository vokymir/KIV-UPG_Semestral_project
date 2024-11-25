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
        private Graphics _g;
        public LegendForm(Renderer rnd)
        {
            this.Size = new Size(200, 300);
            InitializeComponent();

            _renderer = rnd;
            _g = panel1.CreateGraphics();
            

            _renderer.ColorScaleChanged += DrawLegend;

            DrawLegend();
        }

        public void DrawLegend()
        {
            FieldVisualizationLegend fvl = new FieldVisualizationLegend(_renderer.FCM);

            Rectangle rect = new Rectangle(
                0,
                0,
                50,
                200
                );

            fvl.DrawLegend(_g, rect);

            Invalidate();
        }

    }
}
