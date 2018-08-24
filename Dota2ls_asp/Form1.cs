using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2ls_asp
{
    public partial class Form1 : Form
    {


        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        public const int HSHELL_WINDOWCREATED = 1;
        public delegate bool WindowEnumCallback(int hwnd, int lparam);
        public int uMsgNotify;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(WindowEnumCallback lpEnumFunc, int lParam);
        [DllImport("user32.dll")]
        public static extern void GetWindowText(IntPtr h, StringBuilder s, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(int h);
        [DllImport("user32.dll")]
        private static extern int RegisterWindowMessage(string lpString);
        [DllImport("user32.dll")]
        private static extern int RegisterShellHookWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern void FlashWindow(IntPtr a, bool b);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        //Нажатие на левую кнопку мыши
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //Поднятие левой кнопки мыши
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        //перемещение указателя мыши
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x00000008;
        private const int MOUSEEVENTF_RIGHTUP = 0x00000010;

        int resx, resy;

        private const int BUTTON_X = 750;
        private const int BUTTON_Y = 539;
        private IntPtr dotaWindow;
        Rectangle resolution;
        bool DotaCheck = false;
        int Time = 30;

        public Form1()
        {
            InitializeComponent();
            Visible = false;
            ShowInTaskbar = false;
            Enabled = true;
            getHandle();
            setupWindowListener();
            this.Visible = false;
          

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //timer1.Enabled = true;
        }
        


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == uMsgNotify)
            {
                IntPtr handle = m.LParam;
                StringBuilder sb = new StringBuilder(255);
                switch (m.WParam.ToInt32())
                {
                    case HSHELL_WINDOWCREATED:
                        break;
                    default:
                        if (!DotaCheck && DotaEnabled() && !timer1.Enabled) { Time = 15; timer1.Enabled = true; }
                        GetWindowText(handle, sb, sb.Capacity);
                        var test = sb.ToString().Equals("Dota 2");
                        if (sb.ToString().Equals("Dota 2") && m.WParam.ToInt32() == 6)
                        {
                            //MessageBox.Show(m.Msg + "  " + uMsgNotify + " Handle " + handle);
                            if  (!timer1.Enabled) timer1.Enabled = true;
                            dotaWindow = handle;
                            Task.Factory.StartNew(() =>
                            {
                                //label1.Text = "Найдена";
                                getHandle();
                                Thread.Sleep(3000);
                                if (Time >= 30 && DotaEnabled())
                                {
                                    this.Invoke(new Action(() => clickAccept()));
                                    

                                    Time = 0;
                                }
                            });
                        }
                        break;
                }
            }

            base.WndProc(ref m);

        }

        public void getHandle()
        {
            dotaWindow = FindWindow(null, "Dota 2");
        }

        private bool DotaEnabled()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (process.ProcessName == "dota2")
                {
                    return true;

                }
            }
            return false;
        }

        private void clickAccept()
        {
            Console.WriteLine("Setting Dotawindow " + dotaWindow.ToString() + " to foreground");
            SetForegroundWindow(dotaWindow);
            //Task.Factory.StartNew(() =>
           // {
                Thread.Sleep(3000);
                ClickOnPoint(dotaWindow, getButtonCoordinates());
                ClickOnPoint(dotaWindow, getButtonCoordinates());

          //  });
        }
        private Point getButtonCoordinates()
        {
            resolution = Screen.PrimaryScreen.Bounds;
            // return new Point(BUTTON_X * resolution.Width / 1920, BUTTON_Y * resolution.Height / 1080);
            return new Point(resolution.Width/2, resolution.Height/2-150);
        }

        private void setupWindowListener()
        {
            uMsgNotify = RegisterWindowMessage("SHELLHOOK");
            RegisterShellHookWindow(this.Handle);
        }
        private void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
        {
            Point oldPoint;
            GetCursorPos(out oldPoint);

            /// get screen coordinates
            ClientToScreen(wndHandle, ref clientPoint);

            /// set cursor on coords, and press mouse

            for (int i = 0; i < 30; i++)
            {
                SetCursorPos(resolution.Width / 2, resolution.Height / 2 + i*10);
                // SetCursorPos(clientPoint.X, clientPoint.Y+i*10);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero); /// left mouse button down
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero); /// left mouse button up
                Thread.Sleep(10);
                SetCursorPos(resolution.Width / 2, resolution.Height / 2 - i * 10);
                //SetCursorPos(clientPoint.X, clientPoint.Y + i*10);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero); /// left mouse button down
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero); /// left mouse button up
            }
            // SetCursorPos((resolution.Width / 2), ((resolution.Height / 2) - 50));
            // mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (resolution.Width / 2), ((resolution.Height / 2)), 0, 0);


            /// return mouse 
            SetCursorPos(oldPoint.X, oldPoint.Y);

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (process.ProcessName == "dota2")
                {
                    DotaCheck = true;

                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Time++;
            
            // var dota2ls = Process.GetProcessesByName("Dota2ls");
            //MessageBox.Show(dota2ls.Count().ToString());
            Process[] processlist = Process.GetProcesses();
            var p = processlist.Count(x => x.ProcessName == "Dota2ls");
           // var p1 = processlist.Count(x => x.ProcessName == "Dota2ls_asp");
            // MessageBox.Show(p.ToString());
            if (p == 0) this.Close();
            //if (p1 >0) this.Close();

        }
    }
}
