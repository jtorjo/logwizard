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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LogWizard {
    // a row in a filter - a filter can have multiple rows
    //
    // this is the equivalent of a row of a filter in the UI 
    //
    // the filter row itself can have several lines, each of the lines need to yield a positive match, in order for the row to yield a match
    class filter_row {
        protected bool Equals(filter_row other) {
            return same(other) && (enabled == other.enabled) && (dimmed == other.dimmed);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((filter_row) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (items_.GetHashCode() * 397) ^ additions_.GetHashCode();
            }
        }

        public static bool operator ==(filter_row left, filter_row right) {
            return Equals(left, right);
        }

        public static bool operator !=(filter_row left, filter_row right) {
            return !Equals(left, right);
        }

        // returns true if it's equal to the other row, regardless of being enabled or not
        //
        // if it is, we can keep the cached information (about line matches)
        public bool same(filter_row other) {
            return Enumerable.SequenceEqual(items_, other.items_) && Enumerable.SequenceEqual(additions_, other.additions_);            
        }

        private List<filter_line> items_ = new List<filter_line>();
        private List<addition> additions_ = new List<addition>();

        private HashSet<int> line_matches_ = new HashSet<int>();
        // cached - so that what we computed once, we don't ask again
        private log_line_reader old_line_matches_log_ = null;
        private int old_line_count_ = 0;


        private filter_line.font_info font_ = null;

        public filter_row(string text) {
            List<filter_line> lines = new List<filter_line>();
            List<addition> additions = new List<addition>();
            foreach ( string line in text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                filter_line item = filter_line.parse(line);
                if ( item != null)
                    lines.Add(item);
                addition add = addition.parse(line);
                if ( add != null)
                    additions.Add(add);
            }
            init(lines, additions);
        }

        public bool is_valid {
            get { return items_.Count > 0; }
        } 

        public class match {
            public filter_line.font_info font = null;
        }

        public HashSet<int> line_matches {
            get { return line_matches_; }
        }

        public List<addition> additions {
            get { return additions_; }
        }

        private filter_line.font_info font() {
            var result = new filter_line.font_info();
            foreach ( var item in items_)
                if (item.part == filter_line.part_type.font)
                    result.copy_from( item.fi);

            result.bg = get_bg_color(result.bg, enabled);
            result.fg = get_fg_color(result.fg, enabled);
            return result;
        }

        // if !enabled && dimmed:  - dim this filter_row compared to the rest
        private bool enabled_ = true;
        public bool enabled {
            get { return enabled_; }
            set { enabled_ = value;
                font_ = font();
            }
        }

        private bool dimmed_ = false;
        public bool dimmed {
            get { return dimmed_; }
            set {
                dimmed_ = value; 
                font_ = font();                
            }
        }

        private void init(List<filter_line> items, List<addition> additions) {
            items_ = items;
            additions_ = additions;
            font_ = font();
            update_case_sensitive();
        }

        private void update_case_sensitive() {
            bool is_sensitive = true;
            foreach (filter_line line in items_)
                if (!line.case_sensitive)
                    is_sensitive = false;
            items_ = items_.Where(i => i.part != filter_line.part_type.case_sensitive_info).ToList();
            if ( !is_sensitive)
                foreach (filter_line line in items_)
                    line.case_sensitive = false;
        }

        public void refresh() {
            lock (this) {
                line_matches_.Clear();
                old_line_count_ = 0;                
            }
        }

        // computes the line matches - does not care about colors or the additions - just to know which lines actually match
        public void compute_line_matches(log_line_reader log) {
            log.refresh();
            if (old_line_matches_log_ != log) {
                old_line_matches_log_ = log;
                line_matches_.Clear();
                old_line_count_ = 0;
            }

            // note: in order to match, all lines must match
            int new_line_count = log.line_count;
            for (int i = old_line_count_; i < new_line_count; ++i) {
                bool matches = true;
                foreach (filter_line fi in items_)
                    if ( fi.part != filter_line.part_type.font)
                        if ( !fi.matches(log.line_at(i))) {
                            matches = false;
                            break;
                        }
                if ( matches)
                    line_matches_.Add(i);
            }
            // if we have at least one line - we'll recheck this last line next time - just in case we did not fully read it last time
            old_line_count_ = new_line_count > 0 ? new_line_count -1 : new_line_count;
        }

        private Color get_bg_color(Color c, bool enabled) {
            if (enabled)
                return c != util.transparent ? c : Color.White;
            else
                return Color.White;
        }
        private Color get_fg_color(Color c, bool enabled) {
            if (enabled)
                return c != util.transparent ? c : Color.Black;
            else
                return Color.LightGray;
        }

        // returns exactly how we match this line
        public match get_match(int line_idx) {
            Debug.Assert(line_matches_.Contains(line_idx));
            Debug.Assert(old_line_matches_log_ != null);

            return new match { font = font_ };
        }
    }
}
