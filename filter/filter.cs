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

namespace LogWizard {
    class filter : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public class match {
            // this contains what filters were matched - we need to know this, so that we can later apply 'additions'
            //
            // if this is an empty array, then this match is actually an addition (or, the filter contains no rows -> thus, we return the whole file)
            public BitArray matches = null;

            public filter_line.font_info font = null;
            public line line = null;
            public int line_idx = 0;
        }


        private List<filter_row> rows_ = new List<filter_row>();

        private log_line_reader new_log_ = null, old_log_ = null;
        // 1.0.75+ - wait for the full log to be read first - in the hopes of using les memory on huge logs
        private bool new_log_fully_read_at_least_once_ = false;

        // the filter matches
        // FIXME make this a list!
        private Dictionary<int, match> matches_ = new Dictionary<int, match>();
        // ... the indexes, in sorted order
        private memory_optimized_list<int> match_indexes_ = new memory_optimized_list<int>() { min_capacity = app.inst.no_ui.min_filter_capacity }; 

        private bool rows_changed_ = false;

        private Thread compute_matches_thread_ = null;
        private bool disposed_ = false;

        private bool post_force_recompute_matches_ = false;

        private bool is_up_to_date_ = false;

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
            get { lock(this) return matches_.Count; }
        }

        public bool is_up_to_date {
            get { lock (this) return is_up_to_date_;  }
        }

        // note: the only time this can return null is this: since we're refreshing on another thread,
        //       we might at some point get a match_count, and while we're retrieving the items, the matches array clears
        public match match_at(int idx) {
            lock (this) 
                return idx < matches_.Count ? matches_[match_indexes_[idx]] : null;            
        }

        // this will return the log we last read the matches from
        internal log_line_reader log {
            get { lock(this) return old_log_; }
        }

        // note: 
        // since we're thread-safe: the rows' can be updated in the following ways:
        // - either the full array points to the new array
        // - enabled/dimmed has changed - from different items in the rows' array
        //
        // in case any of the filter rows changes, we will end up pointing to the new filter_row array
        public void update_rows(List<filter_row> rows) {
            lock (this) {
                // note: if filter is the same, we want to preserve anything we have cached, about the filter
                rows_changed_ = false;
                bool same = false;
                if (rows_.Count == rows.Count) {
                    same = true;
                    for (int i = 0; i < rows_.Count; ++i)
                        if (!rows[i].same(rows_[i]))
                            same = false;
                }
                if (same) {
                    // at this point, see if user has fiddled with Enabled/Dimmed
                    bool same_ed = true;
                    for (int i = 0; i < rows_.Count; ++i)
                        if (rows[i].enabled != rows_[i].enabled || rows[i].dimmed != rows_[i].dimmed) {
                            same_ed = false;
                            // note: we want to preserve what we already have cached (in the filter rows)
                            rows_[i].enabled = rows[i].enabled;
                            rows_[i].dimmed = rows[i].dimmed;
                        }

                    if (!same_ed) 
                        rows_changed_ = true;
                } else {
                    rows_changed_ = true;
                    rows_ = rows;                    
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
                compute_matches_impl(new_, old);
                // the reason I do this here - I need to let the main thread know htat the log was fully set (and the matches are from This log)
                // ONLY after I have read from it at least once
                lock (this)
                    old_log_ = new_;

                Thread.Sleep(100);
            }
        }

        // this gets called only on the compute_matches_thread - since we want to update the matches_/match_indexes_ on that thread!
        private void force_recompute_matches() {
            lock (this) {
                post_force_recompute_matches_ = false;
                if (matches_.Count < 1)
                    return;
                matches_.Clear();
                match_indexes_.Clear();
                //old_log_ = null;
            }
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

            /* problem - the full log colors are not updated correctly, AND we crash on huge log (even with full log off)
            // 1.0.75+ - wait until log has fully loaded - in the hopes of using less memory
            if (new_log == old_log) {
                bool at_least_once;
                lock (this) at_least_once = new_log_fully_read_at_least_once_;
                if (!at_least_once) {
                    new_log.refresh();
                    if (!new_log.up_to_date)
                        return;
                    lock (this) new_log_fully_read_at_least_once_ = true;
                }
            }
            */

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
                int expected_capacity = (new_log.line_count - old_line_count) / 5;
                // the filter matches
                Dictionary<int, match> new_matches = new Dictionary<int, match>();
                // ... the indexes, in sorted order
                memory_optimized_list<int> new_indexes = new memory_optimized_list<int>() { min_capacity = expected_capacity }; 

                // from old_lines to log.line_count -> these need recomputing
                int old_match_count;
                lock (this)
                    old_match_count = match_indexes_.Count;
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
                                bool is_font_only = row.raw_font != null;
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
                                font = filter_line.font_info.default_;
                        }
                        new_matches.Add(line_idx, new match {
                            font = font, line = new_log.line_at(line_idx), line_idx = line_idx, matches = new BitArray(matches)
                        });
                        new_indexes.Add(line_idx);
                        continue;
                    }

                    bool any_filter = (rows.Count > 0);
                    if (!any_filter) {
                        new_matches.Add(line_idx, new match { matches = new BitArray(0), line = new_log.line_at(line_idx), line_idx = line_idx, font = filter_line.font_info.default_ });
                        new_indexes.Add(line_idx);
                    }
                }

                lock (this) {
                    foreach ( var kv in new_matches)
                        matches_.Add(kv.Key, kv.Value);
                    match_indexes_.AddRange(new_indexes);
                }

                apply_additions(old_match_count, new_log, rows);
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
            int new_match_count;
            lock (this) new_match_count = match_indexes_.Count;
            for (int match_idx = old_match_count; match_idx < new_match_count; ++match_idx) {
                int line_idx;
                lock(this) 
                    line_idx = match_indexes_[match_idx];
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

            lock(this)
                foreach (var add_idx in additions)
                    add_addition_line(add_idx.Key, add_idx.Value, log);
        }

        private static BitArray empty_match = new BitArray(0);
        private void add_addition_line(int line_idx, Color fg, log_line_reader log) {
            // if insert_idx > 0 , that means we already have it
            int insert_idx = match_indexes_.BinarySearch(line_idx);
            if (insert_idx < 0) {
                match_indexes_.Insert(~insert_idx, line_idx);
                matches_.Add(line_idx, new match {
                    matches = empty_match, line = log.line_at(line_idx), line_idx = line_idx, font = new filter_line.font_info {
                        bg = Color.White, fg = fg
                    }
                } );
            }
        }

        public void Dispose() {
            disposed_ = true;
        }
    }
}