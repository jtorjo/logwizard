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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LogWizard;

namespace lw_common.ui {
    public partial class search_form : Form {

        private const int MAX_PREVIEW_ROWS = 1000;

        public class search_for {
            public bool case_sensitive = false;
            public bool full_word = false;
            public string text = "";
            
            public bool use_regex = false;
            public Regex regex = null;

            public string friendly_name {
                get {
                    List<string> attr = new List<string>();
                    if ( !case_sensitive)
                        attr.Add("case-insensitive");
                    if ( full_word)
                        attr.Add("whole word");
                    var extra = util.concatenate(attr, ",");

                    if (!use_regex)
                        return text + (extra != "" ? " (" + extra + ")" : "");

                    return "Regex " + regex.ToString() + (extra != "" ? " (" + extra + ")" : "");
                }
            }

            public Color fg = util.transparent, bg = util.transparent;
            public bool mark_lines_with_color = false;
        }
        private search_for search_ = new search_for();
        private settings_file sett;

        private List< List<string>> preview_items_ = new List<List<string>>();
        // contains the rows that match
        private List<int> matches_ = new List<int>(); 

        private class item {
            public List<string> parts = new List<string>();

            public item(List<string> parts) {
                this.parts = parts;
            }

            public string part(int i) {
                return parts.Count > i ? parts[i] : "";
            }

            public string t0 { get { return part(0); }}
            public string t1 { get { return part(1); }}
            public string t2 { get { return part(2); }}
            public string t3 { get { return part(3); }}
            public string t4 { get { return part(4); }}
            public string t5 { get { return part(5); }}
            public string t6 { get { return part(6); }}
            public string t7 { get { return part(7); }}
            public string t8 { get { return part(8); }}
            public string t9 { get { return part(9); }}

            public string t10 { get { return part(10); }}
            public string t11 { get { return part(11); }}
            public string t12 { get { return part(12); }}
            public string t13 { get { return part(13); }}
            public string t14 { get { return part(14); }}
            public string t15 { get { return part(15); }}
            public string t16 { get { return part(16); }}
            public string t17 { get { return part(17); }}
            public string t18 { get { return part(18); }}
            public string t19 { get { return part(19); }}

            public string t20 { get { return part(20); }}
            public string t21 { get { return part(21); }}
            public string t22 { get { return part(22); }}
            public string t23 { get { return part(23); }}
            public string t24 { get { return part(24); }}
            public string t25 { get { return part(25); }}
            public string t26 { get { return part(26); }}
            public string t27 { get { return part(27); }}
            public string t28 { get { return part(28); }}
            public string t29 { get { return part(29); }}

            public string t30 { get { return part(30); }}
            public string t31 { get { return part(31); }}
            public string t32 { get { return part(32); }}
            public string t33 { get { return part(33); }}
            public string t34 { get { return part(34); }}
            public string t35 { get { return part(35); }}
            public string t36 { get { return part(36); }}
            public string t37 { get { return part(37); }}
            public string t38 { get { return part(38); }}
            public string t39 { get { return part(39); }}

            public string t40 { get { return part(40); }}
            public string t41 { get { return part(41); }}
            public string t42 { get { return part(42); }}
            public string t43 { get { return part(43); }}
            public string t44 { get { return part(44); }}
            public string t45 { get { return part(45); }}
            public string t46 { get { return part(46); }}
            public string t47 { get { return part(47); }}
            public string t48 { get { return part(48); }}
            public string t49 { get { return part(49); }}

            public string t50 { get { return part(50); }}
        }

        // 1.2.7+ if there's something selected by the user, override what we had
        public search_form(Form parent, log_view lv, string smart_edit_search_for_text) {
            this.sett = app.inst.sett;
            InitializeComponent();
            fg.BackColor = util.str_to_color( sett.get("search_fg", "transparent"));
            bg.BackColor = util.str_to_color( sett.get("search_bg", "#faebd7") ); // antiquewhite

            caseSensitive.Checked = sett.get("search_case_sensitive", "0") != "0";
            fullWord.Checked = sett.get("search_full_word", "0") != "0";
            int type = int.Parse(sett.get("search_type", "0"));
            switch (type) {
            case 0:
                radioAutoRecognize.Checked = true;
                break;
            case 1:
                radioText.Checked = true;
                break;
            case 2:
                radioRegex.Checked = true;
                break;
                default: Debug.Assert(false);
                break;
            }

            txt.Text = sett.get("search_text");

            if (smart_edit_search_for_text != "") {
                txt.Text = smart_edit_search_for_text;
                radioText.Checked = true;
            }

            update_autorecognize_radio();
            TopMost = parent.TopMost;

            result.Font = lv.list.Font;
            load_surrounding_rows(lv);
            update_preview_text();
        }

        private void load_surrounding_rows(log_view lv) {
            int sel = lv.sel_row_idx;
            if (sel < 0)
                sel = 0;
            // get as many rows as possible, in both directions
            int max_count = Math.Min(MAX_PREVIEW_ROWS, lv.item_count);
            int min = sel - max_count / 2, max = sel + max_count / 2;
            if (min < 0) {
                max += -min;
                min = 0;
            }
            if (max > lv.item_count) {
                min -= max - lv.item_count;
                max = lv.item_count;
            }
            if (min < 0)
                min = 0;
            if (max > lv.item_count)
                max = lv.item_count;
            // at this point, we know the start and end

            // see which columns actuall have useful data
            List< Tuple<int,int>> columns_and_displayidx = new List<Tuple<int,int>>();
            for (int col_idx = 0; col_idx < lv.list.AllColumns.Count; ++col_idx) {
                if ( lv.list.AllColumns[col_idx].Width > 0)
                    columns_and_displayidx.Add( new Tuple<int, int>(col_idx, lv.list.AllColumns[col_idx].DisplayIndex ));
            }
            var columns = columns_and_displayidx.OrderBy(x => x.Item2).Select(x => x.Item1).ToList();

            for (int idx = min; idx < max; ++idx) {
                var i = lv.item_at(idx);
                List<string> row = new List<string>();
                foreach (int col_idx in columns)
                    row.Add(log_view_cell.cell_value(i, col_idx));
                preview_items_.Add(row);
            }

            int preview_col_idx = 0;
            foreach (int col_idx in columns) {
                result.AllColumns[preview_col_idx].Width = lv.list.AllColumns[col_idx].Width;
                result.AllColumns[preview_col_idx].Text = lv.list.AllColumns[col_idx] != lv.msgCol ? lv.list.AllColumns[col_idx].Text : "Message";
                result.AllColumns[preview_col_idx].FillsFreeSpace = lv.list.AllColumns[col_idx].FillsFreeSpace;
                ++preview_col_idx;
            }
            for (; preview_col_idx < result.AllColumns.Count; ++preview_col_idx)
                result.AllColumns[preview_col_idx].IsVisible = false;
            result.RebuildColumns();

            for ( int match_idx = 0; match_idx < preview_items_.Count; ++match_idx)
                matches_.Add(match_idx);
            rebuild_result();
        }

        private void rebuild_result() {
            result.Freeze();
            result.ClearObjects();
            foreach (var match_idx in matches_) 
                result.AddObject( new item( preview_items_[match_idx] ) );
            result.Unfreeze();
        }

        public static search_for default_search {
            get {
                var sett = app.inst.sett;

                int type = int.Parse(sett.get("search_type", "0"));
                switch (type) {
                case 0: // auto
                    break;
                case 1: // text
                    break;
                case 2: // regex
                    break;
                    default: Debug.Assert(false);
                    break;
                }
                string text = sett.get("search_text");
                bool use_regex = type == 2;
                if (type == 0)
                    use_regex = is_auto_regex(text);
                Regex regex = null;
                if ( use_regex)
                    try {
                        regex = new Regex(text, RegexOptions.Singleline);
                    } catch {
                        regex = null;
                    }

                search_for default_ = new search_for {
                    fg = util.str_to_color( sett.get("search_fg", "transparent")),
                    bg = util.str_to_color( sett.get("search_bg", "#faebd7") ),
                    case_sensitive = sett.get("search_case_sensitive", "0") != "0",
                    full_word = sett.get("search_full_word", "0") != "0",
                    use_regex = use_regex, 
                    regex = regex, 
                    text = text, 
                    mark_lines_with_color = sett.get("search_mark_lines_with_color", "1") != "0"
                };
                return default_;
            }
        }

        public search_for search {
            get { return search_; }
        }

        private void fg_Click(object sender, EventArgs e) {
            var color = util.select_color_via_dlg();
            if (color.ToArgb() != util.transparent.ToArgb())
                fg.BackColor = color;
        }

        private void bg_Click(object sender, EventArgs e) {
            var color = util.select_color_via_dlg();
            if (color.ToArgb() != util.transparent.ToArgb())
                bg.BackColor = color;
        }

        private void ok_Click(object sender, EventArgs e) {
            if (txt.Text != "") {
                sett.set("search_bg", util.color_to_str(bg.BackColor));
                sett.set("search_fg", util.color_to_str(fg.BackColor));
                sett.set("search_case_sensitive", caseSensitive.Checked ? "1" : "0");
                sett.set("search_full_word", fullWord.Checked ? "1" : "0");
                sett.set("search_mark_lines_with_color", mark.Checked ? "1" : "0");
                sett.set("search_text", txt.Text);

                int type = 0;
                if (radioAutoRecognize.Checked) type = 0;
                else if (radioText.Checked) type = 1;
                else if (radioRegex.Checked) type = 2;
                else Debug.Assert(false);

                sett.set("search_type", "" + type);
                sett.save();
                search_ = default_search;
                DialogResult = DialogResult.OK;
            }
        }

        private void cancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void txt_TextChanged(object sender, EventArgs e) {
            update_autorecognize_radio();
        }

        private static bool is_auto_regex(string text) {
            bool is_regex = text.IndexOfAny(new char[] {'[', ']', '(', ')', '\\'}) >= 0;
            return is_regex;
        }

        private void update_autorecognize_radio() {
            radioAutoRecognize.Text = "Auto recognized (" + (is_auto_regex(txt.Text) ? "Regex" : "Text") + ")";
        }

        private void update_preview_text() {
            preview.Text = "Previewing surrounding " + preview_items_.Count + " rows.";

        }

        private void checkResults_Tick(object sender, EventArgs e) {

        }
    }
}
