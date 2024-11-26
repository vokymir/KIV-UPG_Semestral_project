using ElectricFieldVis.Controller;
using ElectricFieldVis.Model;
using OpenTK.Platform.Windows;
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
    public partial class OtherProbesForm : Form
    {
        private MainForm _form;
        private Renderer _rnd;
        public OtherProbesForm(MainForm form, Renderer rnd)
        {
            _rnd = rnd;
            _form = form;
            _form.OtherProbesChanged += () =>
            {
                RenderButtons();
            };

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            InitializeComponent();

            this.AutoScroll = true;

            this.KeyDown += (s, e) =>
            {
                Utils.HandleCtrlW(this, e);
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _form.opf = null;
            base.OnClosing(e);
        }

        private void RenderButtons()
        {
            // Clear existing buttons if dynamically added
            foreach (var control in Controls.OfType<Button>().ToList())
            {
                if (control.Text.Contains("Probe"))
                {
                    Controls.Remove(control);
                    control.Dispose();
                }
            }

            int buttonX = 10; // Starting position for buttons
            int buttonY = 10;

            foreach ((Probe probe, GraphForm graphForm) in _rnd._otherProbes)
            {
                // Create a new button for each probe
                Button probeButton = new Button
                {
                    Text = $"Probe {probe.ID}",
                    ForeColor = probe.color,
                    Tag = (probe, graphForm),  // Store the probe and its GraphForm
                    Location = new Point(buttonX, buttonY),
                    Size = new Size(130, 30)
                };

                // Attach click event
                probeButton.Click += (sender, args) =>
                {
                    var (p, gf) = ((Probe, GraphForm))((Button)sender).Tag;

                    if (gf.IsDisposed)
                    {
                        // If the graph form was closed, create a new one
                        gf = new GraphForm(p, _rnd);
                        _rnd._otherProbes.RemoveWhere(x => x.Item1 == p);
                        _rnd._otherProbes.Add((p, gf));
                        gf.Show();
                    }
                    else
                    {
                        // Focus the existing graph form
                        gf.Focus();
                    }
                };

                // Add button to the form
                Controls.Add(probeButton);

                // Adjust Y position for the next button
                buttonY += 40;
            }
        }
    }
}
