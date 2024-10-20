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
            ProbeCoords = new Label();
            ProbeDirection = new Label();
            SuspendLayout();
            // 
            // ProbeCoords
            // 
            ProbeCoords.AutoSize = true;
            ProbeCoords.Location = new Point(12, 9);
            ProbeCoords.Name = "ProbeCoords";
            ProbeCoords.Size = new Size(77, 45);
            ProbeCoords.TabIndex = 0;
            ProbeCoords.Text = "probe coords\r\nx\r\ny";
            // 
            // ProbeDirection
            // 
            ProbeDirection.AutoSize = true;
            ProbeDirection.Location = new Point(12, 63);
            ProbeDirection.Name = "ProbeDirection";
            ProbeDirection.Size = new Size(88, 45);
            ProbeDirection.TabIndex = 1;
            ProbeDirection.Text = "Probe direction\r\nX\r\nY";
            // 
            // StatsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ProbeDirection);
            Controls.Add(ProbeCoords);
            Name = "StatsForm";
            Text = "Statistics";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ProbeCoords;
        private Label ProbeDirection;
    }
}