using SimplePowerToysApp.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimplePowerToysApp.Hooks
{
    class Hooks
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x104;

        private IntPtr hookId = IntPtr.Zero;
        
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        private WindowSwitcher switcher;
        public Hooks()
        {
            hookId = SetHook(HookCallback);
            switcher = new Forms.WindowSwitcher();
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(hookId);
        }

        private static IntPtr SetHook(HookProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                                         GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && ((wParam == (IntPtr)WM_KEYDOWN) || wParam == (IntPtr) WM_SYSKEYDOWN ))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // Check if Control + Alt + T is pressed
                if ((Control.ModifierKeys & Keys.Control) != 0 &&
                    (Control.ModifierKeys & Keys.Alt) != 0)
                {
                    var wnd = new WindowOperations();
                    if (vkCode == 0x54) // 'T'
                    {
                        wnd.ToggleTopBottom();
                    }
                    // 'Y'
                    else if (vkCode == 0x59)
                    {
                        wnd.ToggleWindowButtons();
                    }
                }

                // if Alt + Space is pressed
                if ((Control.ModifierKeys & Keys.Alt) != 0 && vkCode == 0x20)
                {
                    // Toggle WindowSwitcher form
                    if (switcher.IsDisposed)
                    {
                        Debug.WriteLine("WindowSwitcher is disposed");
                        switcher = new Forms.WindowSwitcher();
                    }
                    switcher.ToggleHideShow();
                    // switcher.Visible = !switcher.Visible;
                    return (IntPtr)1;
                }
                // if Control + Alt + Right arrow is pressed
                if ((Control.ModifierKeys & Keys.Control) != 0 &&
                                       (Control.ModifierKeys & Keys.Alt) != 0 &&
                                                          vkCode == 0x27)
                {
                    Windows.Window.MoveCursorNextScreen();
                    return (IntPtr)1;
                }
                // if Control + Alt + Left arrow is pressed
                if ((Control.ModifierKeys & Keys.Control) != 0 &&
                      (Control.ModifierKeys & Keys.Alt) != 0 &&
                      vkCode == 0x25)
                {
                    Windows.Window.MoveCursorPreviousScreen();
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
