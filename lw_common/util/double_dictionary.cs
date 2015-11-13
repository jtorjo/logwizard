using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace lw_common {
    public class double_dictionary<K,V> {
        private Dictionary<K,V> keys_ = new Dictionary<K, V>(); 
        private Dictionary<V,K> values_ = new Dictionary<V, K>();

        public double_dictionary() {
            
        }

        public double_dictionary(Dictionary<K, V> source) {
            foreach ( var kv in source)
                set( kv.Key, kv.Value);            
        }

        public void set(K key, V value) {

            if (keys_.ContainsKey(key)) {
                V old = keys_[key];
                keys_.Remove(key);
                values_.Remove(old);
            }

            Debug.Assert(!values_.ContainsKey(value));

            keys_.Add(key, value);
            values_.Add(value, key);
        }

        public bool has_key(K key) {
            return keys_.ContainsKey(key);
        }

        public bool has_value(V value) {
            return values_.ContainsKey(value);
        }

        public V key_to_value(K key) {
            Debug.Assert(has_key(key));
            return keys_[key];
        }

        public K value_to_key(V value) {
            Debug.Assert(has_value(value));
            return values_[value];
        }

    }
}
