using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiMonitorTools.Models;
using MultiMonitorTools.Settings;

namespace MultiMonitorTools
{
    /// <summary>
    /// 
    /// </summary>
    public class AppData
    {
        public static AppData Current { get; } = new AppData();

        private UserSettings savedSettings;

        public MonitorSettings MonitorSettings
        {
            get
            {
                return this.savedSettings.MonitorSettings;
            }
            set
            {
                this.savedSettings.MonitorSettings = value;
            }
        }

        private WallpaperChanger DisplayManager { get; set; }

        public int DeviceCount => this.DisplayManager.MonitorCount;


        private AppData()
        {
            this.savedSettings = new UserSettings();
            this.DisplayManager = new WallpaperChanger();

            this.Load();
        }

        public void Load()
        {
            this.savedSettings.Load(this.DisplayManager.MonitorCount);
        }

        public void Save()
        {
            this.savedSettings.Save();
        }
        

        public async Task RotateAndChangeWallPaperAsync(int increment, IObserver<int> progress)
        {
            await this.DisplayManager.RotateAndChangeWallPaperAsync(this.MonitorSettings, increment, progress);
        }

        public void RefreshWallpaper()
        {
            this.DisplayManager.RefreshWallpaper(this.MonitorSettings);
        }
    }
}
