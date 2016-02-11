/* 
 * Copyright (C) 2014-2016 John Torjo
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
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;

namespace lw_common.ui {
    public static class log_view_show_columns {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal const int MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS = 10;
        private const int DEFAULT_COL_WIDTH = 80;

        // if too many columns in a log, by default show only a few in the log view - the rest are available via the Details pane
        private const int MAX_DEFAULT_VIEW_COLUMNS = 6;

        static private bool has_value_at_column(log_view lv, info_type type, int max_rows_to_check) {
            // msg is always shown
            if (type == info_type.msg)
                return true;
            if (type == info_type.line || type == info_type.view)
                return true;

            if (max_rows_to_check == 0)
                // before reading anything from the log
                return false;

            var aliases = lv.filter.log.aliases;
            // 1.5.4+ if the user has already specified clearly that we have this column, we consider it true
            if (aliases.has_column(type))
                return true;

            for (int idx = 0; idx < max_rows_to_check; ++idx) {
                var i = lv.item_at(idx) ;
                if (i.line_idx < 0)
                    continue;
                if (log_view_cell.cell_value_by_type(i, type) != "")
                    return true;
            }
            return false;
        }

        static public void refresh_visible_columns(List<log_view> all_views, log_view full_log) {
            Debug.Assert(full_log.is_full_log);
            if (refresh_visible_columns_any_change(full_log)) {
                if (full_log.available_columns_refreshed_ >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS) {
                    // at this point, we need to apply custom positions to all views 
                    // if they were cached (computed before) -> we already have them
                    // if they were not cached, we force them now
                    if ( full_log.column_positions == "")
                        full_log.column_positions = column_positions_as_string(full_log);
                    foreach (log_view lv in all_views)
                        if (lv.column_positions == "")
                            lv.column_positions = full_log.column_positions;
                }

                foreach (log_view lv in all_views) 
                    lv.list.SuspendLayout();
                full_log.list.SuspendLayout();

                // notify all other views
                foreach (log_view lv in all_views) 
                    refresh_visible_columns(lv);

                // now, apply the saved-custom-positions, if any
                foreach (log_view lv in all_views)
                    apply_column_positions(lv);
                apply_column_positions(full_log);

                full_log.update_column_names();
                foreach (log_view lv in all_views)
                    lv.update_column_names();

                foreach (log_view lv in all_views) 
                    lv.list.ResumeLayout(true);
                full_log.list.ResumeLayout(true);
            }
        }

        static internal void apply_column_positions(log_view lv) {
            if (lv.column_positions != "") {
                if (lv.column_positions == column_positions_as_string(lv))
                    // in this case, nothing changed
                    return;
                load_column_positions(lv, lv.column_positions);
            }
        }

        // note: only after I have at least MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS, can I know the available columns
        //       even though I may have a valid column_positions string, I will use it only when we have enough rows (MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
        static private bool refresh_visible_columns_any_change(log_view full_log) {
            Debug.Assert(full_log.is_full_log);

            bool force_refresh = false;
            // 1.6.13+ - if we already have the cached available columns (computed before), just use them
            if (full_log.available_columns_refreshed_ == -1)
                if (full_log.available_columns.Count > 0 ) {
                    // at this point, I already have computed the columns - they were computed last time we ran
                    full_log.use_previous_available_columns_ = true;
                    // here, I make sure every other view reuses what we already have cached
                    force_refresh = true;
                    full_log.available_columns_refreshed_ = 0;
                }

            if (full_log.available_columns_refreshed_ >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
                return false;

            int count = full_log.item_count;
            bool needs_refresh = count != full_log.available_columns_refreshed_;
            if (needs_refresh) {
                // if they were previously computed, the only time I recompute them, is after we have at least MIN_ROWS
                bool needs_recompute_now = count > 0 && ( !full_log.use_previous_available_columns_ || count >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS);
                full_log.available_columns_refreshed_ = count;
                if (needs_recompute_now) {
                    full_log.list.SuspendLayout();

                    // this is the first time we compute the columns for this log - just in case they were moved around in the previous log(s),
                    // we want to reset them back
                    foreach (var column in full_log.list.AllColumns) 
                        column.LastDisplayIndex = column.fixed_index();

                    List<info_type> available_columns = new List<info_type>();
                    int row_count = Math.Min(count, MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS);
                    foreach (info_type type in Enum.GetValues(typeof (info_type))) {
                        if (type == info_type.max)
                            continue;
                        bool is_visible = has_value_at_column(full_log, type, row_count);
                        show_column(log_view_cell.column(full_log, type), DEFAULT_COL_WIDTH, is_visible);
                        if (is_visible)
                            available_columns.Add(type);
                    }

                    full_log.available_columns = available_columns;
                    logger.Debug("available columns (" + row_count + ") - " + util.concatenate(available_columns, ", "));

                    full_log.list.RebuildColumns();
                    full_log.list.ResumeLayout(true);
                } else
                    // if we end up here, we've already refreshed the other views with what we have cached
                    needs_refresh = false;

                if (!full_log.use_previous_available_columns_ && count >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
                    keep_only_important_columns(full_log);
            }

            if (count >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS) 
                full_log.available_columns_refreshed_ = MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS;
            return needs_refresh || force_refresh;
        }

        static private void keep_only_important_columns(log_view full_log) {
            Debug.Assert(full_log.is_full_log);
            // the idea is that we do this only the first time when showing the log
            // after that, the user can customize it as he wishes
            Debug.Assert( !full_log.use_previous_available_columns_ );

            var available = full_log.available_columns.OrderBy(x => -info_type_io.show_in_view_by_default(x) ).ToList();
            bool has_date_column = available.Contains(info_type.date);
            if (available.Count <= MAX_DEFAULT_VIEW_COLUMNS && !has_date_column)
                // we're fine
                return;

            // too many columns, hide a few
            full_log.list.SuspendLayout();

            var to_erase = available.GetRange(MAX_DEFAULT_VIEW_COLUMNS, available.Count - MAX_DEFAULT_VIEW_COLUMNS);
            if ( has_date_column && !to_erase.Contains(info_type.date))
                to_erase.Add(info_type.date);
            foreach ( var col_type in to_erase)
                show_column( log_view_cell.column(full_log, col_type), DEFAULT_COL_WIDTH, false );

            full_log.list.RebuildColumns();
            full_log.list.ResumeLayout(true);

            // recompute visible columns
            full_log.column_positions = column_positions_as_string(full_log);

            full_log.lv_parent.needs_details_pane();
        }

        static private void refresh_visible_columns(log_view lv) {
            Debug.Assert(!lv.is_full_log);
            if (lv.column_positions != "")
                // in this case, we know everything about how to position the columns, and which is visible/invisible
                return;

            var available = lv.available_columns;
            for (int col_idx = 0; col_idx < lv.list.AllColumns.Count; ++col_idx) {
                var col = lv.list.AllColumns[col_idx];
                var col_type = log_view_cell.cell_idx_to_type(col_idx);
                show_column(col, DEFAULT_COL_WIDTH, available.Contains(col_type) && col != lv.viewCol);
            }
            lv.list.RebuildColumns();
        }

        static private void show_column(OLVColumn col, int width, bool show) {
            if (col.Width == 0)
                col.Width = width;
            if (col.is_visible() == show)
                return;

            col.col_width(width);
            col.is_visible(show);
        }

        static private void load_column_positions(log_view lv, string str) {
            if (str == "")
                return;

            // display index -> column index
            Dictionary<int, int > display_indexes = new Dictionary<int, int>();
            foreach ( var pos in str.Split(';'))
                if (pos != "") {
                    string[] infos = pos.Split(',');
                    Debug.Assert(infos.Length == 4);
                    int col_idx = int.Parse(infos[0]);
                    int display_index = int.Parse(infos[1]);
                    int width = int.Parse(infos[2]);
                    bool visible = infos[3] != "0";

                    // this means this column is visible - so we can apply column positioning
                    // (othwerise, this column doesn't even exist for this specific file - nothing to do)
                    lv.list.AllColumns[col_idx].col_width( width);
                    // 1.5.4+ - if we don't have a value in the given column, don't show it
                    if (! lv.available_columns.Contains( log_view_cell.cell_idx_to_type(col_idx)))
                        visible = false;
                    if (!lv.is_full_log && log_view_cell.cell_idx_to_type(col_idx) == info_type.view)
                        // View(s) only visible in Full Log
                        visible = false;
                    lv.list.AllColumns[col_idx].is_visible( visible);
                    if (visible) {
                        while (display_indexes.ContainsKey(display_index))
                            // this can happen when moving from one log to another, and they have very different columns
                            ++display_index;
                        display_indexes.Add(display_index, col_idx);
                    }
                }

            // need to convert display indexes into what can be displayed - you can't have a display index bigger than
            // the number of shown columns
            Dictionary<int,int> index_to_loaded_index = new Dictionary<int, int>();
            int cur_idx = 0;
            foreach ( var loaded_idx in  display_indexes.Select(x => x.Key).OrderBy(x => x) )
                index_to_loaded_index.Add(cur_idx++, loaded_idx);

            foreach (var raw_idx in index_to_loaded_index) {
                int display_index = raw_idx.Key;
                int original_display_index = raw_idx.Value;
                int col_idx = display_indexes[original_display_index];
                var col = lv.list.AllColumns[col_idx];
                col.LastDisplayIndex = display_index;
            }

            lv.list.RebuildColumns();
        }


        static private string default_column_positions_string(log_view lv) {
            string positions = "";
            // column: display index, width, visible
            for (int col_idx = 0; col_idx < lv.list.AllColumns.Count; ++col_idx) {
                var col = lv.list.AllColumns[col_idx];
                int display_index = col.DisplayIndex >= 0 ? col.DisplayIndex : col.LastDisplayIndex;
                if (col.Width > 0)
                    positions += "" + col_idx + "," + display_index + "," + (col == lv.msgCol ? col.Width : DEFAULT_COL_WIDTH)+ "," + "1" + ";";
            }
            return positions;
        }

        static internal string column_positions_as_string(log_view lv) {
            string positions = "";
            // column: display index, width, visible
            for (int col_idx = 0; col_idx < lv.list.AllColumns.Count; ++col_idx) {
                var col = lv.list.AllColumns[col_idx];
                int display_index = col.DisplayIndex >= 0 ? col.DisplayIndex : col.LastDisplayIndex;
                if (col.Width > 0) {
                    // "2" - I don't have this column in my current log - so I don't show it, but, when another log might have this column, do show it
                    // "1" - column is to be shown
                    // "0" - user manually chose to hide this column
                    string visible = lv.available_columns.Contains(log_view_cell.cell_idx_to_type(col_idx)) ? (col.is_visible() ? "1" : "0") : "2";
                    positions += "" + col_idx + "," + display_index + "," + col.col_width() + "," + visible + ";";
                }
            }

            if (positions == default_column_positions_string(lv))
                // user hasn't changed anything
                return "";
            return positions;
        }


    }
}
