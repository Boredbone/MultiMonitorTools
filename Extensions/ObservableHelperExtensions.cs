using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Boredbone.Utility.Extensions
{
    public static class ObservableHelperExtensions
    {

       

        public static T AddTo<T,TKey>
            (this T disposable, IDictionary<TKey, IDisposable> dictionary, TKey key) where T : IDisposable
        {
            IDisposable result;
            if (dictionary.TryGetValue(key, out result))
            {
                result.Dispose();
                dictionary.Remove(key);
            }
            //if (dictionary.ContainsKey(key))
            //{
            //    dictionary[key].Dispose();
            //    dictionary.Remove(key);
            //}
            dictionary.Add(key, disposable);

            return disposable;
        }

        public static IObservable<T> DownSample<T>(this IObservable<T> source, TimeSpan interval)
        {
            return Observable.Create<T>(o =>
            {
                var subscriptions = new CompositeDisposable();
                var acceepted = true;

                //var pub = source.Where(x => acceepted).Publish(x => x);
                var pub = source.Where(x => acceepted);

                pub.Subscribe(o).AddTo(subscriptions);

                pub.Do(x => acceepted = false)
                .Delay(interval).Subscribe(x => acceepted = true)
                .AddTo(subscriptions);

                return subscriptions;
            });

            //bool acceepted = true;
            //var sub = new Subject<T>();
            //
            //source.Where(x => acceepted).Subscribe(sub);
            //sub.Do(x => acceepted = false).Delay(interval).Subscribe(x => acceepted = true);
            //
            //return sub.AsObservable();
        }
        
    }
}
