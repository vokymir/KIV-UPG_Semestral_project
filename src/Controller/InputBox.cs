using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectricFieldVis.Controller
{
    public partial class InputBox : Form
    {
        private TextBox txtInput;

        public string InputText => txtInput.Text;

        public static string Show(string title, Point position,int width = 200, int height = 10)
        {
            using (var inputBox = new InputBox(title,width,height))
            {
                // Set the position to where the user clicked
                inputBox.StartPosition = FormStartPosition.Manual;
                inputBox.Location = position;

                inputBox.ControlBox = false;

                // Show the input box and return the result
                if (inputBox.ShowDialog() == DialogResult.OK)
                {
                    return inputBox.InputText;
                }
                return string.Empty; // If canceled or closed without input
            }
        }

        public InputBox(string title, int width, int height)
        {
            // Basic settings for the form
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Width = width;
            this.Height = height;

            if (title != "")
            {
                this.Height += 30;
                this.Text = title;
            }

            // Create and configure the TextBox for input
            txtInput = new TextBox
            {
                Width = this.Width,
                Location = new Point(0, 5),
                MaxLength = 20 // Limit the input to 20 chars
            };

            txtInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }

                if (e.KeyCode == Keys.Enter)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            };


            this.Controls.Add(txtInput);
        }

        // Center the dialog at the clicked position
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Adjust the form to ensure the input box is properly positioned on the form
            this.Width = Math.Min(250, TextRenderer.MeasureText(this.Text, this.Font).Width + 30); // Adjust width based on the title
        }
    }
}

