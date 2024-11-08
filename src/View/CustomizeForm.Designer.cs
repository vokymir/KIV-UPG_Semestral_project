namespace ElectricFieldVis.View
{
    partial class CustomizerForm
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
            PC_label = new Label();
            PPS_label = new Label();
            ParticlePositiveColor = new Button();
            PNC_label = new Label();
            ParticleNegativeColor = new Button();
            ParticleDynamicWidth = new CheckBox();
            ZoomFactorBar = new HScrollBar();
            ZoomFactorLabel = new Label();
            SuspendLayout();
            // 
            // ProbeColor
            // 
            ProbeColor.Location = new Point(98, 21);
            ProbeColor.Margin = new Padding(3, 4, 3, 4);
            ProbeColor.Name = "ProbeColor";
            ProbeColor.Size = new Size(55, 20);
            ProbeColor.TabIndex = 0;
            ProbeColor.UseVisualStyleBackColor = true;
            ProbeColor.Click += btnProbeColor_Click;
            // 
            // PC_label
            // 
            PC_label.AutoSize = true;
            PC_label.Location = new Point(14, 21);
            PC_label.Name = "PC_label";
            PC_label.Size = new Size(86, 20);
            PC_label.TabIndex = 1;
            PC_label.Text = "Probe color";
            // 
            // PPS_label
            // 
            PPS_label.AutoSize = true;
            PPS_label.Location = new Point(6, 100);
            PPS_label.Name = "PPS_label";
            PPS_label.Size = new Size(151, 20);
            PPS_label.TabIndex = 4;
            PPS_label.Text = "Particle positive color";
            // 
            // ParticlePositiveColor
            // 
            ParticlePositiveColor.Location = new Point(150, 100);
            ParticlePositiveColor.Margin = new Padding(3, 4, 3, 4);
            ParticlePositiveColor.Name = "ParticlePositiveColor";
            ParticlePositiveColor.Size = new Size(55, 20);
            ParticlePositiveColor.TabIndex = 3;
            ParticlePositiveColor.UseVisualStyleBackColor = true;
            ParticlePositiveColor.Click += ParticlePositiveColor_Click;
            // 
            // PNC_label
            // 
            PNC_label.AutoSize = true;
            PNC_label.Location = new Point(6, 128);
            PNC_label.Name = "PNC_label";
            PNC_label.Size = new Size(156, 20);
            PNC_label.TabIndex = 6;
            PNC_label.Text = "Particle negative color";
            // 
            // ParticleNegativeColor
            // 
            ParticleNegativeColor.Location = new Point(150, 128);
            ParticleNegativeColor.Margin = new Padding(3, 4, 3, 4);
            ParticleNegativeColor.Name = "ParticleNegativeColor";
            ParticleNegativeColor.Size = new Size(55, 20);
            ParticleNegativeColor.TabIndex = 5;
            ParticleNegativeColor.UseVisualStyleBackColor = true;
            ParticleNegativeColor.Click += ParticleNegativeColor_Click;
            // 
            // ParticleDynamicWidth
            // 
            ParticleDynamicWidth.AutoSize = true;
            ParticleDynamicWidth.CheckAlign = ContentAlignment.MiddleRight;
            ParticleDynamicWidth.Checked = true;
            ParticleDynamicWidth.CheckState = CheckState.Checked;
            ParticleDynamicWidth.Location = new Point(6, 67);
            ParticleDynamicWidth.Margin = new Padding(3, 4, 3, 4);
            ParticleDynamicWidth.Name = "ParticleDynamicWidth";
            ParticleDynamicWidth.Size = new Size(180, 24);
            ParticleDynamicWidth.TabIndex = 8;
            ParticleDynamicWidth.Text = "Particle dynamic width";
            ParticleDynamicWidth.UseVisualStyleBackColor = true;
            ParticleDynamicWidth.CheckedChanged += ParticleDynamicWidth_CheckedChanged;
            // 
            // ZoomFactorBar
            // 
            ZoomFactorBar.Cursor = Cursors.Hand;
            ZoomFactorBar.LargeChange = 5;
            ZoomFactorBar.Location = new Point(14, 183);
            ZoomFactorBar.Minimum = 1;
            ZoomFactorBar.Name = "ZoomFactorBar";
            ZoomFactorBar.Size = new Size(191, 26);
            ZoomFactorBar.TabIndex = 9;
            ZoomFactorBar.Value = 1;
            ZoomFactorBar.ValueChanged += ZoomFactorBar_ValueChanged;
            // 
            // ZoomFactorLabel
            // 
            ZoomFactorLabel.AutoSize = true;
            ZoomFactorLabel.BackColor = SystemColors.Control;
            ZoomFactorLabel.Location = new Point(30, 163);
            ZoomFactorLabel.Name = "ZoomFactorLabel";
            ZoomFactorLabel.Size = new Size(114, 20);
            ZoomFactorLabel.TabIndex = 10;
            ZoomFactorLabel.Text = "Zooming speed";
            // 
            // CustomizerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(ZoomFactorLabel);
            Controls.Add(ZoomFactorBar);
            Controls.Add(ParticleDynamicWidth);
            Controls.Add(PNC_label);
            Controls.Add(ParticleNegativeColor);
            Controls.Add(PPS_label);
            Controls.Add(ParticlePositiveColor);
            Controls.Add(PC_label);
            Controls.Add(ProbeColor);
            Margin = new Padding(3, 4, 3, 4);
            Name = "CustomizerForm";
            Text = "Customizer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ColorDialog colorDialog1;
        private Button ProbeColor;
        private Label PC_label;
        private Label PPS_label;
        private Button ParticlePositiveColor;
        private Label PNC_label;
        private Button ParticleNegativeColor;
        private CheckBox ParticleDynamicWidth;
        private HScrollBar ZoomFactorBar;
        private Label ZoomFactorLabel;
    }
}