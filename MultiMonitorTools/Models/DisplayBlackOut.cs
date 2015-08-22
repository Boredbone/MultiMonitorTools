using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MultiMonitorTools
{
    /// <summary>
    /// 画面を暗転させる
    /// </summary>
    class DisplayBlackOut : IDisposable
    {
        public const int AW_HIDE = 0x10000;
        public const int AW_ACTIVATE = 0x20000;
        public const int AW_SLIDE = 0x40000;
        public const int AW_BLEND = 0x80000;
        public const int AW_HOR_POSITIVE = 0x00000001;
        public const int AW_HOR_NEGATIVE = 0x00000002;
        public const int AW_VER_POSITIVE = 0x00000004;
        public const int AW_VER_NEGATIVE = 0x00000008;
        public const int AW_CENTER = 0x00000010;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);

        private List<Form> forms;// = new List<Form>();
        private int interval = 200;

        public DisplayBlackOut(int interval)
        {
            this.interval = interval;
            this.Out();
        }

        private void Out()
        {
            this.forms = Screen.AllScreens
                .Select(screen =>
                {
                    var length = screen.Bounds.Width > screen.Bounds.Height
                        ? screen.Bounds.Width
                        : screen.Bounds.Height;


                    return new Form()
                    {
                        BackColor = Color.Black,
                        ShowInTaskbar = false,
                        TopMost = true,
                        //WindowState = FormWindowState.Maximized,

                        Left = screen.Bounds.Left,
                        Top = screen.Bounds.Top,
                        Width = length,
                        Height = length,

                        FormBorderStyle = FormBorderStyle.None,

                    };
                })
                .ToList();

            

            foreach (var form in forms)
            {
                var top = form.Top;
                var left = form.Left;

                var result = AnimateWindow(form.Handle, interval, AW_BLEND | AW_ACTIVATE);
                //if (result != 0) Debug.WriteLine(Marshal.GetLastWin32Error());
                form.Show();
                form.Top = top;
                form.Left = left;
            }
        }

        private void In()
        {
            if (forms == null)
            {
                return;
            }
            foreach (var form in forms)
            {
                var result = AnimateWindow(form.Handle, interval, AW_BLEND | AW_HIDE);
                //if (result != 0) Debug.WriteLine(Marshal.GetLastWin32Error());
                form.Hide();
                form.Dispose();
            }
        }

        public void Dispose()
        {
            this.In();
        }
    }
}
