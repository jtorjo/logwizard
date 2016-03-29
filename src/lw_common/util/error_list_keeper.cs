using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common {
    // 1.8.23+ - allow keeping the error level as well (granularity)
    public class error_list_keeper {

        public enum level_type {
            warning, error, fatal
        }

        private const int EXPIRE_MS = 15000;

        private int max_errors = 30;

        private List< Tuple<string,level_type, DateTime>> errors_ = new List<Tuple<string, level_type,DateTime>>();
        private List< Tuple<string,level_type>> empty_ = new List<Tuple<string, level_type>>();
        public void add(string err, level_type level = level_type.error) {
            lock (this) {
                errors_.Add(new Tuple<string,level_type,DateTime>(err,level, DateTime.Now.AddMilliseconds(EXPIRE_MS)) );
                update_list();
            }
        }

        private void update_list() {
            while ( errors_.Count > max_errors)
                errors_.RemoveAt(0);
            DateTime now = DateTime.Now;
            // remove those that expired
            while ( errors_.Count > 0 && errors_[0].Item3 < now)
                errors_.RemoveAt(0);
        }

        public void clear() {
            lock (this)
                errors_.Clear();
        }

        public List< Tuple<string,level_type>> errors {
            get {
                lock (this) {
                    update_list();
                    return errors_.Count > 0 ? errors_.Select(x => new Tuple<string, level_type>(x.Item1, x.Item2)).ToList() : empty_;
                }
            }
        } 
    }
}
