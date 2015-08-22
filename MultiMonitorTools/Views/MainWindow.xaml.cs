using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Boredbone.Utility.Extensions;
using MultiMonitorTools.Helpers;
using MultiMonitorTools.ViewModels;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;

namespace MultiMonitorTools.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow:RestorableWindow
    {
        private readonly string[] windowIds = new[]
        {
            "91b2d79d-5add-4e57-b213-7c20e8b1a202" ,
            "2341c11e-5d8f-47fc-aad0-69fd8fcdc710"
        };

        private int _windowIndex = 0;
        private int WindowIndex
        {
            get { return this._windowIndex; }
            set
            {
                this._windowIndex = value;
                if (value == 0)
                {
                    this.rotateButtons.Visibility = Visibility.Visible;
                    this.ShowInTaskbar = true;
                }
                else
                {
                    this.rotateButtons.Visibility = Visibility.Collapsed;
                    this.ShowInTaskbar = false;
                }
            }
        }

        public override string WindowId => windowIds[this.WindowIndex];


        private MainWindowViewModel viewModel;

        private CompositeDisposable unsubscribers;



        public MainWindow()
        {
            InitializeComponent();

            this.unsubscribers = new CompositeDisposable();

            this.viewModel = this.DataContext as MainWindowViewModel;
            this.viewModel.DisplayRotating.Subscribe(x =>
            {
                Minimize();

            }).AddTo(this.unsubscribers);
        }

        /*
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowPlacementSaver.Instance.RestorePosition(this, windowIds[this.WindowIndex]);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!e.Cancel)
            {
                WindowPlacementSaver.Instance.Store(this, windowIds[this.WindowIndex]);
            }
        }
        */



        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Minimize();
        }

        private void Minimize()
        {
            var mainWindow = this.Owner as MainWindow;
            if (mainWindow != null)
            {
                SystemCommands.MinimizeWindow(mainWindow);
            }
            else
            {
                SystemCommands.MinimizeWindow(this);
            }
        }



        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WindowPlacementSaver.Instance.Store(this, WindowId);

            var mainWindow = this.Owner as MainWindow;
            if (mainWindow != null)
            {
                SystemCommands.CloseWindow(mainWindow);
            }
            else
            {
                SystemCommands.CloseWindow(this);
            }
        }

        private bool? ShowDialogUniquely<T>(Window owner) where T : Window, new()
        {
            Application.Current
               .Windows
               .Cast<Window>()
               .Select(x => x as T)
               .Where(x => x != null)
               .ForEach(x => x.Close());

            var dialog = new T();
            dialog.Owner = this;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return dialog.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialogResult = ShowDialogUniquely<EnumerateWindow>(this);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dialogResult = ShowDialogUniquely<SettingWindow>(this);
        }

        private void RestorableWindow_Closing(object sender, CancelEventArgs e)
        {
            Application.Current
               .Windows
               .Cast<Window>()
               .Select(x => x as MainWindow)
               .Where(x => x != null)
               .ForEach(x => WindowPlacementSaver.Instance.Store(x, x.WindowId));

            this.DataContext = null;
            this.unsubscribers.Dispose();
            this.viewModel.Dispose();
        }

        
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.WindowIndex == 0)
            {
                new MainWindow() { Owner = this, WindowIndex = this.WindowIndex + 1 }.Show();
            }
        }
        

        //private async void Button_Click_4(object sender, RoutedEventArgs e)
        //{
        //    await AppData.Current.RotateAndChangeWallPaperAsync(0, Observer.Create<int>(y => { }));
        //    SystemCommands.MinimizeWindow(this);
        //}

    }
}
