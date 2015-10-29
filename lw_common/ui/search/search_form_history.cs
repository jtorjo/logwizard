using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace lw_common.ui {
    internal class search_form_history {
        private const int MAX_SEARCH_COUNT = 50;

        private List<search_for> history_ = new List<search_for>();
        private int next_unique_id_ = 0;

        private static search_form_history inst_ = new search_form_history();
        public static search_form_history inst {
            get { return inst_; }
        }

        private search_form_history() {
            load();
        }

        private void load() {
            var sett = app.inst.sett;
            int count = int.Parse(sett.get("search.count", "0"));
            for (int idx = 0; idx < count; ++idx) {
                var seach = search_for.load("search." + idx);
                seach.unique_id = ++next_unique_id_;
                history_.Add(seach);
            }
        }

        private void save() {
            var sett = app.inst.sett;
            sett.set("search.count", "" + history_.Count);
            for ( int idx = 0; idx < history_.Count; ++idx)
                history_[idx].save("search." + idx);
            sett.save();
        }

        public search_for default_search {
            get { return last_search; }
        }

        public search_for last_search {
            get {
                if (history_.Count > 0)
                    return history_.Last();

                // history is empty...
                // ... basically this will load it with the defaults
                var last = search_for.load("search.last");
                last.unique_id = ++next_unique_id_;
                return last;
            }
        }

        // saves this as being the last search
        public void save_last_search(search_for last) {
            Debug.Assert(last.unique_id > 0);

            // moves this search to the end! (which visually means - to the top)
            var exists = history_.FirstOrDefault(x => x.unique_id == last.unique_id);
            if (exists != null)
                history_.Remove(exists);
            history_.Add(last);
            while ( history_.Count > MAX_SEARCH_COUNT)
                history_.RemoveAt(0);

            // if it's from history, bring to top!
            save();
        }

        public List<search_for> all_searches_cur_view_first(string view_name) {
            List<search_for> searches = history_.ToList();

            if ( searches.Count < 1)
                searches.Add(default_search);
            return searches;
        } 

    }
}
