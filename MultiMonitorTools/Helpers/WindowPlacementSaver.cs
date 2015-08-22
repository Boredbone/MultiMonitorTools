using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Boredbone.Utility;

namespace MultiMonitorTools.Helpers
{
    /// <summary>
    /// Windowの位置とサイズをファイルに保存
    /// </summary>
    public class WindowPlacementSaver
    {

        public static WindowPlacementSaver Instance { get; } = new WindowPlacementSaver();

        private string placementFileName = "placement.config";
        
        private XmlSettingManager<Dictionary<string, Rectangle>> PlacementXml { get; }
        
        private Dictionary<string, Rectangle> WindowPlacements { get; set; }
        


        public WindowPlacementSaver()
        {
            this.WindowPlacements = new Dictionary<string, Rectangle>();
            
            this.PlacementXml = new XmlSettingManager<Dictionary<string, Rectangle>>(this.placementFileName);

            this.Load();
        }
        
        /// <summary>
        /// 設定ファイルを保存
        /// </summary>
        public void Save()
        {
            this.PlacementXml.SaveXml(this.WindowPlacements);
        }

        /// <summary>
        /// 設定ファイルを読み込み
        /// </summary>
        public void Load()
        {
            this.WindowPlacements = this.PlacementXml.LoadXml().Value;
        }

        /// <summary>
        /// 位置とサイズを復元
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void Restore(Window window, string key)
        {
            Rectangle placement;
            if (this.WindowPlacements.TryGetValue(key, out placement))
            {
                new WindowInformation(new WindowInteropHelper(window).Handle).Relocate(placement);
            }
        }

        /// <summary>
        /// サイズを変更せず、位置のみを復元
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void RestorePosition(Window window, string key)
        {
            Rectangle placement;
            if (this.WindowPlacements.TryGetValue(key, out placement))
            {
                new WindowInformation(new WindowInteropHelper(window).Handle).Relocate(placement.Left, placement.Top);
            }
        }

        /// <summary>
        /// 位置とサイズを保存
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void Store(Window window, string key)
        {
            var placement = new WindowInformation(new WindowInteropHelper(window).Handle).GetPlacement();
            this.WindowPlacements[key] = placement;
        }
    }
}
