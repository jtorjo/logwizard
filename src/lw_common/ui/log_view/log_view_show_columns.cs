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

        private const int MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS = 10;
        private const int DEFAULT_COL_WIDTH = 80;

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

            int value_count = 0;
            for (int idx = 0; idx < max_rows_to_check; ++idx) {
                var i = lv.item_at(idx) ;
                if (i.line_idx < 0)
                    continue;
                if (log_view_cell.cell_value_by_type(i, type) != "")
                    ++value_count;
            }
            bool has_values = (value_count > 0);
            return has_values;
        }

        static public void refresh_visible_columns(List<log_view> all_views, log_view full_log) {
            Debug.Assert(full_log.is_full_log);
            if (refresh_visible_columns_any_change(full_log)) {
                // notify all other views
                foreach (log_view lv in all_views) {
                    lv.visible_columns = full_log.visible_columns;
                    refresh_visible_columns(lv, full_log);
                }

                if ( full_log.visible_columns_refreshed_ >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
                    foreach (log_view lv in all_views)
                        // in tihs case, we consider knowing the visible-columns for ALL views
                        lv.visible_columns_refreshed_ = MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS;

                // now, apply the saved-custom-positions, if any
                foreach (log_view lv in all_views)
                    apply_column_positions(lv);
                apply_column_positions(full_log);

                full_log.update_column_names();
                foreach (log_view lv in all_views)
                    lv.update_column_names();
            }
        }

        static internal void apply_column_positions(log_view lv) {
            if ( lv.column_positions != "")
                load_column_positions(lv, lv.column_positions);
        }

        static private bool refresh_visible_columns_any_change(log_view full_log) {
            Debug.Assert(full_log.is_full_log);
            if (full_log.visible_columns_refreshed_ >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
                return false;

            int count = full_log.item_count;
            bool needs_refresh = count != full_log.visible_columns_refreshed_;
            if (needs_refresh) {
                List<info_type> visible_columns = new List<info_type>();
                int row_count = Math.Min(count, MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS);
                foreach (info_type type in Enum.GetValues(typeof (info_type))) {
                    if (type == info_type.max)
                        continue;
                    bool is_visible = has_value_at_column(full_log, type, row_count);
                    show_column(full_log, log_view_cell.column(full_log,type), DEFAULT_COL_WIDTH, is_visible);
                    if ( is_visible)
                        visible_columns.Add(type);                    
                }

                full_log.visible_columns_refreshed_ = count;
                full_log.visible_columns = visible_columns;
                logger.Debug("visible columns (" + row_count + ") - " + util.concatenate(visible_columns, ", "));
                
                full_log.list.RebuildColumns();
            }

            if (count >= MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS) 
                full_log.visible_columns_refreshed_ = MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS;
            return needs_refresh;
        }

        static private void refresh_visible_columns(log_view lv, log_view full_log) {
            Debug.Assert(!lv.is_full_log);

            for (int idx = 0; idx < lv.list.AllColumns.Count; ++idx) {
                var col = lv.list.AllColumns[idx];
                if (col != lv.viewCol) 
                    show_column(lv, col, full_log.list.AllColumns[idx].Width, full_log.list.AllColumns[idx].IsVisible);
                else 
                    show_column(lv, col, DEFAULT_COL_WIDTH, false);
            }
            lv.list.RebuildColumns();
        }

        static private void show_column(log_view lv, OLVColumn col, int width, bool show) {
            //if( lv.is_full_log)
              //  logger.Debug("showing column " + col.Text + " w=" + width +" visible=" + show);
            if (col.Width == 0)
                col.Width = width;
            if (col.IsVisible == show)
                return;

            col.Width = width;
            col.IsVisible = show;
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
                    lv.list.AllColumns[col_idx].Width = width;
                    // 1.5.4+ - if we don't have a value in the given column, don't show it
                    if (! lv.visible_columns.Contains( log_view_cell.cell_idx_to_type(col_idx)))
                        visible = false;
                    if (!lv.is_full_log && log_view_cell.cell_idx_to_type(col_idx) == info_type.view)
                        // View(s) only visible in Full Log
                        visible = false;
                    lv.list.AllColumns[col_idx].IsVisible = visible;
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

        static internal string save_column_positions(log_view lv) {
            if (lv.visible_columns_refreshed_ < MIN_ROWS_FOR_COMPUTE_VISIBLE_COLUMNS)
                // in this case, we're not sure what columns are visible for sure or not, so don't do anything
                return "";

            string positions = "";
            // column: display index, width, visible
            for (int col_idx = 0; col_idx < lv.list.AllColumns.Count; ++col_idx) {
                var col = lv.list.AllColumns[col_idx];
                int display_index = col.DisplayIndex >= 0 ? col.DisplayIndex : col.LastDisplayIndex;
                if (col.Width > 0) {
                    // "2" - I don't have this column in my current log - so I don't show it, but, when another log might have this column, do show it
                    // "1" - column is to be shown
                    // "0" - user manually chose to hide this column
                    string visible = lv.visible_columns.Contains(log_view_cell.cell_idx_to_type(col_idx)) ? (col.IsVisible ? "1" : "0") : "2";
                    positions += "" + col_idx + "," + display_index + "," + col.Width + "," + visible + ";";
                }
            }

            if (positions == default_column_positions_string(lv))
                // user hasn't changed anything
                return "";
            return positions;
        }


    }
}
