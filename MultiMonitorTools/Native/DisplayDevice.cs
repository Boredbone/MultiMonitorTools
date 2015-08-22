using System;
using System.Runtime.InteropServices;

namespace MultiMonitorTools.Native
{
    public class DisplayDevice
    {

        [DllImport("User32.dll")]
        public static extern bool EnumDisplayDevices(
            IntPtr lpDevice, int iDevNum,
            ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

        [DllImport("User32.dll")]
        public static extern bool EnumDisplaySettings(
            string devName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(
            ref DEVMODE devMode, int flags);

        /// <summary>
        /// DISP_CHANGE_SUCCESSFUL = 0: Indicates that the function succeeded.
        /// DISP_CHANGE_BADMODE = -2: The graphics mode is not supported.
        /// DISP_CHANGE_FAILED = -1: The display driver failed the specified graphics mode.
        /// DISP_CHANGE_RESTART = 1: The computer must be restarted for the graphics mode to work.
        /// </summary>
        /// <param name="lpszDeviceName"></param>
        /// <param name="lpDevMode"></param>
        /// <param name="hwnd"></param>
        /// <param name="dwflags"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]//"coredll.dll")]
        public static extern int ChangeDisplaySettingsEx(
            string lpszDeviceName,
            ref DEVMODE lpDevMode,
            IntPtr hwnd,
            int dwflags,
            IntPtr lParam);
        
        // constants
        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;

        public const int DM_DISPLAYORIENTATION = 8388608;
        public const int DM_DISPLAYQUERYORIENTATION = 16777216;

        public const int DMORIENT_PORTRAIT = 1;
        public const int DMORIENT_LANDSCAPE = 2;

        //public const int CDS_VIDEOPARAMETERS = &H20;
        //public const int CDS_RESET = &H40000000;

        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
        public const int DISP_CHANGE_BADMODE = -2;
        public const int DISP_CHANGE_NOTUPDATED = -3;
        public const int DISP_CHANGE_BADFLAGS = -4;
        public const int DISP_CHANGE_BADPARAM = -5;

        //public const int ENUM_CURRENT_SETTINGS = -1;
        public const int DM_BITSPERPEL = (int)0x00040000L;
        public const int DM_PELSWIDTH = (int)0x00080000L;
        public const int DM_PELSHEIGHT = (int)0x00100000L;
        public const int DM_POSITION = (int)0x00000020L;
        public const int DM_DISPLAYFREQUENCY = (int)0x00400000L;
        public const int CDS_NORESET = (int)0x10000000;
        public const int CDS_RESET = (int)0x40000000;
        public const int CDS_UPDATEREGISTRY = (int)0x00000001;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    };


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        //[MarshalAs(UnmanagedType.U4)]
        //public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        public int StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;

        public DISPLAY_DEVICE(int flags)
        {
            cb = 0;
            StateFlags = flags;
            DeviceName = new string((char)32, 32);
            DeviceString = new string((char)32, 128);
            DeviceID = new string((char)32, 128);
            DeviceKey = new string((char)32, 128);
            cb = Marshal.SizeOf(this);
        }
    }
}
