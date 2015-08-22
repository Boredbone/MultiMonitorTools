using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Boredbone.Utility.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// コレクション内のTaskが全て完了するまで待機
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// コレクション内のTaskが全て完了するまで待機
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return Task.WhenAll(tasks);
        }
        
        /// <summary>
        /// Taskの完了を待機せず、例外発生時に投げる
        /// </summary>
        /// <param name="task"></param>
        public static void FireAndForget(this Task task)
        {
            task.ContinueWith(x =>
            {
                throw x.Exception.InnerException;
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Taskの完了を待機せず、例外発生時に指定された処理を行ってから投げる
        /// </summary>
        /// <param name="task"></param>
        /// <param name="onFaulted"></param>
        public static void FireAndForget(this Task task, Action<AggregateException> onFaulted)
        {
            task.ContinueWith(x =>
            {
                onFaulted?.Invoke(x.Exception);
                throw x.Exception.InnerException;
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

    }

}
