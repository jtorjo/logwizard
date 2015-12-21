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
using LogWizard;

namespace lw_common.ui {

    // the reason we derive from filter.match is memory efficiency; otherwise, we would have two lists - one in filter,
    // and one here, full of pointers
    //
    // for large files, this can add up to a LOT of memory
    internal class match_item : filter.match {

        public Color override_bg = util.transparent, override_fg = util.transparent;

        public match_item(BitArray matches, font_info font, line line, int lineIdx, log_view parent) : base(matches, font, line, lineIdx) {
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
            return is_focused ? darker_bg : dark_bg;
        }


        public virtual Color fg(log_view parent) {
            Color result;

            if (parent.has_bookmark(base.line_idx))
                result = parent.bookmark_fg;
            else if (override_fg != util.transparent)
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
            if (parent.has_bookmark(base.line_idx))
                result = parent.bookmark_bg;
            else if (override_bg != util.transparent)
                result = override_bg;
            else 
                result = font.bg;

            if (result == util.transparent)
                result = app.inst.bg;
            return result;
        }

        private List<Tuple<int, int, print_info>> override_print_from_all_places(log_view parent, string text, int col_idx) {
            List<Tuple<int, int, print_info>> print = new List<Tuple<int, int, print_info>>();

            // 1.2.6 - for now, just for msg do match-color
            if (col_idx == parent.msgCol.fixed_index()) {
                var from_filter = parent.filter.match_indexes(base.line, info_type.msg);
                foreach ( var ff in from_filter)
                    print.Add( new Tuple<int, int, print_info>( ff.start, ff.len, new print_info {
                        bg = ff.bg, fg = ff.fg, text = new string('?', ff.len), bold = true
                    } ));
            }

            string sel = parent.edit.sel_text.ToLower();
            if ((col_idx == parent.sel_col_idx || col_idx == parent.search_found_col_idx) && sel != "") {
                // look for the text typed by the user
                var matches = util.find_all_matches(text.ToLower(), sel);
                if (matches.Count > 0) {
                    print_info print_sel = new print_info { bold = true, text = sel, is_typed_search = true };
                    foreach ( var match in matches)
                        print.Add( new Tuple<int, int, print_info>(match, sel.Length, print_sel));
                }
            }

            string find = parent.cur_search != null ? parent.cur_search.text : "";
            if (col_idx == parent.msgCol.fixed_index() && find != "") {
                var matches = string_search.match_indexes(text, parent.cur_search);
                if (matches.Count > 0) {
                    // if we're showing both selected text and the results of a find, differentiate them visually
                    bool italic = sel != "";
                    print_info print_sel = new print_info { text = find, bg = parent.cur_search.bg, fg = parent.cur_search.fg, bold = true, italic = italic, is_typed_search = italic };
                    foreach ( var match in matches)
                        print.Add( new Tuple<int, int, print_info>(match.Item1, match.Item2, print_sel));
                }                    
            }
                
            if ( util.is_debug)
                foreach ( var p in print)
                    Debug.Assert(p.Item1 >= 0 && p.Item2 >= 0);

            return print;
        }

        public List<Tuple<int, int, print_info>> override_print(log_view parent, string text, info_type type) {
            return override_print(parent, text, log_view_cell.info_type_to_cell_idx(type));
        }

        // returns the overrides, sorted by index in the string to print
        public List<Tuple<int, int, print_info>> override_print(log_view parent, string text, int col_idx) {
            var print = override_print_from_all_places(parent, text, col_idx);

            // for testing only
            // var old_print = util.is_debug ? print.ToList() : null;

            // check for collitions
            bool collitions_found = true;
            while (collitions_found) {
                // sort it
                // note: I need to sort it after each collision is solved, since in imbricated prints, we can get un-sorted
                print.Sort((x, y) => {
                    if (x.Item1 != y.Item1)
                        return x.Item1 - y.Item1;
                    // if two items at same index - first will be the one with larger len
                    return - (x.Item2 - y.Item2);
                });

                collitions_found = false;
                for (int idx = 0; !collitions_found && idx < print.Count - 1; ++idx) {
                    var now = print[idx];
                    var next = print[idx + 1];

                    // special case - we split something into 3, but one of the parts was empty
                    if (now.Item2 == 0) {
                        print.RemoveAt(idx);
                        collitions_found = true;
                        continue;
                    }
                    if (next.Item2 == 0) {
                        print.RemoveAt(idx + 1);
                        collitions_found = true;
                        continue;
                    }

                    if (now.Item1 + now.Item2 > next.Item1)
                        collitions_found = true;

                    if (collitions_found) {
                        // first, see what type of collision it is
                        bool exactly_same = now.Item1 == next.Item1 && now.Item2 == next.Item2;
                        if (exactly_same)
                            // doesn't matter - just keep one
                            print.RemoveAt(idx + 1);
                        else {
                            // here - either one completely contains the other, or they just intersect
                            bool contains_fully = now.Item1 + now.Item2 >= next.Item1 + next.Item2;
                            if (contains_fully) {
                                bool starts_at_same_idx = now.Item1 == next.Item1;
                                if (starts_at_same_idx) {
                                    print[idx] = next;
                                    int len = next.Item2;
                                    int second_len = now.Item2 - len;
                                    Debug.Assert(second_len >= 0);
                                    print[idx + 1] = new Tuple<int, int, print_info>(now.Item1 + len, second_len, now.Item3);
                                } else {
                                    // in this case, we need to split in 3
                                    int len1 = next.Item1 - now.Item1;
                                    int len2 = now.Item2 - len1 - next.Item2;
                                    var now1 = new Tuple<int, int, print_info>(now.Item1, len1, now.Item3);
                                    var now2 = new Tuple<int, int, print_info>(next.Item1 + next.Item2, len2, now.Item3);
                                    Debug.Assert(len1 >= 0 && len2 >= 0);
                                    print[idx] = now1;
                                    print.Insert(idx + 2, now2);
                                }
                            } else {
                                // they just intersect
                                int intersect_count = now.Item1 + now.Item2 - next.Item1;
                                Debug.Assert( intersect_count > 0);
                                int interesect_len = now.Item2 - intersect_count;
                                Debug.Assert(interesect_len >= 0);
                                now = new Tuple<int, int, print_info>(now.Item1, interesect_len, now.Item3);
                                print[idx] = now;
                            }
                        }
                    }
                }
            }

            if ( util.is_debug)
                foreach ( var p in print)
                    Debug.Assert(p.Item1 >= 0 && p.Item2 >= 0);

            return print;
        } 

        public filter.match match {
            get { return this;  }
        }
    }
}
