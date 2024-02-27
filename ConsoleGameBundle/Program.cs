using Colorful;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace DistanceFunction
{
    class Program
    {
        static void Main(string[] args) {
            while (true) {
                ConsoleHelper.SetCurrentFont("Consolas", 15);
                Console.BackgroundColor = Color.Black;
                Console.ForegroundColor = Color.Gray;

                System.Threading.Thread.Sleep(1);
                Console.Clear();
                

                Console.WriteLine("Available apps:");
                Console.WriteLine("1) Fractal viewer");
                Console.WriteLine("2) Tetris");
                Console.Write("\nSelect ID: ");
                string text = Console.ReadLine();
                int id = -1;
                try { id = int.Parse(text); } catch { continue; }
                switch (id) {
                    case 1: Fractal.Run(); break;
                    case 2: Tetris.Run(); break;
                    default: break;
                }

            }
        }
    }
}
