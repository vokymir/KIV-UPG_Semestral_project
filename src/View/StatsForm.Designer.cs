namespace ElectricFieldVis.View
{
    partial class StatsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            colorDialog1 = new ColorDialog();
            ProbeColor = new Button();
            SuspendLayout();
            // 
            // ProbeColor
            // 
            ProbeColor.ImageAlign = ContentAlignment.MiddleRight;
            ProbeColor.Location = new Point(32, 26);
            ProbeColor.Name = "ProbeColor";
            ProbeColor.Size = new Size(83, 83);
            ProbeColor.TabIndex = 0;
            ProbeColor.TextAlign = ContentAlignment.TopCenter;
            ProbeColor.UseMnemonic = false;
            ProbeColor.UseVisualStyleBackColor = true;
            ProbeColor.Click += btnProbeColor_Click;
            // 
            // StatsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ProbeColor);
            Name = "StatsForm";
            Text = "StatsForm";
            ResumeLayout(false);
        }

        #endregion

        private ColorDialog colorDialog1;
        private Button ProbeColor;
    }
}