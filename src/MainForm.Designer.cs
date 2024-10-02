using System.Drawing;
using System.Windows.Forms;

namespace UPG_SP_2024
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            drawingPanel = new DrawingPanel();
            panelMenu = new Panel();
            button1 = new Button();
            panel1 = new Panel();
            panelMenu.SuspendLayout();
            SuspendLayout();
            // 
            // drawingPanel
            // 
            drawingPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            drawingPanel.BorderStyle = BorderStyle.FixedSingle;
            drawingPanel.Location = new Point(198, 5);
            drawingPanel.Name = "drawingPanel";
            drawingPanel.Size = new Size(580, 454);
            drawingPanel.TabIndex = 0;
            drawingPanel.Paint += drawingPanel_Paint;
            // 
            // panelMenu
            // 
            panelMenu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panelMenu.AutoScroll = true;
            panelMenu.BackColor = Color.FromArgb(40, 25, 52);
            panelMenu.BorderStyle = BorderStyle.FixedSingle;
            panelMenu.Controls.Add(button1);
            panelMenu.Location = new Point(5, 5);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(187, 549);
            panelMenu.TabIndex = 0;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(40, 25, 20);
            button1.ForeColor = Color.Snow;
            button1.Location = new Point(3, 85);
            button1.Name = "button1";
            button1.Size = new Size(176, 23);
            button1.TabIndex = 0;
            button1.Text = "Open Scenario From File";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.FromArgb(30, 25, 52);
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Location = new Point(198, 467);
            panel1.Name = "panel1";
            panel1.Size = new Size(580, 87);
            panel1.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 25, 52);
            ClientSize = new Size(784, 561);
            Controls.Add(panel1);
            Controls.Add(panelMenu);
            Controls.Add(drawingPanel);
            MinimumSize = new Size(800, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "A23B0235P - Semestrální práce KIV/UPG 2024/2025";
            panelMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DrawingPanel drawingPanel;
        private Panel panelMenu;
        private Panel panel1;
        private Button button1;
    }
}
