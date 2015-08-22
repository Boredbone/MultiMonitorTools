using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.ComponentModel;

namespace MultiMonitorTools.Helpers
{
    /// <summary>
    /// 終了時に位置と大きさを保存し、次回起動時に復元するWindow
    /// </summary>
    public abstract class RestorableWindow : Window
    {
        /// <summary>
        /// 各Windowに固有のキー
        /// </summary>
        public abstract string WindowId { get; }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowPlacementSaver.Instance.Restore(this, WindowId);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!e.Cancel)
            {
                WindowPlacementSaver.Instance.Store(this, WindowId);
            }
        }
    }
}
