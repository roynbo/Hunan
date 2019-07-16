﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace NewUI_test_2.Untiy
{
    public partial class UnityControl : UserControl
    {
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);
        public UnityControl()
        {
            InitializeComponent();
            this.Load += UnityControl_Load;
            panel1.Resize += panel1_Resize;
        }
        private void UnityControl_Load(object sender, EventArgs e)
        {
            try
            {
                process = new Process();
                process.StartInfo.FileName = Application.StartupPath + @"\unityAPP\game.exe";
                process.StartInfo.Arguments = "-parentHWND " + panel1.Handle.ToInt32() + " " + Environment.CommandLine;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.WaitForInputIdle();
                // Doesn't work for some reason ?!
                //unityHWND = process.MainWindowHandle;
                EnumChildWindows(panel1.Handle, WindowEnum, IntPtr.Zero);

                unityHWNDLabel.Text = "Unity HWND: 0x" + unityHWND.ToString("X8");
            }
            catch (Exception ex)
            {
                unityHWNDLabel.Text = ex.Message;
                //MessageBox.Show(ex.Message);
            }
        }

        internal void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        internal void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, panel1.Width, panel1.Height, true);
            ActivateUnityWindow();
        }

        // Close Unity application
        internal void Form1_FormClosed()
        {
            try
            {
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (process.HasExited == false)
                    process.Kill();
            }
            catch (Exception)
            {

            }
        }

        internal void Form1_Activated()
        {
            ActivateUnityWindow();
        }

        internal void Form1_Deactivate()
        {
            DeactivateUnityWindow();
        }

    }
}
