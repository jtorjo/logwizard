using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common {
    // caches the last X entries
    //
    // very simple policy - when cache becomes full, it simply empties it
    public class simple_cache_dictionary<K,T> {
        private int max_count_;

        private Dictionary<K,T> values_ = new Dictionary<K, T>();

        public simple_cache_dictionary(int max_count = 500) {
            max_count_ = max_count;
        }

        public T get(K key, ref bool found) {
            lock (this) {
                T value;
                found = values_.TryGetValue(key, out value);
                if (found)
                    return value;
                else
                    return default(T);
            }
        }
        public T get(K key) {
            bool found = false;
            return get(key, ref found);
        }

        public void set(K key, T value) {
            lock (this) {
                if ( values_.Count >= max_count_)
                    // if value already has this key ,it will replace it, thus, cache won't exceed its limit
                    if (!values_.ContainsKey(key))
                        values_.Clear();

                if (values_.ContainsKey(key))
                    values_[key] = value;
                else 
                    values_.Add(key, value);
            }
        }

    }
}
