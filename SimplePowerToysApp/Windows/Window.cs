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
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        // Define the necessary constants
        const int SW_SHOWMAXIMIZED = 3;
        const int SW_SHOWNORMAL = 1;

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
[return: MarshalAs(UnmanagedType.Bool)]
private static extern bool IsIconic(IntPtr hWnd);


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
            WINDOWPLACEMENT windowPlacement = new WINDOWPLACEMENT();
            windowPlacement.length = Marshal.SizeOf(windowPlacement);
            GetWindowPlacement(HWnd, ref windowPlacement);

            const int SW_RESTORE = 9;
            if (IsIconic(HWnd))
            {
                ShowWindow(HWnd, SW_RESTORE);
            }
            else
            {
                SetWindowPlacement(HWnd, ref windowPlacement);
            }

            SetForegroundWindow(HWnd);
        }

        public void ClickOnWindow()
        {
            const uint WM_LBUTTONDOWN = 0x0201;
            const uint WM_LBUTTONUP = 0x0202;

            int x = 0;
            int y = 0;
            IntPtr lParam = (IntPtr)((y << 16) | x);

            SendMessage(HWnd, WM_LBUTTONDOWN, IntPtr.Zero, lParam);

            SendMessage(HWnd, WM_LBUTTONUP, IntPtr.Zero, lParam);

        }

        public static void MoveCursorNextScreen()
        {
            Screen currentScreen = Screen.FromPoint(Cursor.Position);
            int currentScreenIndex = Screen.AllScreens.ToList().IndexOf(currentScreen);

            Screen nextScreen = Screen.AllScreens[(currentScreenIndex + 1) % Screen.AllScreens.Length];
            Rectangle nextBounds = nextScreen.Bounds;

            // Calculate the center coordinates of the next screen
            int nextX = nextBounds.Left + (nextBounds.Width / 2);
            int nextY = nextBounds.Top + (nextBounds.Height / 2);

            Cursor.Position = new Point(nextX, nextY);
        }

        public static void MoveCursorPreviousScreen()
        {
            Screen currentScreen = Screen.FromPoint(Cursor.Position);
            int currentScreenIndex = Screen.AllScreens.ToList().IndexOf(currentScreen);

            int previousScreenIndex = ((currentScreenIndex - 1) + Screen.AllScreens.Length) % Screen.AllScreens.Length;
            Screen previousScreen = Screen.AllScreens[previousScreenIndex];
            Rectangle previousBounds = previousScreen.Bounds;

            // Calculate the center coordinates of the next screen
            int previousX = previousBounds.Left + (previousBounds.Width / 2);
            int previousY = previousBounds.Top + (previousBounds.Height / 2);

            Cursor.Position = new Point(previousX, previousY);
        }
    }
}
