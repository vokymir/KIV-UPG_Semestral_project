using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.View
{
    public class FieldVisualizationLegend
    {
        private readonly FieldColorMapper _colorMapper;
        private readonly Font _font;
        private const int LEGEND_WIDTH = 60;
        private const int LEGEND_HEIGHT = 200;
        private const int LABEL_PADDING = 10;
        private const int NUM_LABELS = 5;

        public FieldVisualizationLegend(FieldColorMapper colorMapper)
        {
            _colorMapper = colorMapper;
            _font = new Font("Arial", 8);
        }

        public void DrawLegend(Graphics g, Rectangle bounds)
        {
            // bounds rect
            Rectangle boundsRect = new Rectangle(
                bounds.Right - LEGEND_WIDTH - 10,
                bounds.Top - 10,
                LEGEND_WIDTH * 2,
                LEGEND_HEIGHT + LABEL_PADDING * 4
            );

            // gradient rectangle
            Rectangle gradientRect = new Rectangle(
                bounds.Right - LEGEND_WIDTH,
                bounds.Top,
                LEGEND_WIDTH / 2,
                LEGEND_HEIGHT
            );

            // draw the color gradient
            using (Bitmap gradientBitmap = new Bitmap(gradientRect.Width, gradientRect.Height))
            {
                for (int y = 0; y < gradientRect.Height; y++)
                {
                    // intensity for this y position (bottom = min, top = max)
                    float normalizedPosition = 1 - (float)y / gradientRect.Height;
                    float intensity = (float)(_colorMapper.MinIntensity +
                        normalizedPosition * (_colorMapper.MaxIntensity - _colorMapper.MinIntensity));

                    Color color = _colorMapper.ConvertIntensityToColor(intensity);

                    // draw a line of this color
                    using (Pen pen = new Pen(color))
                    {
                        for (int x = 0; x < gradientRect.Width; x++)
                        {
                            gradientBitmap.SetPixel(x, y, color);
                        }
                    }
                }

                Brush brush = new SolidBrush(Color.FromArgb(200, Color.White));

                // draw background for bitmap
                g.FillRectangle(brush, boundsRect);

                // draw the bitmap
                g.DrawImage(gradientBitmap, gradientRect);
            }

            // draw border around the gradient
            using (Pen borderPen = new Pen(Color.Black, 1))
            {
                g.DrawRectangle(borderPen, gradientRect);
            }

            // draw labels
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;

                for (int i = 0; i < NUM_LABELS; i++)
                {
                    float normalizedPosition = i / (float)(NUM_LABELS - 1);
                    float value = (float)(_colorMapper.MaxIntensity -
                        normalizedPosition * (_colorMapper.MaxIntensity - _colorMapper.MinIntensity));

                    int y = gradientRect.Top + (int)(normalizedPosition * gradientRect.Height);

                    // draw tick mark
                    g.DrawLine(Pens.Black,
                        gradientRect.Right,
                        y,
                        gradientRect.Right + 5,
                        y);

                    // draw label
                    string label = value.ToString("F0")+"%";
                    g.DrawString(label,
                        _font,
                        Brushes.Black,
                        gradientRect.Right + LABEL_PADDING,
                        y,
                        format);
                }
            }

            // draw title
            using (StringFormat titleFormat = new StringFormat())
            {
                titleFormat.Alignment = StringAlignment.Center;
                g.DrawString("Field Intensity",
                    _font,
                    Brushes.Black,
                    gradientRect.Left + gradientRect.Width / 2,
                    gradientRect.Bottom + LABEL_PADDING);
            }
        }

        public void Dispose()
        {
            _font?.Dispose();
        }
    }
}
