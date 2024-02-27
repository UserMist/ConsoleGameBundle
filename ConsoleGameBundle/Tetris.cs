using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DistanceFunction {
    static class Tetris
    {
        const int O = 1, _ = 0;
        public static List<int[]> Shapes = new List<int[]>() {
            new int[]{
                    O, _, _,
                    O, _, _,
                    O, _, _,
                    O, _, _
            },
            new int[]{
                    _, O, _,
                    _, O, _,
                    O, O, _
            },
            new int[]{
                    O, _, _,
                    O, _, _,
                    O, O, _
            },
            new int[]{
                    O, O, _,
                    O, O, _
            },
            new int[]{
                    O, O, _,
                    _, O, O
            },
            new int[]{
                    O, O, _,
                    _, O, O
            },
            new int[]{
                    _, O, O,
                    O, O, _
            },
            new int[]{
                    _, O, _,
                    O, O, O
            }
        };


        public const int Width = 8, Height = 16;

        public static int[,] Map = new int[Width, Height];
        public static int Score = 0;

        static bool exists(int i, int j) { 
            return i >= 0 && j >= 0 && i < Width && j < Height; 
        }

        static char AsyncKey = ' ', key;
        static float dt = 0.01f;
        static bool needRedraw;
        static float TimeUntilFall = 1;

        public static void RunInput()
        {
            while (true)
            {
                char k = Console.ReadKey(true).KeyChar;
                AsyncKey = k;
            }
        }

        /*
        public static void RotateAround(int _i, int _j)
        {
            for (int i = 0; i < Width; i++) for (int j = 0; j < Height; j++) if(Map[i,j] < 0) {
                    var buff = Map[i, j];
                    var tj = _j + (i - _i);
                    var ti = _i - (j - _j);
                    if (exists(ti, tj)) { Map[i, j] =  Map[ti, tj]; Map[ti, tj] = buff; }
                    else Map[i, j] = 0;
                }
        }
        */

        public static void Run()
        {
            ConsoleHelper.SetCurrentFont("Consolas", 15);
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;

            Thread inp = new Thread(RunInput);
            inp.Start();
            //Timer time = new Timer();

            while (true)
            {
                key = AsyncKey;
                AsyncKey = ' ';
                Tick();

                if (key == '`') { inp.Abort(); return; }

                if (NeedRestart) {
                    Restart();
                    NeedRestart = false;
                }
                Thread.Sleep(10);
            }
        }

        public static void Restart()
        {
            for (int i = 0; i < Width; i++) for (int j = 0; j < Height; j++) {
                Map[i, j] = 0;
            }

            NextShape();
            TimeUntilFall = 1;
            Score = 0;
            needRedraw = true;
        }

        public static void Tick()
        {
            TimeUntilFall -= dt;

            bool obstructedLeft = false, obstructedRight = false, obstructedBottom = false;

            if (key != ' ' || TimeUntilFall <= 0)
            {
                CheckObstructions(out obstructedLeft, out obstructedRight, out obstructedBottom);
            }

            if (key != ' ')
            {
                if (key == 'd' && !obstructedRight) { DisplaceShape(+1, 0); needRedraw = true; }
                if (key == 'a' && !obstructedLeft) { DisplaceShape(-1, 0); needRedraw = true; }
                if (key == 's' && !obstructedBottom) { DisplaceShape(0, +1); TimeUntilFall = 1; needRedraw = true; return; }
                if (key == 's' && obstructedBottom) { AttachShape(); TimeUntilFall = 1; needRedraw = true; return; }
                if (key == 'r') { Restart(); return; }
            }

            if (TimeUntilFall <= 0)
            {
                if (obstructedBottom) AttachShape();
                else DisplaceShape(0, +1);
                TimeUntilFall = 1;
                needRedraw = true;
            }

            needRedraw |= (key != ' ');

            if (needRedraw) {
                Redraw();
                needRedraw = false;
            }
        }

        public static void Redraw()
        {
            var s = "";
            for (var j = 0; j < Height; j++)
            {
                for (var i = 0; i < Width; i++)
                {
                    var m = Map[i,j];
                    if (m < 0)
                        s += 'X';
                    else if (m > 0)
                        s += 'O';
                    else
                        s += ' ';
                    s += ' ';
                }
                s += "||";
                s += '\n';
            }
            for (var i = 0; i < 2*Width; i++ )
                s += '=';
            s += "XX\nScore: "; s+=Score;
            Console.Clear();
            Console.Write(s);
        }

        public static void DisplaceShape(int di, int dj)
        {
            int dirI = di > 0 ? -1 : +1, startI = di > 0 ? Width - 1 : 0, endI = di > 0 ? -1: Width;
            int dirJ = dj > 0 ? -1 : +1, startJ = dj > 0 ? Height - 1 : 0, endJ = dj > 0 ? -1: Height;
 

            for (var i = startI; i != endI; i += dirI) for (var j = startJ; j != endJ; j += dirJ) {
                if (exists(i + di, j + dj) && exists(i, j) && Map[i, j] < 0)
                    Map[i + di, j + dj] = Map[i, j];
                if (exists(i, j) && Map[i, j] < 0)
                    Map[i, j] = 0;
            }
        }

        public static bool RowIsFull(int id)
        {
            for (var i = 0; i < Width; i++) if(Map[i, id] == 0)
                return false;
            return true;
        }

        public static void RemoveRow(int id)
        {
            for (var i = 0; i < Width; i++) for (var j = id; j >= 1; j--) if (Map[i, j-1] >= 0) {
                Map[i,j] = Map[i, j-1];
            }

            for (var i = 0; i < Width; i++) if (Map[i, 0] > 0) {
                Map[i, 0] = 0;
            }

            Thread.Sleep(45);
            Score++;
            Redraw();
        }

        static bool NeedRestart = true;

        public static void NextShape()
        {
            var random = new Random();
            if (!AddShape(random.Next(Shapes.Count), 0, 0))
            {
                NeedRestart = true;
            }
        }

        public static void AttachShape()
        {
            for (var i = 0; i < Width; i++) for (var j = 0; j < Height; j++) {
                Map[i, j] = Map[i,j]>0? Map[i, j] : -Map[i,j];
            }

            for (int i = Height - 1; i >= 0; i--) {
                if (RowIsFull(i))
                {
                    RemoveRow(i);
                    i++;
                }
            }

            NextShape();
        }

        public static bool ShapeFits(int[] shape, int _i, int _j)
        {
            for (int l = 0; l < shape.Length; l++) {
                int i = l % 3, j = l / 3;
                if (shape[l] != 1) continue;
                if (!exists(_i + i, _j + j) || (Map[_i+i,_j+j] == 1)) return false;
            }
            return true;
        }

        public static bool AddShape(int id, int _i, int _j)
        {
            var shape = Shapes[id];
            if(!ShapeFits(shape, _i, _j)) return false;

            for (int l = 0; l < shape.Length; l++) {
                int i = l % 3, j = l / 3;
                if (shape[l] != 0)
                    Map[_i + i, _j + j] = -1;
            }

            return true;
        }

        public static void CheckObstructions(out bool left, out bool right, out bool down)
        {
            left = right = down = false;
            for (var i = 0; i < Width; i++) for (var j = 0; j < Height; j++) {
                if (Map[i, j] == -1) {
                    if (exists(i-1, j))  left |= Map[i-1, j] > 0;
                    else                 left |= true;

                    if (exists(i+1, j))  right |= Map[i+1, j] > 0;
                    else                 right |= true;

                    if(exists(i, j + 1)) down |= Map[i, j+1] > 0;
                    else                 down |= true;
                }
            }
        }
    }
}
