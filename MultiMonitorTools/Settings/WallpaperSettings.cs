using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorTools.Settings
{
    /// <summary>
    /// ディスプレイの方向と対応した背景画像の辞書
    /// </summary>
    [DataContract]
    public class WallpaperSettings
    {
        [DataMember]
        public Dictionary<DisplayOrientations, WallpaperInformation> OrientationSettings { get; private set; }

        public WallpaperSettings(string wallpaperPath)
        {
            this.OrientationSettings = new Dictionary<DisplayOrientations, WallpaperInformation>()
            {
                [DisplayOrientations.Landscape] = new WallpaperInformation(wallpaperPath),
                [DisplayOrientations.LandscapeFlipped] = new WallpaperInformation(wallpaperPath),
                [DisplayOrientations.Portrait] = new WallpaperInformation(wallpaperPath),
                [DisplayOrientations.PortraitFlipped] = new WallpaperInformation(wallpaperPath),
            };
        }

        public WallpaperSettings() : this("")
        {

        }

        public WallpaperInformation this[DisplayOrientations orientation]
        {
            set { this.OrientationSettings[orientation] = value; }
            get { return this.OrientationSettings[orientation]; }
        }


        public WallpaperSettings Clone()
        {
            var clone = new WallpaperSettings();
           
            foreach(var item in this.OrientationSettings)
            {
                clone.OrientationSettings[item.Key] = item.Value.Clone();
            }

            return clone;
        }
    }
}
