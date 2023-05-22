using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimplePowerToysApp.Hooks
{
    class WindowOperations
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSysColor(int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);


        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOACTIVATE = 0x0010;
        const uint SWP_SHOWWINDOW = 0x0040;
        const int GWL_STYLE = -16;
        const int WS_TILEDWINDOW = 0x00CF0000;


        uint processId;
        Process currentProcess;
        IntPtr hWnd;
        public WindowOperations ()
        {
            hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out processId);
            currentProcess = Process.GetProcessById((int)processId);
        }
        public void KeepOnTop() {
            SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        public void KeepOnBottom()
        {
            SetWindowPos(hWnd, new IntPtr(-2), 0, 0, 0, 0,
                               SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        public void ToggleTopBottom()
        {
            if (IsOnTop())
            {
                KeepOnBottom();
            }
            else
            {
                KeepOnTop();
            }
        }

        public bool IsOnTop()
        {
            const int GWL_EXSTYLE = -20;
            const int WS_EX_TOPMOST = 0x00000008;
            IntPtr exStylePtr = GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            return ((exStylePtr.ToInt64() & WS_EX_TOPMOST) != 0);
        }

        public void SetBorder()
        {
            const int WS_BORDER = 0x00800000;
            IntPtr style = GetWindowLongPtr(hWnd, GWL_STYLE);
            style = (IntPtr) ((int) style | WS_BORDER);
            SetWindowLongPtr(hWnd, GWL_STYLE, (IntPtr)style);

            uint border_color = (uint)GetSysColor(8); // retrieve blue color
            uint flags = 0x11;
            byte alpha = 255;
            SetLayeredWindowAttributes(hWnd, border_color, alpha, flags);
        }
        public bool HasWindowButtons()
        {
            IntPtr style = GetWindowLongPtr(hWnd, GWL_STYLE);
            return ((style.ToInt64() & WS_TILEDWINDOW) != 0);
        }
        public void RemoveWindowButtons()
        {
            IntPtr style = GetWindowLongPtr(hWnd, GWL_STYLE);
            style = new IntPtr((long)style & ~(WS_TILEDWINDOW));
            SetWindowLongPtr(hWnd, GWL_STYLE, style);
        }
        public void AddWindowButtons()
        {
            IntPtr style = GetWindowLongPtr(hWnd, GWL_STYLE);
            style = new IntPtr((long)style | WS_TILEDWINDOW);
            SetWindowLongPtr(hWnd, GWL_STYLE, style);
        }

        public void ToggleWindowButtons()
        {
            if (HasWindowButtons())
            {
                RemoveWindowButtons();
            }
            else
            {
                AddWindowButtons();
            }
        }
    }
}
