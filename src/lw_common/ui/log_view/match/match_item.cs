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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using lw_common.ui.format;
using LogWizard;

namespace lw_common.ui {

    // the reason we derive from filter.match is memory efficiency; otherwise, we would have two lists - one in filter,
    // and one here, full of pointers
    //
    // for large files, this can add up to a LOT of memory
    internal class match_item : filter.match {

        public Color override_bg = util.transparent, override_fg = util.transparent;

        public match_item(BitArray matches, font_info font, line line, int line_idx, log_view parent) : base(matches, font, line, line_idx) {
        }

        public new int line { get { return base.line_idx + 1; }}
        public string date { get { return base.line.part(info_type.date); }}
        public string time { get { return base.line.part(info_type.time); }}
        public string level { get { return base.line.part(info_type.level); }}
        public string msg { get { return base.line.part(info_type.msg); }}

        public string file { get { return base.line.part(info_type.file); }}
        public string func { get { return base.line.part(info_type.func); }}
        public string class_ { get { return base.line.part(info_type.class_); }}
        public string ctx1 { get { return base.line.part(info_type.ctx1); }}
        public string ctx2 { get { return base.line.part(info_type.ctx2); }}
        public string ctx3 { get { return base.line.part(info_type.ctx3); }}

        public string ctx4 { get { return base.line.part(info_type.ctx4); }}
        public string ctx5 { get { return base.line.part(info_type.ctx5); }}
        public string ctx6 { get { return base.line.part(info_type.ctx6); }}
        public string ctx7 { get { return base.line.part(info_type.ctx7); }}
        public string ctx8 { get { return base.line.part(info_type.ctx8); }}
        public string ctx9 { get { return base.line.part(info_type.ctx9); }}
        public string ctx10 { get { return base.line.part(info_type.ctx10); }}
        public string ctx11 { get { return base.line.part(info_type.ctx11); }}
        public string ctx12 { get { return base.line.part(info_type.ctx12); }}
        public string ctx13 { get { return base.line.part(info_type.ctx13); }}
        public string ctx14 { get { return base.line.part(info_type.ctx14); }}
        public string ctx15 { get { return base.line.part(info_type.ctx15); }}
        public string thread { get { return base.line.part(info_type.thread); }}

        public virtual string view { get { return ""; }}

#if old_code
        public Color sel_bg(log_view parent) {
            var bg = this.bg(parent);

            if ( parent.needs_scroll)
                // the idea is that if this view updates a LOT and we're at the last row, 
                // it's disturbing to the eye to constantly have new rows added (thus the former "last" would go up - being marked as "selected",
                // then the selection would change to be the new "last", and so on)
                if (parent.filter.last_change.AddSeconds(3.5) > DateTime.Now)
                    return bg;

            Color dark_bg = util.darker_color(bg);
            Color darker_bg = util.darker_color(dark_bg);
            var focus = win32.focused_ctrl();
            bool is_focused = focus == parent.list || focus == parent.edit;
            bool is_single = parent.lv_parent.is_showing_single_view;
            return is_focused && !is_single ? darker_bg : dark_bg;
        }
#endif


        public virtual Color fg(log_view parent) {
            Color result;

#if old_code
            if (parent.has_bookmark(base.line_idx))
                result = parent.bookmark_fg;
            else 
#endif
            if (override_fg != util.transparent)
                result = override_fg;
            else if (parent.filter.matches.binary_search(line_idx).Item2 < 0)
                result = font_info.full_log_gray.fg;
            else 
                result = font.fg;

            if (result == util.transparent)
                result = app.inst.fg;
            return result;
        }

        public virtual Color bg(log_view parent) {
            Color result;
#if old_code
            if (parent.has_bookmark(base.line_idx))
                result = parent.bookmark_bg;
            else 
#endif
            if (override_bg != util.transparent)
                result = override_bg;
            else 
                result = font.bg;

            // 1.6.6 - if user has searched for something only visible in description pane, show it a bit darker, so the user knows it's a match on this line
            if (result == util.transparent) 
                if (parent.is_searching()) {
                    var view_visible = parent.view_visible_column_types();
                    bool search_matches_view = search_matches_any_column(parent, view_visible);
                    if (!search_matches_view) {
                        var all_visible = parent.all_visible_column_types();
                        var details_visible = all_visible.Where( x => !view_visible.Contains(x) ).ToList();
                        if (search_matches_any_column(parent, details_visible))
                            result = search_form_history.inst.default_search.bg;
                    }
                }
            

            if (result == util.transparent)
                result = app.inst.bg;
            return result;
        }

        private bool search_matches_any_column(log_view parent,  List<info_type> columns) {
            string sel_text = parent.edit.sel_text.ToLower();
            var cur_col = log_view_cell.cell_idx_to_type(parent.cur_col_idx);
            if (sel_text != "") {
                if (app.inst.edit_search_all_columns) {
                    foreach (info_type col in columns) {
                        string txt = log_view_cell.cell_value_by_type(this, col).ToLower();
                        if (txt.Contains(sel_text))
                            return true;
                    }
                } else if (columns.Contains(cur_col)) {
                    string txt = log_view_cell.cell_value_by_type(this, cur_col).ToLower();
                    if (txt.Contains(sel_text))
                        return true;
                }
            } else {
                // search for cur_search
                if (string_search.matches( match, columns.Where(info_type_io.is_searchable) , parent.cur_search))
                    return true;
            }
            return false;            
        }

        private List<text_part> override_print_from_all_places(log_view parent, string text, int col_idx) {
            List<text_part> print = new List<text_part>();

            // 1.2.6 - for now, just for msg do match-color
            if (col_idx == parent.msgCol.fixed_index()) {
                var from_filter = parent.filter.match_indexes(base.line, info_type.msg);
                foreach ( var ff in from_filter)
                    print.Add( new text_part(ff.start, ff.len) { bg = ff.bg, fg = ff.fg, text = new string('?', ff.len), bold = true } );
            }

            string sel = parent.edit.sel_text.ToLower();
            if ((col_idx == parent.sel_col_idx || col_idx == parent.search_found_col_idx) && sel != "") {
                // look for the text typed by the user
                var matches = util.find_all_matches(text.ToLower(), sel);
                if (matches.Count > 0) 
                    foreach ( var match in matches)
                        print.Add( new text_part(match, sel.Length) { bold = true, text = sel, is_typed_search = true } );
            }

            string find = parent.cur_search != null ? parent.cur_search.text : "";
            var col_type = log_view_cell.cell_idx_to_type(col_idx);
            if (parent.cur_search != null && parent.cur_search.is_column_searchable(col_type) && find != "") {
                var matches = string_search.match_indexes(text, parent.cur_search);
                if (matches.Count > 0) {
                    // if we're showing both selected text and the results of a find, differentiate them visually
                    bool italic = sel != "";
                    foreach ( var match in matches)
                        print.Add( new text_part(match.Item1, match.Item2) { text = find, bg = parent.cur_search.bg, fg = parent.cur_search.fg, bold = true, italic = italic, is_find_search = true } );
                }
            }
                
            return print;
        }

        // returns the overrides, sorted by index in the string to print
        public column_formatter_base.format_cell override_print(log_view parent, string text, int col_idx, column_formatter_base.format_cell.location_type location) {
            int row_idx = parent.item_index(this);
            int top_row_idx = parent.top_row_idx;
            string prev_text = "";
            if (row_idx > 0)
                prev_text = log_view_cell.cell_value(parent.item_at(row_idx - 1), col_idx);

            int sel_index = parent.sel_row_idx_ui_thread;
            bool is_bokmark = parent.has_bookmark(line_idx);

            var cell = new column_formatter_base.format_cell(this, parent, col_idx, log_view_cell.cell_idx_to_type(col_idx), new formatted_text(text),
                row_idx, top_row_idx, sel_index, is_bokmark, prev_text, location);
            parent.formatter.format_before(cell);
            var print = override_print_from_all_places(parent, cell.format_text.text, col_idx);
            cell.format_text.add_parts( print);
            parent.formatter.format_after(cell);
            return cell;
        } 

        public filter.match match {
            get { return this;  }
        }
    }
}
