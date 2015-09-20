using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net.Util;

namespace LogWizard {
    static class list_extensions {
        // http://stackoverflow.com/questions/1766328/can-linq-use-binary-search-when-the-collection-is-ordered
        //
        // does not throw, returns null if not found
        // returns the position of the element as well (so that we don't have to call the inefficient IndexOf)
        public static Tuple<T,int> binary_search<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
                where TKey : IComparable<TKey> where T : class
        {
            if (list.Count == 0)
                return new Tuple<T, int>(null,-1);

            var closest = binary_search_closest(list, keySelector, key);
            if (closest.Item1 != null) {
                // see if it's an exact match
                bool is_exact = keySelector(list[closest.Item2]).CompareTo(key) == 0;
                if (is_exact)
                    return closest;
            }

            return new Tuple<T, int>(null,-1);
        }

        // returns the closest item - guaranteed to return something
        public static Tuple<T,int> binary_search_closest<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
                where TKey : IComparable<TKey> where T : class
        {
            if ( list.Count < 1)
                return new Tuple<T, int>(default(T), -1);

            int min = 0;
            int max = list.Count;
            while (min < max)
            {
                int mid = min + ((max - min) / 2);
                T midItem = list[mid];
                TKey midKey = keySelector(midItem);
                int comp = midKey.CompareTo(key);
                if (comp < 0)
                    min = mid + 1;
                else if (comp > 0)
                    max = mid - 1;
                else
                    return new Tuple<T, int>(midItem, mid);
            }

            if (min >= list.Count)
                min = list.Count - 1;
            if (max >= list.Count)
                max = list.Count - 1;
            if (max < 0)
                max = 0;

            if (min == max )
                return new Tuple<T, int>(list[min], min);

            Debug.Assert(min >= 0);
            // now - see if to return start or end
            T startItem = list[min];
            TKey startKey = keySelector(startItem);
            T endItem = list[max];
            TKey endKey = keySelector(endItem);
            bool return_start = Math.Abs(startKey.CompareTo(key)) < Math.Abs(endKey.CompareTo(key));
            return new Tuple<T, int>(return_start ? startItem : endItem, return_start ? min : max);
        }

        // returns where you would insert an item in this list
        public static int binary_search_insert<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
            where TKey : IComparable<TKey> where T : class {
            if (list.Count < 1)
                return 0;
            
            var closest = binary_search_closest(list, keySelector, key);
            if ( key.CompareTo( keySelector(list[closest.Item2])) == 0)
                // the element already exists
                return closest.Item2;


            if (closest.Item2 < list.Count) {
                TKey after = keySelector(list[closest.Item2 + 1]);
                if (key.CompareTo(after) < 0)
                    return closest.Item2 + 1;
            }

            return closest.Item2;
        }

    }
}
