using SimplePowerToysApp.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SimplePowerToysApp.Forms
{
    public partial class WindowSwitcher : Form
    {
        private bool HideForm = true;
        public List<Windows.Window> Windows_ { get; set; }
        private Dictionary<string, IntPtr> windowMap = new Dictionary<string, IntPtr>();
        Windows.WindowList windowList;
        public WindowSwitcher()
        {
            InitializeComponent();

            Visible = false;
            TopMost = false;
            // centers in current screen
            textBox1.KeyDown += new KeyEventHandler(textBox1_KeyDown);
            listView1.DrawItem += new DrawListViewItemEventHandler(listView1_DrawItem);
            listView1.MultiSelect = false;
            listView1.OwnerDraw = true;
            listView1.HideSelection = true;
            listView1.HeaderStyle = ColumnHeaderStyle.None;
            VisibleChanged += new EventHandler(WindowSwitcher_VisibleChanged);
            windowList = new Windows.WindowList();
        }

        public void ToggleHideShow()
        {
            HideForm = !HideForm;

            if (HideForm)
            {
                // TopMost = false;
                Hide();
            }
            else
            {
                // TopMost = true;
                Show();
                Center();
                windowList.UpdateWindowList();
                UpdateWindowsList(windowList.Windows);
                FocusOnSearchBox();
            }
        }

        private void WindowSwitcher_VisibleChanged(object sender, EventArgs e)
        {
            /*
            if (Visible)
            {
                windowList.UpdateWindowList();
                Center();
                UpdateWindowsList(windowList.Windows);
                FocusOnSearchBox();
            }
            else
            {
                ActiveControl = null;
            } */
        }

        private void SetSelected(int idx)
        {
            listView1.Items[idx].Selected = true;
        }

        public void Center()
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            Screen currentScreen = Screen.FromPoint(new Point(x, y));

            Rectangle workingArea = currentScreen.WorkingArea;

            Left = workingArea.Left + (workingArea.Width - this.Width) / 2;
            Top = workingArea.Top + (workingArea.Height - this.Height) / 2;
        }

        public void UpdateWindowsList(List<Windows.Window> windows)
        {
            listView1.Items.Clear();
            listView1.Columns.Add("Windows", -2, HorizontalAlignment.Left);
            foreach (var window in windows)
            {
                listView1.Items.Add(window.Title);
                windowMap[window.Title] = window.HWnd;
            }
            if (windows.Count > 0)
            {
                SetSelected(0);
            }
            Windows_ = windows;
        }

        private void WindowSwitcher_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchQuery = textBox1.Text.ToLower();
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (var window in Windows_)
            {
                if (window.Title.ToLower().Contains(searchQuery))
                {
                    listView1.Items.Add(window.Title);
                }
            }
            if (listView1.Items.Count > 0)
            {
                listView1.Items[0].Selected = true;
            }
            listView1.EndUpdate();
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(Brushes.RoyalBlue, e.Bounds);
                e.Item.ForeColor = Color.White;
            }
            else
            {
                e.DrawDefault = true;
                e.Item.ForeColor = Color.Black;
            }
            e.DrawText();
            // e.Graphics.DrawString(e.Item.Text, e.Item.Font, Brushes.Black, e.Bounds);
            // e.DrawFocusRectangle();
        }

        public void FocusOnSearchBox()
        {
            // Activate();
            // BringToFront();
            Windows.Window.SetForegroundWindow(Handle);
            Windows.Window.SetFocus(Handle);
            // Focus();
            // textBox1.Focus();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (listView1.Items.Count == 0)
                {
                    return;
                }
                int currentSelectedIndex = listView1.SelectedIndices[0];
                currentSelectedIndex++;
                if (currentSelectedIndex >= listView1.Items.Count)
                {
                    currentSelectedIndex = 0;
                }
                    SetSelected(currentSelectedIndex);
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (listView1.Items.Count == 0)
                {
                    return;
                }
                int currentSelectedIndex = listView1.SelectedIndices[0];
                currentSelectedIndex--;
                if (currentSelectedIndex < 0)
                {
                    currentSelectedIndex = listView1.Items.Count - 1;
                }
                SetSelected(currentSelectedIndex);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                // Switch to the selected window
                int currentSelectedIndex = listView1.SelectedIndices[0];
                // get item
                string windowTitle = listView1.Items[currentSelectedIndex].Text;
                IntPtr hwnd;
                if (windowMap.TryGetValue(windowTitle, out hwnd))
                {
                    var wnd = new Windows.Window(hwnd);
                    wnd.SwitchToWindow();
                }
                Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
