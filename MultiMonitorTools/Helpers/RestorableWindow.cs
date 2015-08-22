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
    public abstract class RestorableWindow : Window
    {
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
