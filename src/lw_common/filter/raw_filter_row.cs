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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using LogWizard;

namespace lw_common {
    // a row in a filter - a filter can have multiple rows
    //
    // this is the equivalent of a row of a filter in the UI 
    //
    // the filter row itself can have several lines, each of the lines need to yield a positive match, in order for the row to yield a match
    public class raw_filter_row {

        public const string FILTER_ID_PREFIX = "# // ";

        protected bool Equals(raw_filter_row other) {
            return same(other) && (enabled == other.enabled) && (dimmed == other.dimmed);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((raw_filter_row) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (items_.GetHashCode() * 397) ^ additions_.GetHashCode();
            }
        }

        public static bool operator ==(raw_filter_row left, raw_filter_row right) {
            return Equals(left, right);
        }

        public static bool operator !=(raw_filter_row left, raw_filter_row right) {
            return !Equals(left, right);
        }

        // returns true if it's equal to the other row, regardless of being enabled or not
        //
        // if it is, we can keep the cached information (about line matches)
        public bool same(raw_filter_row other) {
            //return Enumerable.SequenceEqual(items_, other.items_) && Enumerable.SequenceEqual(additions_, other.additions_) && apply_to_existing_lines == other.apply_to_existing_lines;
            return unique_id == other.unique_id && apply_to_existing_lines == other.apply_to_existing_lines &&
                   font_ == other.font_;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // UNIQUE DATA 
        //
        // uniquely identifies the filter - any change to the members below, will need to re-run the filter


        protected List<filter_line> items_ = new List<filter_line>();
        protected List<addition> additions_ = new List<addition>();

        protected readonly font_info font_ = null;

        // if true, this is applied after the normal filters
        //
        // it can : filter out lines from what the normal filters yielded and/or
        //          give a different color to the lines
        public readonly bool apply_to_existing_lines = false;

        protected readonly string unique_id_;

        // 1.2.20+ useful to find a filter by id
        private readonly string[] lines_;
        ////////////////////////////////////////////////////////////////////////////////////////////////
        // CHANGEABLE DATA 
        //
        // this can change without affeacting the cached data
        private bool valid_ = true;
        
        private bool enabled_ = true;
        private bool dimmed_ = false;

        public override string ToString() {
            return font_.ToString() + "; " + unique_id_;
        }

        // returns a string that **** uniqueyly identifies **** the UNIQUE data of the filter
        //
        // in other words, if two filters' unique_id are equal, they are the ***same*** filter (except for enabled, dimmed, font information)
        public string unique_id {
            get {
                return unique_id_;
            }
        }

        // 1.2.20+ - easy way to uniquely identify a filter within a view
        public string filter_id {
            get {
                foreach ( string line in lines_)
                    if (line.StartsWith(FILTER_ID_PREFIX))
                        return line;
                return "";
            }
        }

        public string color_line {
            get {
                var l = lines_.FirstOrDefault(x => x.Trim().StartsWith("color"));
                return l ?? "";
            }
        }
        public string match_color_line {
            get {
                var l = lines_.FirstOrDefault(x => x.Trim().StartsWith("match_color"));
                return l ?? "";
            }
        }

        public static string merge_lines(string old_text, string new_text) {
            // note : we don't care about apply-to-existing-lines - we know it's the same amongst old/new filter row
            //        what I care about is the colors (line color and merge color)
            bool apply_to_existing_lines = false;
            var old_row = new raw_filter_row(old_text, apply_to_existing_lines);
            var new_row = new raw_filter_row(new_text, apply_to_existing_lines);

            // old lines - not trimmed (so that we can insert them as they were - if needed); new lines -> trimmed
            var old_lines = old_row.lines_.Where(x => !filter_line.is_color_or_font_line(x) && !x.Trim().StartsWith("#") && x.Trim() != "" ).ToList();
            var new_lines = new_row.lines_.Where(x => !filter_line.is_color_or_font_line(x) && !x.Trim().StartsWith("#") && x.Trim() != "" ).Select(x => x.Trim()).ToList();

            foreach ( string line in old_lines)
                if ( !new_lines.Contains( line.Trim()))
                    util.append_line(ref new_text, line);

            if (new_row.color_line == "" && old_row.color_line != "") 
                // preserve old color
                util.append_line(ref new_text, old_row.color_line);

            if ( new_row.match_color_line == "" && old_row.match_color_line != "")
                util.append_line(ref new_text, old_row.match_color_line);

            return new_text;

        }

        public raw_filter_row(raw_filter_row other) {
            items_ = other.items_.ToList();
            lines_ = other.lines_.ToArray();
            additions_ = other.additions.ToList();
            apply_to_existing_lines = other.apply_to_existing_lines;
            unique_id_ = other.unique_id_;
            valid_ = other.valid_;
            enabled_ = other.enabled;
            dimmed_ = other.dimmed_;

            font_ = font_info.default_font_copy;
            update_font();
        }

        public raw_filter_row(string text, bool apply_to_existing_lines) {
            this.apply_to_existing_lines = apply_to_existing_lines;
            List<filter_line> lines = new List<filter_line>();
            List<addition> additions = new List<addition>();
            lines_ = text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach ( string line in lines_) {
                filter_line item = filter_line.parse(line);
                if ( item != null)
                    lines.Add(item);
                addition add = addition.parse(line);
                if ( add != null)
                    additions.Add(add);

                bool is_comment = line.StartsWith("#"), is_empty = line.Trim() == "";
                if (!is_comment && !is_empty && !filter_line.is_color_or_font_line(line))
                    unique_id_ += line.Trim() + "\r\n";

                if (item == null && add == null) {
                    if (!is_comment && !is_empty)
                        // in this case, the line is not valid yet
                        valid_ = false;
                }
            }
            unique_id_ += "" + apply_to_existing_lines;
            font_ = font_info.default_font_copy;
            init(lines, additions);

            if (items_.Count < 1)
                valid_ = false;
        }

        public bool is_valid {
            get { return valid_; }
        } 

        public class match {
            public font_info font = null;
        }


        public List<addition> additions {
            get { return additions_; }
        }

        public Color match_fg {
            get { return font_.match_fg; }
        }
        public Color match_bg {
            get { return font_.match_bg; }
        }
        public Color fg {
            get { return font_.fg; }
        }
        public Color bg {
            get { return font_.bg; }
        }

        private font_info get_raw_font_info() {
            var result = new font_info();
            foreach ( var item in items_)
                if (item.part == part_type.font) {
                    if (item.fi.fg != util.transparent || item.fi.bg != util.transparent) {
                        result.fg = item.fi.fg;
                        result.bg = item.fi.bg;
                    }
                    else if (item.fi.match_fg != util.transparent || item.fi.match_bg != util.transparent) {
                        result.match_fg = item.fi.match_fg;
                        result.match_bg = item.fi.match_bg;
                    }
                }

            return result;
        }

        private font_info get_font_info() {
            var result = get_raw_font_info();
            result.bg = get_bg_color(result.bg, enabled);
            result.fg = get_fg_color(result.fg, enabled);
            return result;
        }

        // if !enabled && dimmed:  - dim this raw_filter_row compared to the rest
        public bool enabled {
            get { return enabled_; }
            set { enabled_ = value;
                update_font();
            }
        }

        public bool dimmed {
            get { return dimmed_; }
            set {
                dimmed_ = value; 
                update_font();
            }
        }

        public bool has_font_info {
            get {
                bool has_font_info = items_.Count(i => i.part == part_type.font) > 0;
                return has_font_info;
            }
        }

        private void init(List<filter_line> items, List<addition> additions) {
            items_ = items;
            additions_ = additions;

            update_font();
            update_case_sensitive();
        }

        private void update_font() {
            // 1.0.91+ - make sure we preserve the same font, since any match matching this filter will point to this font
            //           if the font gets updated, we want all matches to point to the existing font            
            var new_font = get_font_info();
            font_.copy_from( new_font);
        }

        private void update_case_sensitive() {
            bool is_sensitive = true;
            foreach (filter_line line in items_)
                if (!line.case_sensitive)
                    is_sensitive = false;
            items_ = items_.Where(i => i.part != part_type.case_sensitive_info).ToList();
            if ( !is_sensitive)
                foreach (filter_line line in items_)
                    line.case_sensitive = false;
        }

        private Color get_fg_color(Color c, bool enabled) {
            return enabled ? c : app.inst.dimmed_fg;
        }

        private Color get_bg_color(Color c, bool enabled) {
            return enabled ? c : app.inst.dimmed_bg;
        }

        public void preserve_cache_copy(raw_filter_row from) {
            enabled_ = from.enabled;
            dimmed_ = from.dimmed;
            // 1.0.91+ this is very important - in case only the font has changed (the rest is the same), we don't want a full refresh
            //         we will use the new font though
            font_.copy_from( from.font_);
        }

        private List<filter_line.match_index> empty_ = new List<filter_line.match_index>();
        public List<filter_line.match_index> match_indexes(line l, info_type type) {
            List<filter_line.match_index> indexes = null;
            bool has_match_color = font_.match_fg != util.transparent || font_.match_bg != util.transparent;
            if ( has_match_color)
                foreach (filter_line line in items_) {
                    var now = line.match_indexes(l, type);
                    if (now.Count > 0) {
                        if ( indexes == null)
                            indexes = new List<filter_line.match_index>();
                        // here, we need to set the match colors
                        foreach (var index in now) {
                            index.fg = font_.match_fg;
                            index.bg = font_.match_bg;
                        }
                        indexes.AddRange(now);
                    }
                }
            return indexes ?? empty_;
        } 

    }
}
