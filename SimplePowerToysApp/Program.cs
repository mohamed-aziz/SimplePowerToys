using Microsoft.VisualBasic;
using SimplePowerToysApp.Hooks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimplePowerToysApp
{
    internal static class Program
    {
        private static NotifyIcon _notifyIcon;
        private static ContextMenuStrip _contextMenuStrip;
        private static Icon myIcon = Properties.Resources.cup_coffee;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ShowConsole.HideTheConsole();
            Hooks.Hooks hooks = new Hooks.Hooks();
            _notifyIcon = new NotifyIcon()
            {
                Icon = myIcon,
                Text = "SimplePowerToys"
            };
            _notifyIcon.Visible = true;
            SetupContextMenu();
            Application.Run();
            hooks.Unhook();
        }

        static void SetupContextMenu()
        {
            _contextMenuStrip = new ContextMenuStrip();
            var menuItem = new ToolStripMenuItem("Prevent Sleep");
            menuItem.Click += Prevent_Sleep;
            menuItem.CheckOnClick = true;
            _contextMenuStrip.Items.Add(menuItem);
            _contextMenuStrip.Items.Add("About", null, About_Click);
            _contextMenuStrip.Items.Add("Exit", null, Exit_Click);
            _notifyIcon.ContextMenuStrip = _contextMenuStrip;
        }

        static void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        static void About_Click(object sender, EventArgs e)
        {
            Forms.AboutForm aboutForm = new Forms.AboutForm();
            aboutForm.Show();
        }
        public static void Prevent_Sleep(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Checked)
            {
                Debug.WriteLine("Prevent Sleep");
                Hooks.PreventSleep.Prevent();
            }
            else
            {
                Debug.WriteLine("Restore Sleep");
                Hooks.PreventSleep.Restore();
            }
        }
    }
}