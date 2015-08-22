using System;
using System.Threading;
using System.Threading.Tasks;
using MultiMonitorTools.Settings;
using MultiMonitorTools.Native;

namespace MultiMonitorTools.Models
{
    /// <summary>
    /// 画面の回転を管理
    /// </summary>
    class DisplayRotate
    {
        private string DeviceName { get; }
        private DEVMODE Mode { get; }
        //public DisplayOrientations Orientation { get; }
        //public int Increment { get; }

        private DisplayRotate(string name, DEVMODE mode)
        {
            this.DeviceName = name;
            this.Mode = mode;
        }

        /// <summary>
        /// 画面を指定量回転させるためのオブジェクトを生成
        /// </summary>
        /// <param name="monitorID">ディスプレイのデバイス番号</param>
        /// <returns>生成に失敗した場合はnull</returns>
        public static DisplayRotate Generate(int monitorID)
        {

            var mode = new DeviceMode(monitorID);

            if (!mode.IsSucceeded)
            {
                return null;
            }

            //var dm = mode.Mode;

            //if (increment % 2 == 1)
            //{
            //    // swap width and height
            //    int temp = dm.dmPelsHeight;
            //    dm.dmPelsHeight = dm.dmPelsWidth;
            //    dm.dmPelsWidth = temp;
            //}
            //var newOrientation = (dm.dmDisplayOrientation + increment) % 4;

            //if (newOrientation == DisplayDevice.DMDO_DEFAULT
            //    || newOrientation == DisplayDevice.DMDO_90
            //    || newOrientation == DisplayDevice.DMDO_180
            //    || newOrientation == DisplayDevice.DMDO_270)
            //{
            //    dm.dmDisplayOrientation = newOrientation;
            //}
            //else
            //{
            //    return null;
            //}

            return new DisplayRotate(mode.Name, mode.Mode);
        }


        /// <summary>
        /// 画面を回転
        /// </summary>
        /// <param name="increment">回転量</param>
        /// <returns></returns>
        public int Rotate(int increment)
        {

            var deviceMode = this.Mode;

            //奇数量回転の場合は高さと幅を入れ替える
            if (increment % 2 == 1)
            {
                // swap width and height
                int temp = deviceMode.dmPelsHeight;
                deviceMode.dmPelsHeight = deviceMode.dmPelsWidth;
                deviceMode.dmPelsWidth = temp;
            }
            var newOrientation = (deviceMode.dmDisplayOrientation + increment) % 4;

            //if (newOrientation == DisplayDevice.DMDO_DEFAULT
            //    || newOrientation == DisplayDevice.DMDO_90
            //    || newOrientation == DisplayDevice.DMDO_180
            //    || newOrientation == DisplayDevice.DMDO_270)
            //{

            deviceMode.dmDisplayOrientation = newOrientation;

            //}
            //else
            //{
            //    return null;
            //}



            //var deviceName = this.DeviceName;
            //var deviceMode = dm;
            //var orientation = this.Orientation;

            var result = DisplayDevice.ChangeDisplaySettingsEx
                (this.DeviceName, ref deviceMode, IntPtr.Zero, DisplayDevice.CDS_UPDATEREGISTRY, IntPtr.Zero);

            //DISP_CHANGE_SUCCESSFUL = 0: Indicates that the function succeeded.
            //DISP_CHANGE_BADMODE = -2: The graphics mode is not supported.
            //DISP_CHANGE_FAILED = -1: The display driver failed the specified graphics mode.
            //DISP_CHANGE_RESTART = 1: The computer must be restarted for the graphics mode to work.

            /*
            if (NativeMethods.DISP_CHANGE_SUCCESSFUL != iRet)
            {
                // add exception handling here
            }*/


            if (increment != 0)
            {
                DisplayDevice.ChangeDisplaySettingsEx
                    (this.DeviceName, ref deviceMode, IntPtr.Zero, 0, IntPtr.Zero);
            }

            return result;
        }

    }
}
