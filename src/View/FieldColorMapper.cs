﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.View
{
    public class FieldColorMapper
    {
        private readonly double _minIntensity;
        public double MinIntensity {  get { return _minIntensity; } }
        private readonly double _maxIntensity;
        public double MaxIntensity {  get { return _maxIntensity; } }
        private ColorScale _colorScale;
        public ColorScale Color_scale
        {
            get { return _colorScale; }
            set
            {
                _colorScale = value;
            }
        }

        public enum ColorScale
        {
            BlueRed,    // blue to red
            Rainbow,     // full rainbow spectrum
            Plasma,      // similar to matplotlib's plasma
            Thermal     // black to white through red/yellow - darkmode
        }

        public FieldColorMapper(double minIntensity, double maxIntensity, ColorScale colorScale = ColorScale.BlueRed)
        {
            _minIntensity = minIntensity;
            _maxIntensity = maxIntensity;
            _colorScale = colorScale;
        }

        public Color ConvertIntensityToColor(float intensity)
        {
            // normalize intensity to [0,1] range
            double normalized = (intensity - _minIntensity) / (_maxIntensity - _minIntensity);
            normalized = Math.Max(0, Math.Min(1, normalized)); // clamp to [0,1]

            return _colorScale switch
            {
                ColorScale.BlueRed => BlueRedScale(normalized),
                ColorScale.Rainbow => RainbowScale(normalized),
                ColorScale.Plasma => PlasmaScale(normalized),
                ColorScale.Thermal => ThermalScale(normalized),
                _ => BlueRedScale(normalized)
            };
        }

        private static Color BlueRedScale(double normalized)
        {
            // blue 240° to red 0° in HSV space
            return HSVToRGB(240 * (1 - normalized), 1, 1);
        }

        private static Color RainbowScale(double normalized)
        {
            // purple > blue > cyan > green > yellow > red
            double hue = 270 - (normalized * 270);
            return HSVToRGB(hue, 1, 1);
        }

        private static Color PlasmaScale(double normalized)
        {
            // approximation of matplotlib's plasma colormap
            if (normalized < 0.25)
            {
                return InterpolateColor(Color.FromArgb(13, 8, 135), Color.FromArgb(126, 3, 168), normalized * 4);
            }
            else if (normalized < 0.5)
            {
                return InterpolateColor(Color.FromArgb(126, 3, 168), Color.FromArgb(204, 71, 120), (normalized - 0.25) * 4);
            }
            else if (normalized < 0.75)
            {
                return InterpolateColor(Color.FromArgb(204, 71, 120), Color.FromArgb(248, 149, 64), (normalized - 0.5) * 4);
            }
            else
            {
                return InterpolateColor(Color.FromArgb(248, 149, 64), Color.FromArgb(240, 249, 33), (normalized - 0.75) * 4);
            }// what a nice code, wanna switch?
        }

        private static Color ThermalScale(double normalized)
        {
            if (normalized < 0.33)
            {
                // black to red
                int value = (int)(normalized * 3 * 255);
                return Color.FromArgb(value, 0, 0);
            }
            else if (normalized < 0.66)
            {
                // red to yellow
                int value = (int)((normalized - 0.33) * 3 * 255);
                return Color.FromArgb(255, value, 0);
            }
            else
            {
                // yellow to white
                int value = (int)((normalized - 0.66) * 3 * 255);
                return Color.FromArgb(255, 255, Math.Max(0, Math.Min(255, value)));
            }
        }

        private static Color HSVToRGB(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(v, t, p),
                1 => Color.FromArgb(q, v, p),
                2 => Color.FromArgb(p, v, t),
                3 => Color.FromArgb(p, q, v),
                4 => Color.FromArgb(t, p, v),
                _ => Color.FromArgb(v, p, q)
            };
        }

        private static Color InterpolateColor(Color c1, Color c2, double factor)
        {
            int r = (int)(c1.R + (c2.R - c1.R) * factor);
            int g = (int)(c1.G + (c2.G - c1.G) * factor);
            int b = (int)(c1.B + (c2.B - c1.B) * factor);
            return Color.FromArgb(r, g, b);
        }
    }
}
