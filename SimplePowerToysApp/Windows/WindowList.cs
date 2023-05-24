using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimplePowerToysApp.Windows
{
    class WindowList
    {
        public List<Window> Windows { get; set; }

        public WindowList()
        {
            Windows = new List<Window>();
        }



        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);


        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private List<IntPtr> windowHandles;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);


        public List<IntPtr> GetWindowHandles()
        {
            windowHandles = new List<IntPtr>();
            EnumWindows(new EnumWindowsProc(EnumWindow), IntPtr.Zero);
            return windowHandles;
        }

        private bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            if (IsWindowVisible(hWnd))
            {
                windowHandles.Add(hWnd);
            }
            return true;
        }


        public void AddWindow(Window window)
        {
            Windows.Add(window);
        }

        public void RemoveWindow(Window window)
        {
            Windows.Remove(window);
        }

        private void Clear()
        {
            Windows.Clear();            
        }

        private string GetProcessName(IntPtr hwnd)
        {
            uint processId;
            GetWindowThreadProcessId(hwnd, out processId);

            // get the process name from the process ID
            Process process = Process.GetProcessById((int)processId);
            string processName = process.ProcessName;

            return processName;
        }


        private String? GetWindowTitle(IntPtr hWnd)
        {
            try
            {
                var len = GetWindowTextLength(hWnd);
                var sb = new StringBuilder(len + 1);
                GetWindowText(hWnd, sb, sb.Capacity);
                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }
        public void UpdateWindowList()
        {
            Clear();
            GetWindowHandles();

            for (int i = 0; i < windowHandles.Count; ++i)
            {
                var wndHandle = windowHandles[i];
                var wndTitle = GetWindowTitle(wndHandle);
                if (wndTitle == null || wndTitle == "")
                {
                    continue;
                }
                var window = new Window(wndHandle, wndTitle);
                AddWindow(window);
            }
        }
    }
}
