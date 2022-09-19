using KeyboardHookManager;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MMDWheel
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_control(Int32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);
        private const int click_left = 0x02;
        private const int unclick_left = 0x04;
        private const int click_right = 0x08;
        private const int unclick_right = 0x10;
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);


        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const string mmdProcessName = "MikuMikuDance";
        private int mmdProcessId = 0;
        RECT rect;

        public Form1()
        {
            InitializeComponent();
            HookManager.KeyDown += HookManager_KeyDown;
            HookManager.MouseWheel += HookManager_MouseWheel;
            HookManager.MouseMove += HookManager_MouseMove;
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            IntPtr handle = GetForegroundWindow();
            RECT rect;
            bool flag = GetWindowRect(handle, out rect);
            Text = rect.top.ToString() + "," + rect.right.ToString();
        }

        private void HookManager_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                Text = Text + "X";
                if (GetActiveProcessName() == mmdProcessName)
                {
                    Cursor.Position = new System.Drawing.Point(rect.left, rect.top);
                    MouseEvent(MouseEventFlags.LeftDown);
                    Cursor.Position = new System.Drawing.Point(rect.left, rect.top - 5);
                    MouseEvent(MouseEventFlags.LeftUp);
                }
            }
            if (e.Delta > 0)
            {
                Text = Text + "Y";
            }
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    Text = Text + "Q";
                    if (this.Focused)
                    {
                        Text = Text + "A";
                    }
                    else
                    {
                        string proc = GetActiveProcessName();
                        if (proc == mmdProcessName)
                        {
                            this.Activate();
                        }
                    }
                    break;
            }
        }
        public string GetActiveProcessName()
        {
            int processid;
            try
            {
                GetWindowThreadProcessId(GetForegroundWindow(), out processid);
                if (0 != processid)
                {
                    Process p = Process.GetProcessById(processid);
                    mmdProcessId = processid;
                    return p.ProcessName;
                }
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            SetWindowRect();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            SetWindowRect();
        }
        private void SetWindowRect()
        {
            IntPtr handle = GetForegroundWindow();
            bool flag = GetWindowRect(handle, out rect);
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            SetWindowRect();
        }
    }


}
