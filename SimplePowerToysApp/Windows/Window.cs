using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimplePowerToysApp.Windows
{
    public class Window
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public Window(IntPtr hWnd, string title = "", string process_name = "")
        {
            Title = title;
            HWnd = hWnd;
            ProcessName = process_name;
        }

        public string Title { get; set; }
        public IntPtr HWnd { get; set; }

        public string ProcessName { get; set; }

        public void SwitchToWindow()
        {
            const int SW_RESTORE = 9;
            ShowWindow(HWnd, SW_RESTORE);
            SetForegroundWindow(HWnd);
        }
    }
}
