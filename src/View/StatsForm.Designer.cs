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
            OriginPosition = new Label();
            ZoomLevel = new Label();
            SuspendLayout();
            // 
            // ProbeCoords
            // 
            ProbeCoords.AutoSize = true;
            ProbeCoords.Location = new Point(14, 12);
            ProbeCoords.Name = "ProbeCoords";
            ProbeCoords.Size = new Size(98, 60);
            ProbeCoords.TabIndex = 0;
            ProbeCoords.Text = "probe coords\r\nx\r\ny";
            // 
            // ProbeDirection
            // 
            ProbeDirection.AutoSize = true;
            ProbeDirection.Location = new Point(14, 84);
            ProbeDirection.Name = "ProbeDirection";
            ProbeDirection.Size = new Size(111, 60);
            ProbeDirection.TabIndex = 1;
            ProbeDirection.Text = "Probe direction\r\nX\r\nY";
            // 
            // OriginPosition
            // 
            OriginPosition.AutoSize = true;
            OriginPosition.Location = new Point(14, 153);
            OriginPosition.Name = "OriginPosition";
            OriginPosition.Size = new Size(108, 60);
            OriginPosition.TabIndex = 2;
            OriginPosition.Text = "Origin position\r\nX\r\nY";
            // 
            // ZoomLevel
            // 
            ZoomLevel.AutoSize = true;
            ZoomLevel.Location = new Point(14, 226);
            ZoomLevel.Name = "ZoomLevel";
            ZoomLevel.Size = new Size(49, 20);
            ZoomLevel.TabIndex = 3;
            ZoomLevel.Text = "Zoom\r\n";
            // 
            // StatsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(ZoomLevel);
            Controls.Add(OriginPosition);
            Controls.Add(ProbeDirection);
            Controls.Add(ProbeCoords);
            Margin = new Padding(3, 4, 3, 4);
            Name = "StatsForm";
            Text = "Statistics";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ProbeCoords;
        private Label ProbeDirection;
        private Label OriginPosition;
        private Label ZoomLevel;
    }
}