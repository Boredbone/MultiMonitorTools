using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiMonitorTools.Native;

namespace MultiMonitorTools.Settings
{
    /// <summary>
    /// ディスプレイの表示方向
    /// </summary>
    public enum DisplayOrientations
    {
        Portrait = DisplayDevice.DMDO_270,
        PortraitFlipped = DisplayDevice.DMDO_90,
        Landscape = DisplayDevice.DMDO_DEFAULT,
        LandscapeFlipped = DisplayDevice.DMDO_180
    }
}
