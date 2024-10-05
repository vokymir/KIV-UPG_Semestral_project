﻿using System.Drawing;
using System.Windows.Forms;
using ElectricFieldVis.View;

namespace ElectricFieldVis.Controller
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
            drawingPanel = new DrawingPanel(_renderer);
            SuspendLayout();
            // 
            // drawingPanel
            // 
            drawingPanel.Dock = DockStyle.Fill;
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.Name = "drawingPanel";            
            drawingPanel.TabIndex = 0;            
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;            
            Controls.Add(drawingPanel);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "A23B0235P - Semestrální práce KIV/UPG 2024/2025";
            ResumeLayout(false);
        }

        #endregion

        private Panel drawingPanel;
    }
}
