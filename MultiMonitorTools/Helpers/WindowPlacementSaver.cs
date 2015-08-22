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
        

        public void Save()
        {
            //await Task.Delay(100);
            this.PlacementXml.SaveXml(this.WindowPlacements);
        }

        public void Load()
        {
            this.WindowPlacements = this.PlacementXml.LoadXml().Value;
        }


        public void Restore(Window window, string key)
        {
            Rectangle placement;
            if (this.WindowPlacements.TryGetValue(key, out placement))
            {
                new WindowInformation(new WindowInteropHelper(window).Handle).Relocate(placement);
            }
        }

        public void RestorePosition(Window window, string key)
        {
            Rectangle placement;
            if (this.WindowPlacements.TryGetValue(key, out placement))
            {
                new WindowInformation(new WindowInteropHelper(window).Handle).Relocate(placement.Left, placement.Top);
            }
        }

        public void Store(Window window, string key)
        {
            var placement = new WindowInformation(new WindowInteropHelper(window).Handle).GetPlacement();
            this.WindowPlacements[key] = placement;
        }
    }
}
