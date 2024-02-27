//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Colorful;
using System.Drawing;

namespace DistanceFunction
{
    static class Fractal
    {
        static double zoom, posX, posY, exposure;
        static bool NeedsRestart = true;

        public static void Run()
        {
            ConsoleHelper.SetCurrentFont("Consolas", 8);
            Console.BackgroundColor = Color.Black;
            Console.ForegroundColor = Color.Cyan;


            while (true)
            {
                if (NeedsRestart) {
                    Restart();
                    NeedsRestart = false;
                }

                
                Redraw(posX, posY, zoom, exposure);

                Console.WriteLine($"\tpos[{posX};{posY}] zoom[{zoom}] bright[{exposure:0.00}]");
                char key = Console.ReadKey(true).KeyChar;
                if (key == '+') zoom *= 2;
                if (key == '-') zoom /= 2;

                if (key == 'w') posY += 0.1 / zoom;
                if (key == 's') posY -= 0.1 / zoom;

                if (key == 'd') posX += 0.1 / zoom;
                if (key == 'a') posX -= 0.1 / zoom;

                if (key == '*') exposure *= 1.1;
                if (key == '/') exposure /= 1.1;

                if (key == 'r') NeedsRestart = true;

                if (key == '`') return;
            }
        }

        static void Restart()
        {
            zoom = 1.0;
            posX = 0.0;
            posY = -0.8;
            exposure = 1.0;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = (int)(System.Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - System.Math.Floor(hue / 60);

            value = value * 255;
            int v = (int)(value);
            int p = (int)(value * (1 - saturation));
            int q = (int)(value * (1 - f * saturation));
            int t = (int)(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        static void Redraw(double posX, double posY, double zoom, double exposure)
        {
            string str0 = " .:-=+*#%@";
            string str1 = " .:!*%$@&#SB";
            string str2 = " .:;!nxrfjZ";
            string str3 = " .░▒♦■▓◘█";
            var str = str3;
            Clear();

            const double width = 75*2+1, height = 30*2+1;
            for (double j = +height / 2; j >= -height / 2; j--)
            {
                for (double i = -width / 2; i <= width / 2; i++)
                {
                    var x = posX + i / width / zoom / 2;
                    var y = posY + j / width / zoom;

                    var v = exposure * Pixel(x, y, out double hue);
                    v = System.Math.Max(0, System.Math.Min(1, v));

                    int id = (int)((str.Length - 1)*v);

                    Console.Write(str[id], ColorFromHSV(hue,1,1));
                }
                Console.WriteLine();
            }
        }

        static void Clear()
        {
            Console.Clear();
        }

        static double Pixel(double x, double y, out double hue)
        {
            //return Math.Sin(x*2)*Math.Cos(y*2*Math.Pow(y,0.4));
            return Mandelbrot(y, x, out hue);
        }

        struct rotor2
        {
            public double s, xy; public rotor2(double s, double xy) { this.s = s; this.xy = xy; }
            public static rotor2 operator *(rotor2 a, rotor2 b)
            {
                return new rotor2(a.s * b.s - a.xy * b.xy, a.s * b.xy + a.xy * b.s);
            }
            public static rotor2 operator +(rotor2 a, rotor2 b)
            {
                return new rotor2(a.s + b.s, a.xy + b.xy);
            }
            public static rotor2 operator -(rotor2 a, rotor2 b)
            {
                return new rotor2(a.s - b.s, a.xy - b.xy);
            }
            public double lengthSq()
            {
                return s * s + xy * xy;
            }
        }

        static double upow(double x, double y) => System.Math.Pow(System.Math.Abs(x), y) * System.Math.Sign(x);

        static double Mandelbrot(double x, double y, out double hue)
        {
            var p = new rotor2(x, y);
            var z = p;

            float i = 0, max = 400;
            for (; i < max && (z = z * z + p).lengthSq() < 4; i++) { }

            hue = 160 + 50 + i * 1;
            //hue = System.Math.Atan2(z.xy, z.s)*780;
            //hue = 180;
            return i / max;
        }

    }
}
