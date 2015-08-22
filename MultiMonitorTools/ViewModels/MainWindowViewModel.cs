using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MultiMonitorTools.ViewModels
{
    class MainWindowViewModel:ViewModelBase
    {


        public ReactiveCommand<object> RotateCommand { get; }

        private Subject<int> DisplayRotatingSubject { get; set; }
        public IObservable<int> DisplayRotating => this.DisplayRotatingSubject.ObserveOnUIDispatcher();


        public MainWindowViewModel()
        {
            this.DisplayRotatingSubject = new Subject<int>().AddTo(this.unsubscribers);

            this.RotateCommand = new ReactiveCommand<object>().AddTo(this.unsubscribers);
            this.RotateCommand.Subscribe(async x =>
            {
                var text = x as string;
                if (text == null)
                {
                    return;
                }
                int number;
                if(int.TryParse(text,out number))
                {
                    //パラメータに従って画面を回転し壁紙を変更
                    await AppData.Current.RotateAndChangeWallPaperAsync(number, Observer.Create<int>(y => this.DisplayRotatingSubject.OnNext(y)));
                }
                
                
            }).AddTo(this.unsubscribers);
        }
    }
}
