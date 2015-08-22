using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorTools.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected CompositeDisposable unsubscribers = new CompositeDisposable();
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Dispose()
        {
            this.unsubscribers.Dispose();
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var d = PropertyChanged;
            if (d != null)
                d(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
