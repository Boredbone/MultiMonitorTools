using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiMonitorTools.Native;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;

namespace MultiMonitorTools
{

    public class WindowInformation
    {
        public string Title { get; set; }
        public IntPtr Handle { get; set; }
        public int Id { get; set; }

        public WindowInformation()
        {

        }

        public WindowInformation(IntPtr handle)
        {
            this.Handle = handle;
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public static class WindowRelocate
    {


        /// <summary>
        /// 全ウインドウを列挙
        /// </summary>
        /// <returns></returns>
        public static List<WindowInformation> EnumerateWindows()
        {

            var windows = new List<WindowInformation>();

            WindowPlacement.EnumWindows((handle, parameter) =>
            {
                var sb = new StringBuilder(0x1024);

                if (WindowPlacement.IsWindowVisible(handle) != 0
                    && WindowPlacement.GetWindowText(handle, sb, sb.Capacity) != 0)
                {

                    var window = new WindowInformation()
                    {
                        Title = sb.ToString(),
                        Handle = handle,
                        Id = (int)WindowPlacement.GetWindowThreadProcessId(handle, IntPtr.Zero),
                    };

                    var title = sb.ToString();

                    if (windows.Count == 0 
                        || windows.Last().Id != window.Id
                        || windows.Last().Title != window.Title)
                    {
                        windows.Add(window);
                    }

                }
                return 1;
            }, 0);

            if (windows.Count > 0 && windows.Last().Title == "Program Manager")
            {
                windows.RemoveAt(windows.Count - 1);
            }

            return windows;
        }

        /// <summary>
        /// ウインドウを指定された位置と大きさに配置
        /// </summary>
        /// <param name="window"></param>
        /// <param name="positon"></param>
        public static void Relocate(this WindowInformation window, Rectangle positon)
        {

            /*
            // メイン・ウィンドウが最小化されていれば元に戻す
            if (IsIconic(hWnd))
            {
                ShowWindowAsync(hWnd, (int)ShowWindowCommands.Restore);
            }
            // メイン・ウィンドウを最前面に表示する
            SetActiveWindow(hWnd);
            SetForegroundWindow(hWnd);*/

            var placement = new WindowPlacement.WINDOWPLACEMENT();

            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));



            WindowPlacement.GetWindowPlacement(window.Handle, ref placement);

            int width = placement.NormalPosition.Right - placement.NormalPosition.Left;
            int height = placement.NormalPosition.Bottom - placement.NormalPosition.Top;

            placement.ShowCmd = WindowPlacement.ShowWindowCommands.Restore;
            placement.NormalPosition.Top = positon.Top;
            placement.NormalPosition.Left = positon.Left;

            if (positon.Top >= positon.Bottom || positon.Right <= positon.Left)
            {
                placement.NormalPosition.Bottom = placement.NormalPosition.Top + height;
                placement.NormalPosition.Right = placement.NormalPosition.Left + width;
            }
            else
            {
                placement.NormalPosition.Bottom = positon.Bottom;
                placement.NormalPosition.Right = positon.Right;
            }

            placement.Flags = 0;

            placement.ShowCmd =
                (placement.ShowCmd == WindowPlacement.ShowWindowCommands.ShowMinimized)
                ? WindowPlacement.ShowWindowCommands.Normal
                : placement.ShowCmd;


            WindowPlacement.SetWindowPlacement(window.Handle, ref placement);
            
            WindowPlacement.SetForegroundWindow(window.Handle);
        }

        /// <summary>
        /// ウインドウの大きさを変えずに指定された位置に配置
        /// </summary>
        /// <param name="window"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public static void Relocate(this WindowInformation window, int left, int top)
        {
            window.Relocate(new Rectangle(left, top, -1, -1));
        }

        /// <summary>
        /// 一番上にあるウインドウを指定された位置と大きさに配置
        /// </summary>
        /// <param name="positon"></param>
        public static void RelocateTop(Rectangle positon)
        {
            int thisProcessId = -1;// System.Diagnostics.Process.GetCurrentProcess().Id;
            

            WindowInformation window = null;
            

            WindowPlacement.EnumWindows((handle, parameter) =>
            {
                var sb = new StringBuilder(0x1024);
                if (WindowPlacement.IsWindowVisible(handle) != 0
                    && WindowPlacement.GetWindowText(handle, sb, sb.Capacity) != 0)
                {
                    var id = (int)WindowPlacement.GetWindowThreadProcessId(handle, IntPtr.Zero);

                    if (thisProcessId < 0)
                    {
                        thisProcessId = id;
                    }
                    else if (id != thisProcessId)
                    {
                        window = new WindowInformation()
                        {
                            Title = sb.ToString(),
                            Handle = handle,
                            Id = id,
                        };

                        return 0;
                    }
                }
                return 1;
            }, 0);

            if (window != null)
            {
                window.Relocate(positon);
            }

        }

        /// <summary>
        /// ウインドウの位置と大きさを取得
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Rectangle GetPlacement(this WindowInformation window)
        {
            var placement = new WindowPlacement.WINDOWPLACEMENT();

            placement.Length = Marshal.SizeOf(typeof(WindowPlacement.WINDOWPLACEMENT));

            WindowPlacement.GetWindowPlacement(window.Handle, ref placement);

            var position = placement.NormalPosition;


            return new Rectangle(position.Left, position.Top, position.Right - position.Left, position.Bottom - position.Top);
        }
    }
}
