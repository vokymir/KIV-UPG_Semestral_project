using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.View
{
    public class GifAnimation
    {
        private Image _gifImage;
        private FrameDimension _frameDimension;
        private int _frameCount;
        private int _currentFrame = 0;
        private bool _isGif;

        public GifAnimation(string gifPath, string backupPath)
        {
            try
            {
                _gifImage = Image.FromFile(gifPath);
                _frameDimension = new FrameDimension(
                    _gifImage.FrameDimensionsList[0]
                );
                _frameCount = _gifImage.GetFrameCount(_frameDimension);
                _isGif = true;
            }
            catch (Exception ex)
            {
                _gifImage?.Dispose();
                _gifImage = Image.FromFile(backupPath);
                _isGif = false;
            }
        }

        public void DrawCurrentFrame(Graphics g, Rectangle drawRect)
        {
            _gifImage.SelectActiveFrame(_frameDimension, _currentFrame);
            g.DrawImage(_gifImage, drawRect);

            // move to next frame, loop back to start if needed
            _currentFrame = (_currentFrame + 1) % _frameCount;
        }

        // draw at specific coordinates
        public void DrawAtPosition(Graphics g, Point position, int width, int height)
        {
            Rectangle drawRect = new Rectangle(
                position.X,
                position.Y,
                width,
                height
            );

            DrawCurrentFrame(g, drawRect);
        }

        // cause of rotation and scale
        public void DrawTransformed(
        Graphics g,
        PointF from,
        PointF to,
        float? maxWidth = null,
        bool maintainAspectRatio = true)
        {
            // select the current frame
            _gifImage.SelectActiveFrame(_frameDimension, _currentFrame);

            // calculate vector and angle
            PointF vector = new PointF(to.X - from.X, to.Y - from.Y);
            float distance = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            float angle = (float)(Math.Atan2(vector.Y, vector.X) * 180 / Math.PI);

            // scaling
            float scaledWidth, scaledHeight;
            if (maxWidth.HasValue)
            {
                // calculate scale based on max width or distance
                float widthScale = maxWidth.Value / _gifImage.Width;
                float lengthScale = distance / _gifImage.Height;

                if (maintainAspectRatio)
                {
                    // use the smaller
                    float scale = Math.Min(widthScale, lengthScale);
                    scaledWidth = _gifImage.Width * scale;
                    scaledHeight = _gifImage.Height * scale;
                }
                else
                {
                    // stretch to fit
                    scaledWidth = _gifImage.Width * widthScale;
                    scaledHeight = distance;
                }
            }
            else
            {
                // default scaling to fit distance
                float scale = distance / _gifImage.Height;
                scaledWidth = _gifImage.Width * scale;
                scaledHeight = distance;
            }

            // save graphics state
            GraphicsState state = g.Save();

            try
            {
                // to startpoint
                g.TranslateTransform(from.X, from.Y);
                g.RotateTransform(angle);

                g.DrawImage(
                    _gifImage,
                    0,
                    -scaledHeight/2,
                    scaledWidth,
                    scaledHeight
                );
            }
            finally
            {
                g.Restore(state);
            }

            // move frame
            _currentFrame = (_currentFrame + 1) % _frameCount;
        }

        // Overload to support Point instead of PointF
        public void DrawTransformed(
            Graphics g,
            Point from,
            Point to,
            float? maxWidth = null,
            bool maintainAspectRatio = true)
        {
            DrawTransformed(
                g,
                new PointF(from.X, from.Y),
                new PointF(to.X, to.Y),
                maxWidth,
                maintainAspectRatio
            );
        }
    }
}
