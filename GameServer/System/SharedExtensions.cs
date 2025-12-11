using System;
using System.Collections.Generic;

namespace DeathWish
{
    public static class SharedExtensions
	{
		public static T[] Transform<T>(this object[] strs)
		{
			T[] array = new T[strs.Length];
			for (int i = 0; i < strs.Length; i++)
			{
				array[i] = (T)Convert.ChangeType(strs[i], typeof(T));
			}
			return array;
		}
		public static void Add<T, T2>(this IDictionary<T, T2> dict, T key, T2 value)
		{
			dict[key] = value;
		}
		public static void Remove<T, T2>(this IDictionary<T, T2> dict, T key)
		{
			dict.Remove(key);
		}
		public static void Iterate<T>(this T[] collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }
    }
}