using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format {
    
    /*  calling override_print during UI printing can get very expensive (especially when scrolling)
        thus, we're caching as much as possible
    */
    class formatted_text_cache {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private log_view parent_;
        private column_formatter_base.format_cell.location_type location_;

        private class cache_data {
            public formatted_text format;
            // ... so we know which items are old
            public int cache_index;
        }
        private Dictionary< Tuple<int,int>, cache_data > cache_ = new Dictionary<Tuple<int, int>, cache_data>();

        // when I reach this size, I clear half of it
        public int max_cache_size = 2048;

        private int next_cache_index_ = 0;

        // just for testing - normally should always be true
        public bool use_cache = !util.is_debug;

        public formatted_text_cache(log_view parent, column_formatter_base.format_cell.location_type location) {
            parent_ = parent;
            location_ = location;
        }

        private formatted_text override_print_no_cache(match_item i, string text, int col_idx) {
            var print = i.override_print(parent_, text, col_idx, location_).format_text;

            var type = log_view_cell.cell_idx_to_type(col_idx);
            if (info_type_io.can_be_multi_line(type)) 
                print = print.get_most_important_single_line();
            return print;
        }

        private bool can_cache(int col_idx) {
            if (!use_cache)
                return false;

            // 1.7.21 - for now, keep it easy - date/time can't be cached, because they user prev_text and top_idx to generate formatting
            switch ( log_view_cell.cell_idx_to_type(col_idx) ) {
                // 1.7.32 - can't cache line - we show selection/bookmark images
            case info_type.line:

            case info_type.date:
            case info_type.time:
                return false;
            }

            return true;
        }

        public formatted_text override_print(match_item i, string text, int row_idx, int col_idx) {
            if (!can_cache(col_idx))
                return override_print_no_cache(i, text, col_idx);

            if (cache_.Count >= max_cache_size)
                drop_old_items();
            if (next_cache_index_ % 500 == 0)
                dump_cache_info();

            var key = new Tuple<int,int>(row_idx, col_idx);
            cache_data in_cache;
            if (cache_.TryGetValue(key, out in_cache)) {
                in_cache.cache_index = ++next_cache_index_;
                return in_cache.format;
            }

            var print = override_print_no_cache(i, text, col_idx);
            
            if (cache_.TryGetValue(key, out in_cache)) {
                // we can sometimes get here - seems a call to top_row_idx => visible_row_indexes() => gets us here, within another overrideprint
                in_cache.cache_index = ++next_cache_index_;
                return in_cache.format;
            }

            cache_.Add(key, new cache_data { format = print, cache_index = ++next_cache_index_ });
            return print;
        }

        public void clear(string reason) {
            cache_.Clear();
            logger.Debug("format cache - cleared: " + reason);
        }

        private void drop_old_items() {
            var indexes = cache_.Values.Select(x => x.cache_index).OrderBy(x => x).ToList();
            int middle = indexes[indexes.Count / 2];
            cache_ = cache_.Where(x => x.Value.cache_index >= middle).ToDictionary(x => x.Key, x => x.Value);

            dump_cache_info();
        }

        private void dump_cache_info() {
            logger.Debug("format cache = " + cache_.Count + " entries, next_idx =" + next_cache_index_);
        }
    }
}
