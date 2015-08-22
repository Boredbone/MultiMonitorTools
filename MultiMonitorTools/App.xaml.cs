using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Boredbone.Utility.Extensions;
using MultiMonitorTools.Helpers;
using MultiMonitorTools.Native;

namespace MultiMonitorTools
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        //}
        //protected override void OnActivated(EventArgs e)
        //{
            base.OnActivated(e);

            int color;
            bool blend;
            DwmApi.DwmGetColorizationColor(out color, out blend);

            int A = color >> 24 & 0xff;
            int R = color >> 16 & 0xff;
            int G = color >> 8 & 0xff;
            int B = color & 0xff;

            var winColor = Color.FromArgb(0xff,
                (byte)(R * A / 0xff + 0xff - A), 
                (byte)(G * A / 0xff + 0xff - A), 
                (byte)(B * A / 0xff + 0xff - A));

            this.Resources["WindowColorBrush"] = new SolidColorBrush(winColor);
            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            WindowPlacementSaver.Instance.Save();
        }
    }
}
