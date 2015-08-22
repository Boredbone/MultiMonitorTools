using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MultiMonitorTools.ViewModels
{
    class EnumerateWindowViewModel:ViewModelBase
    {

        private ObservableCollection<WindowInformation> _fieldWindows;
        /// <summary>
        /// 検出されたすべてのウインドウの一覧
        /// </summary>
        public ObservableCollection<WindowInformation> Windows
        {
            get { return _fieldWindows; }
            set
            {
                if (_fieldWindows != value)
                {
                    _fieldWindows = value;
                    RaisePropertyChanged(nameof(Windows));
                }
            }
        }

        public ReactiveCommand RefreshCommand { get; }

        public WindowInformation SelectedWindow { get; set; }


        public EnumerateWindowViewModel()
        {
            //全ウインドウを列挙
            this.Windows = new ObservableCollection<WindowInformation>(WindowRelocate.EnumerateWindows());

            this.RefreshCommand = new ReactiveCommand().AddTo(this.unsubscribers);
            this.RefreshCommand.Subscribe(x =>
            {
                this.Windows = new ObservableCollection<WindowInformation>(WindowRelocate.EnumerateWindows());
            }).AddTo(this.unsubscribers);
        }

        
    }
}
