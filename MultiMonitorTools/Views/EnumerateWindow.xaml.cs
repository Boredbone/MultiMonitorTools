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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MultiMonitorTools.ViewModels;

namespace MultiMonitorTools.Views
{
    /// <summary>
    /// EnumerateWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EnumerateWindow : Window
    {
        EnumerateWindowViewModel viewModel; 

        public EnumerateWindow()
        {
            InitializeComponent();
            this.viewModel = this.DataContext as EnumerateWindowViewModel;
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.RelocateWindow((sender as FrameworkElement)?.DataContext as WindowInformation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.RelocateWindow(this.listBox1.SelectedItem as WindowInformation);
        }

        private void RelocateWindow(WindowInformation item)
        {
            var placement = new WindowInformation(new WindowInteropHelper(this).Handle).GetPlacement();
            item?.Relocate(placement.Left, placement.Top);

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
