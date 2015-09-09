using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWizard {
    static class list_extensions {
        // http://stackoverflow.com/questions/1766328/can-linq-use-binary-search-when-the-collection-is-ordered
        //
        // does not throw, returns null if not found
        public static T BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
                where TKey : IComparable<TKey> where T : class
        {
            if (list.Count == 0)
                return null;

            int min = 0;
            int max = list.Count;
            while (min < max)
            {
                int mid = min + ((max - min) / 2);
                T midItem = list[mid];
                TKey midKey = keySelector(midItem);
                int comp = midKey.CompareTo(key);
                if (comp < 0)
                {
                    min = mid + 1;
                }
                else if (comp > 0)
                {
                    max = mid - 1;
                }
                else
                {
                    return midItem;
                }
            }
            if (min == max && min < list.Count && keySelector(list[min]).CompareTo(key) == 0)
                return list[min];
            return null;
        }
    }
}
