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
                //MessageBox.Show($"Error loading GIF: {ex.Message}");
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

        public void DrawTransformed(Graphics g, PointF from, PointF to, float maxWidth = 100f)
        {
            // Select the current frame
            _gifImage.SelectActiveFrame(_frameDimension, _currentFrame);

            // Calculate the vector and angle
            PointF vector = new PointF(to.X - from.X, to.Y - from.Y);
            float distance = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            float angle = (float)(Math.Atan2(vector.Y, vector.X) * 180 / Math.PI);

            // Calculate scaling
            float scaleFactor = Math.Min(maxWidth / _gifImage.Width, distance / _gifImage.Height);
            int scaledWidth = (int)(_gifImage.Width * scaleFactor);
            int scaledHeight = (int)(_gifImage.Height * scaleFactor);

            // Save the current graphics state
            GraphicsState state = g.Save();

            try
            {
                // Translate to the start point
                g.TranslateTransform(from.X, from.Y);

                // Rotate around the starting point
                g.RotateTransform(angle);

                // Create a matrix for transformation
                Matrix matrix = new Matrix();
                matrix.Scale(scaleFactor, scaleFactor);

                // Draw the image
                g.DrawImage(
                    _gifImage,
                    0, 0,
                    new Rectangle(0, 0, scaledWidth, scaledHeight),
                    GraphicsUnit.Pixel
                );
            }
            finally
            {
                // Restore the graphics state
                g.Restore(state);
            }

            // Move to next frame, loop back to start if needed
            _currentFrame = (_currentFrame + 1) % _frameCount;
        }

        // Overload to support Point instead of PointF        
        public void DrawTransformed(Graphics g, Point from, Point to, float maxWidth = 100f)
        {
            DrawTransformed(g, new PointF(from.X, from.Y), new PointF(to.X, to.Y), maxWidth);
        }

    }
}
