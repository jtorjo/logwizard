using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using BrightIdeasSoftware;
using LogWizard;
using lw_common;

namespace lw_common.ui {
    internal class list_data_source : AbstractVirtualListDataSource, IDisposable {

        private VirtualObjectListView lv_ = null;
        private log_view parent_;

        private readonly filter.match_list items_;

        private filter.match_list full_log_items_ = null;

        // if false (default), we're showing everything (either the current view, or the full log)
        // if true, we're filtering (based on whether showing full log or not)
        private bool filter_view_ = false;

        // if true, we're showing the full log
        //
        // if filter_view is false, then we're showing everything
        // if filter_view is true, we're filtering on the full log items
        private bool show_full_log_ = false;

        // note: when the user toggles something, we set this values, and then force the filter to recompute.
        //
        //       the idea is this: until we fully recompute everything, we keep returning the old items
        //       (this way we avoid issues like - not finding an item while we toggled something on or off)
        private bool filter_view_now_ = false;
        private bool show_full_log_now_ = false;

        // when this is set to true, the UI needs update
        private bool needs_ui_update_ = false;

        private easy_mutex change_event_ = new easy_mutex("list_data_source");

        private bool disposed_ = false;

        // I chose to use line indexes instead of row indexes - these will work even if the filter changes! (like, some rows are enabled/disabled)
        private memory_optimized_list<int> sorted_line_indexes_ = null; 

        // called for each item to filter in/out 
        public delegate bool filter_func(match_item item, bool applied_on_full_log);

        public filter_func item_filter;

        private int item_count_ = 0;

        public list_data_source(VirtualObjectListView lv, log_view parent ) : base(lv) {
            lv_ = lv;
            parent_ = parent;
            items_ = parent.filter.matches ;
            change_event_.current_thread_is_owner();

            new Thread(update_filter_thread) {IsBackground = true}.Start();
        }

        public string name {
            set {
                items_.name = "list_data " + value; 
                change_event_.friendly_name = "list_data " + value; 
            }
        }

        private filter.match_list full_log_items {
            get {
                if (full_log_items_ == null)
                    full_log_items_ = parent_.lv_parent.full_log.filter.matches;
                Debug.Assert(full_log_items_ != null);
                return full_log_items_;
            }
        }

        public bool filter_view {
            get { return filter_view_now_; }
        }

        public bool show_full_log {
            get { return show_full_log_now_; }
        }

        // set both filter options in one step (so that change_event_ is triggered only once!)
        public void set_filter(bool filter_view, bool show_full_log) {
            lock (this) {
                if (filter_view == filter_view_now_ && show_full_log == show_full_log_now_)
                    return;

                filter_view_now_ = filter_view;
                show_full_log_now_ = show_full_log;
            }
            change_event_.release_and_reaquire();
        }

        public bool needs_ui_update {
            get { lock(this) return needs_ui_update_; }
            set { lock(this) needs_ui_update_ = value; }
        }

        public override int GetObjectIndex(object o) {
            bool filter_view;
            bool show_full_log;
            lock (this) {
                filter_view = filter_view_;
                show_full_log = show_full_log_;
            }

            var i = o as match_item;
            if (!filter_view)
                return show_full_log ? full_log_items.index_of(i) : items_.index_of(i);

            // here, it's filtered
            int line_idx = i.line_idx;
            lock (this) {
                if (sorted_line_indexes_ == null)
                    return -1;

                int found = sorted_line_indexes_.BinarySearch(line_idx);
                return found;
            }
        }

        public override object GetNthObject(int n) {
            return item_at(n);
        }

        public override int GetObjectCount() {
            return item_count_;
        }

        public int item_count {
            get { return item_count_; }
        }

        internal match_item item_at(int idx) {
            bool filter_view;
            bool show_full_log;
            lock (this) {
                filter_view = filter_view_;
                show_full_log = show_full_log_;
            }

            if (!filter_view)
                // not filtered
                return (show_full_log ?  full_log_items.match_at(idx) : items_.match_at(idx)) as match_item;

            // here, we're filtering the items
            int line_index = -1;
            lock (this) {
                Debug.Assert(sorted_line_indexes_ != null);
                if ( sorted_line_indexes_ != null)
                    line_index = idx < sorted_line_indexes_.Count ? sorted_line_indexes_[idx] : -1;
            }
            if (line_index < 0)
                return null;

            // we prefer finding from the current view - so we actually preserve colors and such
            var found_in_items = items_.binary_search(line_index).Item1;
            var found = found_in_items ?? full_log_items.binary_search(line_index).Item1;
            return found as match_item;
        }

        private int item_count_at_this_time {
            get {
                bool filter_view;
                bool show_full_log;
                lock (this) {
                    filter_view = filter_view_;
                    show_full_log = show_full_log_;
                }

                if (!filter_view)
                    // not filtered
                    return show_full_log ? full_log_items.count : items_.count;

                // here, we're filtering the items
                lock (this) {
                    Debug.Assert(sorted_line_indexes_ != null);
                    return sorted_line_indexes_ != null ? sorted_line_indexes_.Count : 0;
                }
            }
        }


        public void refresh() {
            if ( items_.count == 0)
                lv_.ClearObjects();

            // I want the item count to be constant until the next refresh - otherwise, we could end up with more items (for instance, new lines added)
            // the client code always asks for model_.item_count - i don't want to go out of sync with what the listview thinks
            // (and the list view thinks what it was last told at UpdateVirtualListSize())
            item_count_ = item_count_at_this_time;
            lv_.UpdateVirtualListSize();                
        }

        private void update_filter_thread() {
            while (!disposed_) {
                change_event_.wait_and_release();

                bool filter_view;
                bool show_full_log;
                lock (this) {
                    filter_view = filter_view_now_;
                    show_full_log = show_full_log_now_;
                }

                // see what changed
                if (filter_view) 
                    // the user toggled on filtering
                    run_filter(show_full_log);
                
                
                lock (this) {
                    filter_view_ = filter_view_now_;
                    show_full_log_ = show_full_log_now_;

                    if (!filter_view_)
                        // here, user toggled off filtering - so, I'm either showing the view, or the full log
                        sorted_line_indexes_ = null;

                    needs_ui_update_ = true;
                }
            }
        }

        private void run_filter(bool run_on_full_log) {
            filter_func item_filter;
            lock (this) item_filter = this.item_filter;

            Debug.Assert(item_filter != null);
            if (item_filter == null)
                return;

            memory_optimized_list<int> line_indexes = new memory_optimized_list<int>() { min_capacity = app.inst.no_ui.min_list_data_source_capacity };
            var items = run_on_full_log ? full_log_items : items_;

            int count = items.count;
            for (int idx = 0; idx < count; ++idx) {
                var i = items.match_at(idx) as match_item;
                if ( item_filter(i, run_on_full_log))
                    if ( i.line_idx >= 0)
                        line_indexes.Add( i.line_idx);
            }

            lock (this)
                sorted_line_indexes_ = line_indexes;
        }


        public void Dispose() {
            disposed_ = true;
            change_event_.release_and_reaquire();
        }
    }
}
