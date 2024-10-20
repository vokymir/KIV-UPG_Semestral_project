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
            SuspendLayout();
            // 
            // ProbeColor
            // 
            ProbeColor.Location = new Point(86, 16);
            ProbeColor.Name = "ProbeColor";
            ProbeColor.Size = new Size(48, 15);
            ProbeColor.TabIndex = 0;
            ProbeColor.UseVisualStyleBackColor = true;
            ProbeColor.Click += btnProbeColor_Click;
            // 
            // PC_label
            // 
            PC_label.AutoSize = true;
            PC_label.Location = new Point(12, 16);
            PC_label.Name = "PC_label";
            PC_label.Size = new Size(68, 15);
            PC_label.TabIndex = 1;
            PC_label.Text = "Probe color";
            // 
            // PPS_label
            // 
            PPS_label.AutoSize = true;
            PPS_label.Location = new Point(5, 75);
            PPS_label.Name = "PPS_label";
            PPS_label.Size = new Size(120, 15);
            PPS_label.TabIndex = 4;
            PPS_label.Text = "Particle positive color";
            // 
            // ParticlePositiveColor
            // 
            ParticlePositiveColor.Location = new Point(131, 75);
            ParticlePositiveColor.Name = "ParticlePositiveColor";
            ParticlePositiveColor.Size = new Size(48, 15);
            ParticlePositiveColor.TabIndex = 3;
            ParticlePositiveColor.UseVisualStyleBackColor = true;
            ParticlePositiveColor.Click += ParticlePositiveColor_Click;
            // 
            // PNC_label
            // 
            PNC_label.AutoSize = true;
            PNC_label.Location = new Point(5, 96);
            PNC_label.Name = "PNC_label";
            PNC_label.Size = new Size(124, 15);
            PNC_label.TabIndex = 6;
            PNC_label.Text = "Particle negative color";
            // 
            // ParticleNegativeColor
            // 
            ParticleNegativeColor.Location = new Point(131, 96);
            ParticleNegativeColor.Name = "ParticleNegativeColor";
            ParticleNegativeColor.Size = new Size(48, 15);
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
            ParticleDynamicWidth.Location = new Point(5, 50);
            ParticleDynamicWidth.Name = "ParticleDynamicWidth";
            ParticleDynamicWidth.Size = new Size(147, 19);
            ParticleDynamicWidth.TabIndex = 8;
            ParticleDynamicWidth.Text = "Particle dynamic width";
            ParticleDynamicWidth.UseVisualStyleBackColor = true;
            ParticleDynamicWidth.CheckedChanged += ParticleDynamicWidth_CheckedChanged;
            // 
            // CustomizerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ParticleDynamicWidth);
            Controls.Add(PNC_label);
            Controls.Add(ParticleNegativeColor);
            Controls.Add(PPS_label);
            Controls.Add(ParticlePositiveColor);
            Controls.Add(PC_label);
            Controls.Add(ProbeColor);
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
    }
}