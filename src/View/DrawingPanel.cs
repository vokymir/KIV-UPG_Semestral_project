using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace ElectricFieldVis.View
{

    /// <summary>
    /// The main panel with the custom visualization
    /// </summary>
    public class DrawingPanel : Panel
    {
        private Renderer _renderer;

        /// <summary>Initializes a new instance of the <see cref="DrawingPanel" /> class.</summary>
        public DrawingPanel(Renderer renderer)
        {
            ClientSize = new Size(800, 600);
            _renderer = renderer;
            DoubleBuffered = true;

        }


        /// <summary>Calls the _renderer to render.</summary>
        /// <remarks>Raises the <see cref="E:System.Windows.Forms.Control.Paint">Paint</see> event.</remarks>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Calling the base class OnPaint
            base.OnPaint(e);

            _renderer.Render(g,ClientSize);
        }

        /// <summary>
        /// Fires the event indicating that the panel has been resized. Inheriting controls use this in favor of actually listening to the event, but still call <span class="keyword">base.onResize</span> to ensure that the event is fired for external listeners.
        /// </summary>
        /// <param name="eventargs">An <see cref="T:System.EventArgs">EventArgs</see> that contains the event data.</param>
        protected override void OnResize(EventArgs eventargs)
        {
            if (_renderer != null)
            {
                _renderer.ResizeWindow(ClientSize);
            }

            Invalidate();  //ensure repaint

            base.OnResize(eventargs);
        }
    }
}
