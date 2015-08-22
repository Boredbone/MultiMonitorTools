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
    /// アプリケーションの共通データ
    /// </summary>
    public class AppData
    {
        public static AppData Current { get; } = new AppData();

        private UserSettings savedSettings;

        /// <summary>
        /// ユーザー設定
        /// </summary>
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

        /// <summary>
        /// 設定ファイルの読み込み
        /// </summary>
        public void Load()
        {
            this.savedSettings.Load(this.DisplayManager.MonitorCount);
        }

        /// <summary>
        /// 設定ファイルの保存
        /// </summary>
        public void Save()
        {
            this.savedSettings.Save();
        }
        
        /// <summary>
        /// 設定された画面を回転させた後に全画面の壁紙を変更
        /// </summary>
        /// <param name="increment"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task RotateAndChangeWallPaperAsync(int increment, IObserver<int> progress)
        {
            await this.DisplayManager.RotateAndChangeWallPaperAsync(this.MonitorSettings, increment, progress);
        }

        /// <summary>
        /// 全画面の壁紙を変更
        /// </summary>
        public void RefreshWallpaper()
        {
            this.DisplayManager.RefreshWallpaper(this.MonitorSettings);
        }
    }
}
