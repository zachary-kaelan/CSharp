using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestGame
{
    public partial class Form1 : Form
    {
        static int x;
        static int y;
        static Graphics gfx;
        public bool drawing = false;

        static int oldX;
        static int oldY;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _mouseHookID = SetHook(_mouseProc);
            _keyHookID = SetHook(_keyProc);

            x = this.Top;
            y = this.Left;
            gfx = this.CreateGraphics();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            x = this.Top;
            y = this.Left;
        }

        private void Form1_CursorChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            drawing = true;
            gfx.FillRectangle(Brushes.Black, e.X, e.Y, 1, 1);
            oldX = e.X;
            oldY = e.Y;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
            gfx.FillRectangle(Brushes.Black, e.X, e.Y, 1, 1);
        }

        private static IntPtr MouseDrawHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookstruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                gfx.FillRectangle(
                    Brushes.Black,
                    hookstruct.pt.x,
                    hookstruct.pt.y,
                    1, 1
                );
            }

            return CallNextHookEx(
                _mouseHookID,
                nCode, wParam, lParam
            );
        }

        private static IntPtr KeyHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(
                _keyHookID,
                nCode, wParam, lParam
            );
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            x = this.Top;
            y = this.Left;
        }

        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelMouseProc _mouseProc = MouseDrawHook;// = MouseHookCallBack;
        private static LowLevelKeyboardProc _keyProc = KeyHook;// = KeyboardHookCallback;

        public static IntPtr _mouseHookID = IntPtr.Zero;
        public static IntPtr _keyHookID = IntPtr.Zero;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        public enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwTHreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwTHreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static void Start(IntPtr mouseCallback, IntPtr keyCallback)
        {
            _mouseProc = mouseCallback == IntPtr.Zero ?
                MouseHookCallBack :
                Marshal.
                GetDelegateForFunctionPointer<LowLevelMouseProc>(
                    mouseCallback
            );
            _keyProc = keyCallback == IntPtr.Zero ?
                MouseHookCallBack :
                Marshal.
                GetDelegateForFunctionPointer<LowLevelKeyboardProc>(
                    keyCallback
            );

            _mouseHookID = SetHook(_mouseProc);
            _keyHookID = SetHook(_keyProc);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProc = Process.GetCurrentProcess())
            {
                using (ProcessModule curMod = curProc.MainModule)
                {
                    return SetWindowsHookEx(
                        WH_MOUSE_LL,
                        proc,
                        GetModuleHandle(curMod.ModuleName),
                        0);
                }
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProc = Process.GetCurrentProcess())
            {
                using (ProcessModule curMod = curProc.MainModule)
                {
                    return SetWindowsHookEx(
                        WH_KEYBOARD_LL,
                        proc,
                        GetModuleHandle(curMod.ModuleName),
                        0);
                }
            }
        }

        private static IntPtr MouseHookCallBack(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
            }

            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)vkCode);
            }

            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(_mouseHookID);
            UnhookWindowsHookEx(_keyHookID);
        }
    }
}
