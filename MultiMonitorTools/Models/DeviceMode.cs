using System;
using System.Runtime.InteropServices;
using MultiMonitorTools.Native;

namespace MultiMonitorTools
{
    /// <summary>
    /// DISPLAY_DEVICEとDEVMODEのラッパー
    /// </summary>
    public class DeviceMode
    {
        public DEVMODE Mode { get; private set; }
        public string Name { get; set; }
        public bool IsSucceeded { get; }

        public DeviceMode(int id)
        {
            this.IsSucceeded = this.GenerateNewMode(id);
        }


        private bool GenerateNewMode(int id)
        {
            var MyInfoEnumProc = new DISPLAY_DEVICE();
            MyInfoEnumProc.cb = Marshal.SizeOf(MyInfoEnumProc);//構造体のサイズを設定


            if (!DisplayDevice.EnumDisplayDevices(IntPtr.Zero, id, ref MyInfoEnumProc, 0))
            {
                return false;
            }

            // initialize the DEVMODE structure
            var dm = new DEVMODE();

            dm.dmDeviceName = new string(new char[32]);
            dm.dmFormName = new string(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);



            if (!DisplayDevice.EnumDisplaySettings
                (MyInfoEnumProc.DeviceName, DisplayDevice.ENUM_CURRENT_SETTINGS, ref dm))
            {
                return false;
            }

            this.Mode = dm;
            this.Name = MyInfoEnumProc.DeviceName;
            return true;
        }
    }
}
