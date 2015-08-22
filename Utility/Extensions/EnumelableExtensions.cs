using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boredbone.Utility.Extensions
{
    public static class EnumelableExtensions
    {
        /// <summary>
        /// 二つのDictionaryを結合
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>
            (this Dictionary<TKey, TValue> first, IEnumerable<KeyValuePair<TKey, TValue>> second)
        {
            if (first == null && second == null)
            {
                return null;
            }
            else if (first == null)
            {
                return second.ToDictionary(x => x.Key, x => x.Value);
            }
            else if (second == null)
            {
                return first;
            }

            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

            foreach (var x in first.Concat(second).Where(x => x.Value != null))
            {
                dictionary[x.Key] = x.Value;//重複していたら上書き
            }

            return dictionary;

            //return first.Concat(second).Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        }

        public static ConcurrentDictionary<TKey, TValue> Merge<TKey, TValue>
            (this ConcurrentDictionary<TKey, TValue> first, IEnumerable<KeyValuePair<TKey, TValue>> second)
        {
            if (first == null && second == null)
            {
                return null;
            }
            else if (first == null)
            {
                return new ConcurrentDictionary<TKey, TValue>(second);//.ToDictionary(x => x.Key, x => x.Value));
            }
            else if (second == null)
            {
                return first;
            }

            ConcurrentDictionary<TKey, TValue> dictionary = new ConcurrentDictionary<TKey, TValue>();

            foreach (var x in first.Concat(second).Where(x => x.Value != null))
            {
                dictionary[x.Key] = x.Value;//重複していたら上書き
            }

            return dictionary;

            //return new ConcurrentDictionary<TKey, TValue>(first.Concat(second).Where(x => x.Value != null));
            //.ToDictionary(x => x.Key, x => x.Value);
        }

        public static T? FirstOrNull<T>(this IEnumerable<T> source) where T : struct
        {
            foreach (var item in source)
            {
                return item;
            }
            return null;
        }

        public static T? FirstOrNull<T>
            (this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return null;
        }

        /*
        public static int FirstIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int count = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return count;
                }
                count++;
            }
            return -1;
        }*/

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
        public static void ForEachIndexed<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in source)
            {
                action(item, index);
                index++;
            }
        }

        //public static IEnumerable<Tout> Convert<Tin, Tout>
        //    (this IEnumerable<Tin> source, Func<Tin, Tout> converter)
        //{
        //    foreach (var item in source)
        //    {
        //        yield return converter(item);
        //    }
        //}

        /// <summary>
        /// 2つのシーケンスを連結
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, params T[] second)
        {
            return Enumerable.Concat(first, second);
        }

        /// <summary>
        /// シーケンスを指定の個数ごとに分割
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> source, int size)
        {
            var result = new List<T>(size);
            foreach (var item in source)
            {
                result.Add(item);
                if (result.Count == size)
                {
                    yield return result;
                    result = new List<T>(size);
                }
            }
            if (result.Count != 0)
            {
                yield return result.ToArray();
            }
        }

        //public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> source, int size)
        //{
        //    if (size <= 0)
        //    {
        //        throw new ArgumentException("Chunk size must be greater than 0.", nameof(size));
        //    }

        //    while (source.Any())
        //    {
        //        yield return source.Take(size);
        //        source = source.Skip(size);
        //    }
        //}

        /// <summary>
        /// 要素とインデックスを格納するクラス
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class IndexedItem<T>
        {
            public T Value { get; private set; }
            public int Index { get; private set; }

            public IndexedItem(T value, int index)
            {
                this.Value = value;
                this.Index = index;
            }
        }

        /// <summary>
        /// インデックスつき要素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<IndexedItem<T>> Indexed<T>(this IEnumerable<T> source)
        {
            return source.Select((x, i) => new IndexedItem<T>(x, i));
        }

        /// <summary>
        /// シーケンス全体を指定回数繰り返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="souce"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> souce, int count)
        {
            return Enumerable.Range(0, count).SelectMany(_ => souce);
        }

        /// <summary>
        /// シーケンスの各要素ごとに指定回数ずつ繰り返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="souce"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> Stretch<T>(this IEnumerable<T> souce, int count)
        {
            return souce.SelectMany(value => Enumerable.Range(0, count).Select(_ => value));
        }


        /// <summary>
        /// 指定のシーケンスと同じ内容になるようAddとRemoveを行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static IList<T> Imitate<T>
            (this IList<T> source, IEnumerable<T> reference) where T : class
        {
            return source.Imitate(reference, (x, y) => object.ReferenceEquals(x, y), x => x);
        }

        /// <summary>
        /// 指定のシーケンスと同じ内容になるようAddとRemoveを行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="reference"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IList<T> Imitate<T>
            (this IList<T> source, IEnumerable<T> reference, Func<T, T, bool> match)
        {
            return source.Imitate(reference, match, x => x);
        }

        /// <summary>
        /// 指定のシーケンスと同じ内容になるようAddとRemoveを行う
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="reference"></param>
        /// <param name="match"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static IList<T1> Imitate<T1, T2>
            (this IList<T1> source, IEnumerable<T2> reference, Func<T1, T2, bool> match, Func<T2, T1> converter)
        {
            source.Absorb(reference, match, converter);
            return source.FilterStrictlyBy(reference, match);

            //source.FilterBy(reference, match);
            //source.AbsorbStrictly(reference, match, converter);
        }

        private static void AbsorbStrictly<T1, T2>
            (this IList<T1> source, IEnumerable<T2> reference, Func<T1, T2, bool> match, Func<T2, T1> converter)
        {

            int startIndex = 0;

            //新しいアイテムを追加

            foreach (var item in reference)
            {
                var length = source.Count;

                if (startIndex >= length)
                {
                    source.Add(converter(item));
                    startIndex = source.Count;
                    continue;
                }

                if (!match(source[startIndex], item))
                {
                    source.Insert(startIndex, converter(item));
                }
                startIndex++;
            }
        }

        /// <summary>
        /// 二つのシーケンスを順番に比較し，referenceにしかない要素があれば追加
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="reference"></param>
        /// <param name="match"></param>
        /// <param name="converter"></param>
        public static void Absorb<T1, T2>
            (this IList<T1> source, IEnumerable<T2> reference, Func<T1, T2, bool> match, Func<T2, T1> converter)
        {

            int startIndex = 0;

            //新しいアイテムを追加

            foreach (var item in reference)
            {

                var length = source.Count;
                var existance = false;

                for (int i = startIndex; i < length; i++)
                {
                    var checkItem = source[i];

                    if (match(checkItem, item))
                    {
                        existance = true;
                        startIndex = i + 1;
                        break;
                    }
                }

                if (!existance)
                {
                    if (startIndex >= length)
                    {
                        source.Add(converter(item));
                        startIndex = source.Count;
                    }
                    else
                    {
                        source.Insert(startIndex, converter(item));
                        startIndex++;
                    }
                }
            }
        }


        private static List<T1> FilterStrictlyBy<T1, T2>
            (this IList<T1> source, IEnumerable<T2> reference, Func<T1, T2, bool> match)
        {
            //消えたアイテムの削除

            //int referenceIndex = 0;
            var removedItems = new List<T1>();


            using (var e = reference.GetEnumerator())
            {
                var usable = e.MoveNext();
                var currentReference = e.Current;

                foreach (var item in source)
                {
                    if (!usable || !match(item, currentReference))
                    {
                        removedItems.Add(item);
                    }
                    else
                    {
                        usable = e.MoveNext();
                        currentReference = e.Current;
                    }
                }
            }


            //foreach (var item in source)
            //{
            //    if (referenceIndex >= reference.Count
            //        || !match(item, reference[referenceIndex]))
            //    {
            //        removedItems.Add(item);
            //    }
            //    else
            //    {
            //        referenceIndex++;
            //    }
            //}

            foreach (var di in removedItems)
            {
                source.Remove(di);
            }
            return removedItems;
        }

        /// <summary>
        /// 二つのシーケンスを順番に比較し，referenceに存在しない要素があれば削除
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="reference"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<T1> FilterBy<T1, T2>
            (this IList<T1> source, IEnumerable<T2> reference, Func<T1, T2, bool> match)
        {

            //消えたアイテムの削除

            int referenceIndex = 0;

            var removedItems = new List<T1>();
            //var length = reference.Count;

            foreach (var item in source)
            {
                var existingIndex = reference.FindIndex(x => match(item, x), referenceIndex);

                if (existingIndex < 0)
                {
                    removedItems.Add(item);
                }
                else
                {
                    referenceIndex = existingIndex + 1;
                }


                //bool existance = false;
                //
                //for (int i = referenceIndex; i < length; i++)
                //{
                //    var checkItem = reference[i];
                //    if (match(item, checkItem))
                //    {
                //        referenceIndex = i + 1;
                //        existance = true;
                //        break;
                //    }
                //}
                //
                //if (!existance)
                //{
                //    removedItems.Add(item);
                //}
            }


            foreach (var di in removedItems)
            {
                source.Remove(di);
            }
            return removedItems;
        }

        /// <summary>
        /// 条件に一致する最初の要素のインデックスを探す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> match)
        {
            return source.FindIndex(match, 0);
        }

        /// <summary>
        /// 指定されたインデックス以降で条件に一致する最初の要素のインデックスを探す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> match, int startIndex)
        {
            int index = 0;
            foreach (var x in source)
            {
                if (index >= startIndex && match(x))
                {
                    return index;
                }
                index++;
            }
            return -1;
            //source.Where((x,c)=>match(x)&&c>=startIndex)
        }

        /// <summary>
        /// シーケンスの数値を積分
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double> Integral(this IEnumerable<double> source)
        {
            double sum = 0.0;

            foreach (var item in source)
            {
                sum += item;
                yield return sum;
            }
        }

        /// <summary>
        /// シーケンスの数値を積分
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<int> Integral(this IEnumerable<int> source)
        {
            int sum = 0;

            foreach (var item in source)
            {
                sum += item;
                yield return sum;
            }
        }

        /// <summary>
        /// シーケンスの数値を積分
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<long> Integral(this IEnumerable<long> source)
        {
            long sum = 0;

            foreach (var item in source)
            {
                sum += item;
                yield return sum;
            }
        }

        /// <summary>
        /// 二つのシーケンスの順番と長さが同じかどうか調べる
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool SequenceEqual<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> match)
        {
            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && match(e1.Current, e2.Current))) return false;
                }
                if (e2.MoveNext()) return false;
            }
            return true;
        }

        /*
        public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
            where T : IEquatable<T>
        {
            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && e1.Current.Equals(e2.Current))) return false;
                }
                if (e2.MoveNext()) return false;
            }
            return true;
        }*/

        /// <summary>
        /// インデックスが配列の範囲内かどうか調べる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ContainsIndex<T>(this IList<T> list, int index)
        {
            if (list == null)
            {
                return false;
            }
            return (index >= 0 && index < list.Count);
        }

        /// <summary>
        /// インデックスが配列の範囲内かどうか調べる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ContainsIndex<T>(this T[] array, int index)
        {
            if (array == null)
            {
                return false;
            }
            return (index >= 0 && index < array.Length);
        }


        public static T FromIndexOrDefault<T>(this IList<T> list, int index)
        {
            if (list != null && list.ContainsIndex(index))
            {
                return list[index];
            }
            return default(T);
        }

        public static T FromIndexOrDefault<T>(this T[] array, int index)
        {
            if (array != null && array.ContainsIndex(index))
            {
                return array[index];
            }
            return default(T);
        }



        /// <summary>
        /// 一つ前の値と合わせて処理
        /// </summary>
        /// <typeparam name="Tin"></typeparam>
        /// <typeparam name="Tout"></typeparam>
        /// <param name="source"></param>
        /// <param name="func">前の値，現在の値，結果</param>
        /// <returns></returns>
        public static IEnumerable<Tout> TakeOver<Tin,Tout>
            (this IEnumerable<Tin> source, Func<Tin, Tin, Tout> func)
        {
            return source.TakeOver(func, source.First());
        }

        /// <summary>
        /// 一つ前の値と合わせて処理
        /// </summary>
        /// <typeparam name="Tin"></typeparam>
        /// <typeparam name="Tout"></typeparam>
        /// <param name="source"></param>
        /// <param name="func">&lt;前の値，現在の値，結果&gt;</param>
        /// <param name="initialValue">初期値</param>
        /// <returns></returns>
        public static IEnumerable<Tout> TakeOver<Tin, Tout>
            (this IEnumerable<Tin> source, Func<Tin, Tin, Tout> func, Tin initialValue)
        {
            //yield return source.First() - initialValue;
            var prev = initialValue;
            foreach (var value in source)
            {
                yield return func(prev, value);
                prev = value;
            }
        }


        /// <summary>
        /// IEnumerable&lt;object&gt;に変換
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<object> AsEnumerable(this Array array)
        {
            foreach (var item in array)
            {
                yield return item;
            }
        }

        /// <summary>
        /// IEnumerable&lt;T&gt;に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this Array array)
        {
            foreach (var item in array)
            {
                yield return (T)item;
            }
        }
    }
}
