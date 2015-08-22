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
using System.Windows.Shapes;
using MultiMonitorTools.ViewModels;

namespace MultiMonitorTools.Views
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        private SettingWindowViewModel viewModel;

        public SettingWindow()
        {
            InitializeComponent();
            this.viewModel = this.DataContext as SettingWindowViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.Commit();
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DataContext = null;
            this.viewModel.Dispose();
        }
    }
}
