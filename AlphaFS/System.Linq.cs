using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    internal static class LinqExtensions
    {
        public delegate void Action();
        public delegate void Action<T>(T t);
        public delegate void Action<T, U>(T t, U u);
        public delegate void Action<T, U, V>(T t, U u, V v);
        public delegate TResult Func<TResult>();
        public delegate TResult Func<T, TResult>(T t);
        public delegate TResult Func<T, U, TResult>(T t, U u);
        public delegate TResult Func<T, U, V, TResult>(T t, U u, V v);

        public static bool Any<T>(this IEnumerable<T> items)
        {
            IEnumerator<T> enumerator = items.GetEnumerator();

            return enumerator.MoveNext();
        }

        public static bool Any<T>(this IEnumerable<T> items, Func<T, bool> selector)
        {
            IEnumerator<T> enumerator = items.Where(selector).GetEnumerator();

            return enumerator.MoveNext();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> items) where T : class
        {
            IEnumerator<T> enumerator = items.GetEnumerator();

            if (enumerator.MoveNext())
                return enumerator.Current;

            return null;
        }

        public static IEnumerable<T> Select<T, K>(this IEnumerable<K> items, Func<K, T> selector) where T : class
        {
            foreach (K item in items)
                yield return selector(item);
        }

        public static IEnumerable<T> Cast<T>(this IEnumerable items)
        {
            foreach (object item in items)
                yield return (T)item;
        }

        public static int Count(this IEnumerable items)
        {
            int count = 0;
            foreach (object item in items)
                count++;

            return count;
        }

        public static long Sum<T>(this IEnumerable<T> items, Func<T, long> selector)
        {
            long sum = 0;
            foreach (T item in items)
                sum += selector(item);

            return sum;
        }

        public static int Sum<T>(this IEnumerable<T> items, Func<T, int> selector)
        {
            long sum = 0;
            foreach (T item in items)
                sum += selector(item);

            return (int)sum;
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> items, Func<T, bool> selector)
        {
            foreach (T item in items)
                if (selector(item))
                    yield return item;
        }

        public static IEnumerable<char> Where(this string str, Func<char, int, bool> selector)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (selector(str[i], i))
                    yield return str[i];
            }
        }

        public static List<T> ToList<T>(this IEnumerable<T> items)
        {
            List<T> result = new List<T>();

            foreach (T item in items)
                result.Add(item);

            return result;
        }

        public static T[] ToArray<T>(this IEnumerable<T> items)
        {
            List<T> itemsList = items.ToList();

            T[] res = new T[itemsList.Count];
            for (int i = 0; i < itemsList.Count; i++)
                res[i] = itemsList[i];

            return res;
        }

        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> items, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();

            foreach (T item in items)
            {
                TKey key = keySelector(item);
                TValue value = valueSelector(item);

                res.Add(key, value);
            }

            return res;
        }
    }
}

// you need this once (only), and it must be in this namespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
         | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}
