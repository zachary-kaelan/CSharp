using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASCV4EXTMENULib;
using ZachLib;

namespace LowLevelTesting
{
    class Program
    {
        /*[DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool GetCurrentConsoleFontEx(
               IntPtr consoleOutput,
               bool maximumWindow,
               ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(
               IntPtr consoleOutput,
               bool maximumWindow,
               CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        private const int STD_OUTPUT_HANDLE = -11;
        private const int TMPF_TRUETYPE = 4;
        private const int LF_FACESIZE = 32;
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);*/

        public const int MAX_VALUE = ushort.MaxValue * 256;

        static void Main(string[] args)
        {
            GC.TryStartNoGCRegion(1000000);
            for (int i = 0; i < 8; ++i)
            {
                TestPrefix();
                Thread.Sleep(500);
                TestPostfix();
                Thread.Sleep(500);
            }
            GC.EndNoGCRegion();

            Console.ReadLine();
        }

        public static void TestPrefix()
        {
            long temp = 0;
            Stopwatch timer = Stopwatch.StartNew();
            for (int i = 0; i < MAX_VALUE; ++i)
            {
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
                ++temp;
            }
            timer.Stop();
            Console.WriteLine("Prefix: {0} ms, {1} ticks", timer.ElapsedMilliseconds, timer.ElapsedTicks);
        }

        public static void TestPostfix()
        {
            long temp = 0;
            Stopwatch timer = Stopwatch.StartNew();
            for (int i = 0; i < MAX_VALUE; i++)
            {
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
                temp++;
            }
            timer.Stop();
            Console.WriteLine("Postfix: {0} ms, {1} ticks", timer.ElapsedMilliseconds, timer.ElapsedTicks);
        }

        /*[StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;

            internal COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CONSOLE_FONT_INFO_EX
        {
            internal uint cbSize;
            internal uint nFont;
            internal COORD dwFontSize;
            internal int FontFamily;
            internal int FontWeight;
            internal fixed char FaceName[LF_FACESIZE];
        }*/
    }
}
