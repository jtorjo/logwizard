/* 
 * Copyright (C) 2014-2015 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using lw_common;

namespace LogWizard {

    class filter : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public class match {
            // this contains what filters were matched - we need to know this, so that we can later apply 'additions'
            // if this is an empty array, then this match is actually an addition (or, the filter contains no rows -> thus, we return the whole file)
            //
            // also used to know how many lines were matched by a filter
            public readonly BitArray matches = null;

            public readonly filter_line.font_info font = null;
            public readonly line line = null;
            // convention - if we can't find a specific line (match_at), we'll return a line with the index -1
            //              (so that we don't return null)
            public readonly int line_idx = 0;

            public match(BitArray matches, filter_line.font_info font, line line, int lineIdx) {
                this.matches = matches;
                this.font = font;
                this.line = line;
                line_idx = lineIdx;
            }
        }

        // 1.0.84b+ the only thing this provides is thread-safe access - so both of us and the log view can access this fine and dandy
        public class match_list {
            private string name_ = "";
            public string name {
                get { return name_; }
                set {
                    name_ = value;
                    matches_.name = "filter_m " + name;
                }
            }

            private match empty_match;

            public match_list(match empty_match) {
                this.empty_match = empty_match;
            }

            // the filter matches
            //
            // 1.0.84+ the log view accesses this list directly for its items; I'm doing this to save_to memory on large files
            //         this will save_to us quite a few extra pointers, which on x64 can add up to a LOT
            private memory_optimized_list<match> matches_ = new memory_optimized_list<match>() {
                min_capacity = app.inst.no_ui.min_list_data_source_capacity, increase_percentage = 0.4            
            };

            public int count {
                get { lock (this) return matches_.Count;  }
            }

            // in case we're asking for an invalid line, just return something that is fully empty
            // next time, the log view will see that we've updated
            public match match_at(int idx) {
                lock (this) 
                    return idx >= 0 && idx < matches_.Count ? matches_[idx] : empty_match;
            }

            public void clear() {
                lock (this)
                    matches_.Clear();
            }

            // 1.0.84 note: if this is proves too slow, i'll do a binary search by m.line_idx !!!
            public int index_of(match m) {
                lock (this) {
                    //return matches_.IndexOf(m);
                    int idx = matches_.binary_search_closest(x => x.line_idx, m.line_idx).Item2;
                    Debug.Assert(matches_[idx] == m);
                    return idx;
                }
            }

            public void add_range(memory_optimized_list<match> new_matches) {
                lock (this) {
                    bool optimize = matches_.Count == 0 && new_matches.Count > app.inst.no_ui.min_filter_capacity;
                    if (!optimize) {
                        matches_.AddRange(new_matches);
                    } else {
                        // optimization - reuse this memory
                        new_matches.name = matches_.name;
                        matches_ = new_matches;
                    }
                }                
            }

            public void prepare_add(int count) {
                lock(this)
                    matches_.prepare_add(count);
            }

            // returns the index where we can insert a line - if I return -1, it means there's already a line with this index
            public int insert_line_idx(int line_idx) {
                lock (this) {
                    int insert_idx = matches_.binary_search_insert(x => x.line_idx, line_idx);
                    if (insert_idx < matches_.Count)
                        if (matches_[insert_idx].line_idx == line_idx)
                            return -1; // we already have a match

                    return insert_idx;
                }
            }

            public void insert(int insert_idx, match m) {
                lock(this)
                    matches_.Insert(insert_idx, m);
            }

            public Tuple<match, int> binary_search(int line_idx) {
                lock (this)
                    return matches_.binary_search(x => x.line_idx, line_idx);
            }

            public Tuple<match, int> binary_search_closest(int line_idx) {
                lock (this)
                    return matches_.binary_search_closest(x => x.line_idx, line_idx);
            }
            public Tuple<match, int> binary_search_closest(DateTime time) {
                lock (this)
                    return matches_.binary_search_closest(x => x.line.time, time);
            }
        }


        private List<filter_row> rows_ = new List<filter_row>();

        private log_line_reader new_log_ = null, old_log_ = null;
        // 1.0.75+ - wait for the full log to be read first - in the hopes of using les memory on huge logs
        private bool new_log_fully_read_at_least_once_ = false;

        // the filter matches
        private match_list matches_ ;

        public delegate match create_match_func(BitArray matches, filter_line.font_info font, line line, int lineIdx);
        private create_match_func create_match;

        private bool rows_changed_ = false;

        private Thread compute_matches_thread_ = null;
        private bool disposed_ = false;

        private bool post_force_recompute_matches_ = false;

        private bool is_up_to_date_ = false;

        // debugging/testing only
        private string name_ = "";

        public filter(create_match_func creator) {
            create_match = creator;

            var empty_match = new_match(new BitArray(0), line.empty_line(), -1, filter_line.font_info.default_font_copy);
            matches_ = new match_list(empty_match);
        }

        public override string ToString() {
            return name_;
        }

        public match_list matches {
            get { return matches_; }
        }

        public string name {
            get { return name_; }
            set {
                name_ = value;
                matches_.name = "filter_m " + name;
            }
        }

        public void compute_matches(log_line_reader log) {
            Debug.Assert(log != null);
            lock (this) {
                if (new_log_ != log)
                    is_up_to_date_ = false;
                new_log_ = log;
            }

            start_compute_matches_thread();
        }

        public int row_count {
            get { lock(this) return rows_.Count; }
        }

        public bool rows_changed {
            get { lock(this) return rows_changed_; }
        }

        public int match_count {
            get { return matches_.count; }
        }

        public bool is_up_to_date {
            get { lock (this) return is_up_to_date_;  }
        }

        // note: the only time this can return null is this: since we're refreshing on another thread,
        //       we might at some point get a match_count, and while we're retrieving the items, the matches array clears
        public match match_at(int idx) {
            return matches_.match_at(idx);
        }

        // this will return the log we last read the matches from
        internal log_line_reader log {
            get { lock(this) return old_log_; }
        }

        private bool same_rows_regarless_enabled_dimmed(List<raw_filter_row> rows) {
            bool same = false;
            if (rows_.Count == rows.Count) {
                same = true;
                for (int i = 0; i < rows_.Count; ++i)
                    if (rows[i].unique_id != rows_[i].unique_id)
                        same = false;
            }

            return same;
        }

        private bool same_rows(List<raw_filter_row> rows) {
            bool same = false;
            if (rows_.Count == rows.Count) {
                same = true;
                for (int i = 0; i < rows_.Count; ++i)
                    if (rows[i].enabled != rows_[i].enabled || rows[i].dimmed != rows_[i].dimmed || rows[i].same(rows_[i]))
                        same = false;
            }

            return same;
        }

        /*  preserves as much of the cache as possible

            It can preserve the cache in the following ways:
            - if you add a new filter row, it will preserve the existing filter rows (and add the new row at the end)
            - if you delete a filter row, it will preserve the remaining filter rows
            - if you move a filter row up/down, it wil preserve its computations
        */
        private void preserve_cache(List<raw_filter_row> rows) {
            var old_ids = rows_.ToDictionary(r => r.unique_id, r => r);
            var new_ids = rows.ToDictionary(r => r.unique_id, r => r);

            List<filter_row> new_rows = new List<filter_row>();
            int misses = 0;
            foreach (var id in new_ids) {
                if (old_ids.ContainsKey(id.Key)) {
                    var new_ = id.Value;
                    var old = old_ids[id.Key];
                    old.preserve_cache_copy(new_);
                    new_rows.Add(old);
                } else {
                    // this is completely new (or a changed filter row)
                    new_rows.Add( new filter_row(id.Value));
                    ++misses;
                }
            }
            rows_ = new_rows;
            logger.Info("[filter] preserving cache " + (new_log_ != null ? new_log_.name : "") + ", misses = " + misses);
        }
 
        // note: 
        // since we're thread-safe: the rows' can be updated in the following ways:
        // - either the full array points to the new array
        // - enabled/dimmed has changed - from different items in the rows' array
        //
        // in case any of the filter rows changes, we will end up pointing to the new filter_row array
        public void update_rows(List<raw_filter_row> rows) {
            lock (this) {
                // note: if filter is the same, we want to preserve anything we have cached, about the filter
                rows_changed_ = false;
                bool same = same_rows_regarless_enabled_dimmed(rows);
                if (same) {
                    // at this point, see if user has fiddled with Enabled/Dimmed
                    bool same_ed = same_rows(rows);
                    if ( !same_ed)
                        for (int i = 0; i < rows_.Count; ++i) 
                            // note: we want to preserve what we already have cached (in the filter rows)
                            rows_[i].preserve_cache_copy(rows[i]);
                            /* old code
                            rows_[i].enabled = rows[i].enabled;
                            rows_[i].dimmed = rows[i].dimmed;
                            */

                    if (!same_ed) 
                        rows_changed_ = true;
                } else {
                    rows_changed_ = true;
                    preserve_cache(rows);
                }

                if (!rows_changed_)
                    return;

                // this forces a recompute matches on the other thread
                post_force_recompute_matches_ = true;
            }
        }

        private void compute_matches_thread() {
            while (!disposed_) {
                bool needs_recompute = false;
                lock (this)
                    needs_recompute = post_force_recompute_matches_;
                if (needs_recompute)
                    force_recompute_matches();

                log_line_reader new_, old;
                lock (this) {
                    new_ = new_log_;
                    old = old_log_;
                    if (new_ != old)
                        new_log_fully_read_at_least_once_ = false;
                }
                if (needs_recompute)
                    old = null;
                compute_matches_impl(new_, old);
                // the reason I do this here - I need to let the main thread know htat the log was fully set (and the matches are from This log)
                // ONLY after I have read from it at least once
                lock (this)
                    old_log_ = new_;

                Thread.Sleep(100);
            }
        }

        // this gets called only on the compute_matches_thread - since we want to update the matches_ on that thread!
        private void force_recompute_matches() {
            lock (this) {
                post_force_recompute_matches_ = false;
                //old_log_ = null;
            }
            if (matches_.count < 1)
                return;
            matches_.clear();
        }

        private void start_compute_matches_thread() {
            bool needs;
            lock (this)
                needs = compute_matches_thread_ == null;
            if (needs) 
                lock (this) 
                   if ( compute_matches_thread_ == null) {
                        compute_matches_thread_ = new Thread(compute_matches_thread) {IsBackground = true};
                        compute_matches_thread_.Start();
                    }            
        }

        private void compute_matches_impl(log_line_reader new_log, log_line_reader old_log) {
            Debug.Assert(new_log != null);

            if ( app.inst.no_ui.read_full_log_first)
                // 1.0.76d+ - wait until log has fully loaded - in the hopes of using less memory
                if (new_log == old_log) {
                    bool at_least_once;
                    lock (this) at_least_once = new_log_fully_read_at_least_once_;
                    if (!at_least_once) {
                        if (!new_log.parser_up_to_date)
                            return;
                        lock (this) new_log_fully_read_at_least_once_ = true;
                    }
                }

            // fixme: start with number of lines / 20 (capacity), and go from there

            int old_line_count = new_log.line_count;
            new_log.refresh();
            if (new_log != old_log || new_log.forced_reload) {                
                logger.Info(new_log != old_log ? "[filter] new log " + new_log.name : "[filter] forced refresh of " + new_log.name);
                old_line_count = 0;
                force_recompute_matches();
            }
            bool has_new_lines = (old_line_count != new_log.line_count);

            // get a pointer to the rows_; in case it changes on the main thread, we don't care,
            // since next time we will have the new rows
            List<filter_row> rows;
            lock (this) rows = rows_;

            if ( old_line_count == 0)
                foreach ( filter_row row in rows)
                    row.refresh();

            foreach ( filter_row row in rows)
                row.compute_line_matches(new_log);

            if (has_new_lines) {
                bool is_full_log = row_count < 1;
                int expected_capacity = is_full_log ? (new_log.line_count - old_line_count) : (new_log.line_count - old_line_count) / 5;
                // the filter matches
                memory_optimized_list<match> new_matches = new memory_optimized_list<match>() { min_capacity = expected_capacity, name = "temp_m " + name, increase_percentage = .7 };

                // from old_lines to log.line_count -> these need recomputing
                int old_match_count = matches_.count;
                BitArray matches = new BitArray(rows.Count);

                for (int line_idx = old_line_count; line_idx < new_log.line_count; ++line_idx) {
                    bool any_match = false;
                    bool any_non_apply_to_existing_lines_filters = false;
                    // 1.0.69 added "apply to existing filters"
                    for (int filter_idx = 0; filter_idx < matches.Length; ++filter_idx) {
                        var row = rows[filter_idx];
                        if ((row.enabled || row.dimmed) && !row.apply_to_existing_lines) {
                            matches[filter_idx] = row.line_matches.Contains(line_idx);
                            any_non_apply_to_existing_lines_filters = true;
                        } else
                            matches[filter_idx] = false;
                        if (matches[filter_idx])
                            any_match = true;
                    }
                    if (!any_non_apply_to_existing_lines_filters)
                        // in this case - all filters apply to existing lines - thus, by default, we show all the lines
                        any_match = true;

                    // 1.0.69 "apply to existing filters" is applied afterwards
                    filter_line.font_info existing_filter_font = null;
                    if ( any_match)
                        for (int filter_idx = 0; filter_idx < matches.Length && any_match; ++filter_idx) {
                            var row = rows[filter_idx];
                            if ((row.enabled || row.dimmed) && row.apply_to_existing_lines) {
                                bool is_font_only = row.has_font_info;
                                if (row.line_matches.Contains(line_idx)) {
                                    if (existing_filter_font == null && is_font_only) {
                                        // in this case, use the font from "apply to existing filters" - only if the user has specifically set it
                                        existing_filter_font = row.get_match(line_idx).font;
                                        matches[filter_idx] = true;
                                    }
                                } else if (!is_font_only)
                                    // we're filtering this line out
                                    any_match = false;
                            }
                        }

                    if (any_match) {
                        filter_line.font_info font;
                        if (existing_filter_font != null)
                            font = existing_filter_font;
                        else {
                            // in this case, prefer the first "enabled" filter
                            int enabled_idx = -1;
                            for (int filter_idx = 0; filter_idx < matches.Length && enabled_idx < 0; ++filter_idx)
                                if (matches[filter_idx] && rows[filter_idx].enabled)
                                    enabled_idx = filter_idx;
                            int used_idx = -1;
                            if (enabled_idx < 0)
                                for (int filter_idx = 0; filter_idx < matches.Length && used_idx < 0; ++filter_idx)
                                    if (matches[filter_idx] && rows[filter_idx].dimmed)
                                        used_idx = filter_idx;
                            if (enabled_idx >= 0 || used_idx >= 0) {
                                int idx = enabled_idx >= 0 ? enabled_idx : used_idx;
                                font = rows[idx].get_match(line_idx).font;
                            } else
                                font = filter_line.font_info.default_font;
                        }
                        new_matches.Add( new_match(new BitArray(matches), new_log.line_at(line_idx), line_idx, font ));
                        continue;
                    }

                    bool any_filter = (rows.Count > 0);
                    if (!any_filter) 
                        new_matches.Add( new_match(new BitArray(0), new_log.line_at(line_idx), line_idx, filter_line.font_info.default_font ));
                }

                matches_.add_range(new_matches);

                apply_additions(old_match_count, new_log, rows);
                if (new_matches.Count > app.inst.no_ui.min_filter_capacity) {
                    logger.Debug("[memory] GC.collect - from filter " + name );
                    GC.Collect();
                }
            }

            bool is_up_to_date = new_log.up_to_date;
            lock (this)
                is_up_to_date_ = is_up_to_date;

        }


        private void apply_additions(int old_match_count, log_line_reader log, List<filter_row> rows ) {
            // FIXME note: we should normally care about the last match before old_match_count as well, to see maybe it still matches some "addition" lines
            //             but we ignore that for now
            //
            // when impleemnting the above, make sure to find the last matched line, not an existing addition

            bool has_additions = false;
            foreach( filter_row row in rows)
                if (row.additions.Count > 0)
                    has_additions = true;
            if (!has_additions)
                // optimize for when no additions
                return;

            Dictionary<int, Color> additions = new Dictionary<int, Color>();
            int new_match_count = matches_.count;
            for (int match_idx = old_match_count; match_idx < new_match_count; ++match_idx) {
                int line_idx = matches_.match_at(match_idx).line_idx;
                var match = match_at(match_idx);

                int matched_filter = -1;
                for ( int filter_idx = 0; filter_idx < match.matches.Length && matched_filter < 0; ++filter_idx)
                    if (match.matches[filter_idx])
                        matched_filter = filter_idx;

                if (matched_filter >= 0) {
                    Color gray_fg = util.grayer_color(rows[matched_filter].get_match(line_idx).font.fg);
                    foreach (var addition in rows[matched_filter].additions) {
                        switch (addition.type) {
                        case addition.number_type.lines:
                            for (int i = 0; i < addition.number; ++i) {
                                int add_line_idx = line_idx + (addition.add == addition.add_type.after ? i : -i);
                                if (add_line_idx >= 0 && add_line_idx < log.line_count)
                                    additions.Add(add_line_idx, gray_fg);
                            }
                            break;

                        case addition.number_type.millisecs:
                            DateTime start = util.str_to_time(log.line_at(line_idx).part(info_type.time));
                            for (int i = line_idx; i >= 0 && i < log.line_count;) {
                                i = i + (addition.add == addition.add_type.after ? 1 : -1);
                                if (i >= 0 && i < log.line_count) {
                                    DateTime now = util.str_to_time(log.line_at(i).part(info_type.time));
                                    int diff = (int) ((now - start).TotalMilliseconds);
                                    bool ok =
                                        (addition.add == addition.add_type.after && diff <= addition.number) ||
                                        (addition.add == addition.add_type.before && -diff <= addition.number);
                                    if (ok && !additions.ContainsKey(i))
                                        additions.Add(i, gray_fg);
                                    else
                                        break;
                                }
                            }
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                        }
                    }
                }
            }

            matches_.prepare_add( additions.Count);
                foreach (var add_idx in additions)
                    add_addition_line(add_idx.Key, add_idx.Value, log);
        }


        private static BitArray empty_match = new BitArray(0);
        /*
        private void add_addition_line(int line_idx, Color fg, log_line_reader log) {
            // if insert_idx > 0 , that means we already have it
            int insert_idx = match_indexes_.BinarySearch(line_idx);
            if (insert_idx < 0) {
                match_indexes_.Insert(~insert_idx, line_idx);
                matches_.Insert(~insert_idx, new match {
                    matches = empty_match, line = log.line_at(line_idx), line_idx = line_idx, font = new filter_line.font_info {
                        bg = Color.White, fg = fg
                    }
                } );
            }
        }
        */

        private match new_match(BitArray ba, line l, int idx, filter_line.font_info f ) {
            match m = create_match(ba, f, l, idx);

//            if ( idx >= 0)
  //              logger.Debug("[filter] " + name + " " + (idx+1) + "-> " + f);

            return m;
        }

        private void add_addition_line(int line_idx, Color fg, log_line_reader log) {
            // IMPORTANT: I did NOT test the binary_search_insert!
            int insert_idx = matches_.insert_line_idx(line_idx);
            if ( insert_idx >= 0)
                matches_.insert(insert_idx, new_match(empty_match, log.line_at(line_idx), line_idx, new filter_line.font_info {
                        bg = Color.White, fg = fg
                    }));            
        }


        public void Dispose() {
            disposed_ = true;
        }
    }
}