using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common {
    public class error_list_keeper {
        public int max_errors = 10;
        private List<string> errors_ = new List<string>();

        public void add(string err) {
            lock (this) {
                errors_.Insert(0, err);
                while ( errors_.Count > max_errors)
                    errors_.RemoveAt(errors_.Count - 1);
            }
        }

        public void clear() {
            lock (this)
                errors_.Clear();
        }

        // returns null if there are no errors
        // most error is shown first
        public List<string> errors {
            get { lock(this) return errors_.Count > 0 ? errors_.ToList() : null; }
        } 
    }
}
