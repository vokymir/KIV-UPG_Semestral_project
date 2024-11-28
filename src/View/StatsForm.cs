using ElectricFieldVis.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectricFieldVis.View
{
    public partial class StatsForm : Form
    {
        
        /// <summary>
        /// Form with DEV statistics.
        /// </summary>
        public StatsForm()
        {
            InitializeComponent();

            this.Size = new Size(300, 300);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(StatsForm_KeyDown);
        }

        
        private void StatsForm_KeyDown(object? sender, KeyEventArgs e)
        {
            Utils.HandleCtrlW(this, e);
        }

        public void UpdateProbeCoords(Vector2 coords)
        {
            string text = $"Probe Coordinates:\nX = {coords.X}\nY = {coords.Y}";

            ProbeCoords.Text = text;
        }

        public void UpdateProbeDirection(Vector2 direction)
        {
            string text = $"Probe Force Direction\nX = {direction.X}\nY = {direction.Y}";

            ProbeDirection.Text = text;
        }

        public void UpdateOriginPos(Vector2 origin)
        {
            string text = $"Origin Position\nX = {origin.X}\nY = {origin.Y}";

            OriginPosition.Text = text;
        }

        public void UpdateZoom(float zoom)
        {
            string text = $"Zoom\n{zoom}";

            ZoomLevel.Text = text;
        }
    }
}
