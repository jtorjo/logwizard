using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lw_common.ui.format {
    // contains all the text parts - formatted
    class formatted_text {
        private List<text_part> parts_ = new List<text_part>();
        private readonly string text_;

        // 1.7.7 not used yet
        private HorizontalAlignment align_ = HorizontalAlignment.Left;

        public formatted_text(string text) {
            text_ = text;
        }

        public string text {
            get { return text_; }
        }

        public void replace_text(int start, int len, string new_text) {
            
        }

        public void add_parts(List<text_part> parts) {
            if (parts.Count < 1)
                return;

            foreach ( var p in parts)
                Debug.Assert(p.start >= 0 && p.len > 0);

            parts_.AddRange(parts);
            
            // check for collitions
            bool collitions_found = true;
            while (collitions_found) {
                // sort it
                // note: I need to sort it after each collision is solved, since in imbricated prints, we can get un-sorted
                parts_.Sort((x, y) => {
                    if (x.start != y.start)
                        return x.start - y.start;
                    // if two items at same index - first will be the one with larger len
                    return - (x.len - y.len);
                });

                collitions_found = false;
                for (int idx = 0; !collitions_found && idx < parts_.Count - 1; ++idx) {
                    var now = parts_[idx];
                    var next = parts_[idx + 1];

                    // special case - we split something into 3, but one of the parts was empty
                    if (now.len == 0) {
                        parts_.RemoveAt(idx);
                        collitions_found = true;
                        continue;
                    }
                    if (next.len == 0) {
                        parts_.RemoveAt(idx + 1);
                        collitions_found = true;
                        continue;
                    }

                    if (now.start + now.len > next.start)
                        collitions_found = true;

                    if (collitions_found) {
                        // first, see what type of collision it is
                        bool exactly_same = now.start == next.start && now.len == next.len;
                        if (exactly_same) {
                            // doesn't matter - just keep one - 1.7.6+ the latter overrides all settings
                            parts_[idx] = now.merge_copy(next);
                            parts_.RemoveAt(idx + 1);
                        } else {
                            // here - either one completely contains the other, or they just intersect
                            bool contains_fully = now.start + now.len >= next.start + next.len;
                            if (contains_fully) {
                                bool starts_at_same_idx = now.start == next.start;
                                if (starts_at_same_idx) {
                                    parts_[idx] = next;
                                    int len = next.len;
                                    int second_len = now.len - len;
                                    Debug.Assert(second_len >= 0);
                                    parts_[idx + 1] = new text_part(now.start + len, second_len, now.merge_copy(next));
                                } else {
                                    // in this case, we need to split in 3
                                    int len1 = next.start - now.start;
                                    int len2 = now.len - len1 - next.len;
                                    var now1 = new text_part(now.start, len1, now);
                                    var now2 = new text_part(next.start + next.len, len2, now);
                                    Debug.Assert(len1 >= 0 && len2 >= 0);
                                    parts_[idx] = now1;
                                    // 1.7.6
                                    parts_[idx + 1] = now.merge_copy(next);
                                    parts_.Insert(idx + 2, now2);
                                }
                            } else {
                                // 1.7.6+ they just intersect - split into three
                                int intersect_count = now.start + now.len - next.start;
                                Debug.Assert(intersect_count > 0);
                                int now_new_len = now.len - intersect_count;
                                Debug.Assert(now_new_len >= 0);
                                parts_[idx] = new text_part(now.start, now_new_len, now);
                                var intersected = new text_part(now.start + now_new_len, intersect_count, now.merge_copy(next));
                                parts_[idx + 1] = new text_part(next.start + intersect_count, next.len - intersect_count, next);
                                parts_.Insert(idx + 1, intersected);
#if old_code
    // normally, this should actually be split in three - but for now, leave it like this:
    //          just allow the latter become bigger
                                int intersect_count = now.start + now.len - next.start;
                                Debug.Assert( intersect_count > 0);
                                int interesect_len = now.len - intersect_count;
                                Debug.Assert(interesect_len >= 0);
                                now = new text_part(now.start, interesect_len, now);
                                parts_[idx] = now;
#endif
                            }
                        }
                    }
                }
            }

            foreach ( var p in parts_)
                Debug.Assert(p.start >= 0 && p.len > 0);
        }

        public void add_part(text_part part) {
            add_parts( new List<text_part>() { part } );
        }

        // converts '\r\n' to '\r' = this is a must for rich text box - because otherwise we'd end up having the wrong chars printed with different infos
        // (since rich text box considers "\r\n" as a single char, thus we would end up printing colored text off-by-one for each new line)
        public formatted_text to_single_enter_char() {
            var parts = parts_.ToList();
            string text = text_;
            while (true) {
                int next_enter = text.IndexOf('\n');
                if (next_enter < 0)
                    break;

                int start = parts.FindIndex(x => x.start > next_enter);
                if ( start >= 0)
                    for ( int i = start; i < parts.Count; ++i)
                        parts[i] = new text_part( parts[i].start - 1, parts[i].len, parts[i] );
                text = text.Substring(0, next_enter) + text.Substring(next_enter + 1);
            }

            return new formatted_text(text) { parts_ = parts };
        }

        // this updates text + infos, so that it will return a single line from a possible multi-line text
        // we want this, in case the text is multi-line, but we can only print A SINGLE LINE 
        public formatted_text get_most_important_single_line() {
            string text = text_;
            var parts = parts_.ToList();
            if (!text.Contains('\r') && !text.Contains('\n'))
                // text is single line
            return new formatted_text(text) { parts_ = parts };


            char more = '¶';
            var lines = util.split_into_lines(text, util.split_into_lines_type.include_enter_chars_in_returned_lines).ToList();
            if (parts.Count == 0) {
                // in this case, we don't have any custom printing - just return the first non empty line
                text = lines.FirstOrDefault(x => x.Length > 0 && !util.any_enter_char.Contains(x[0]));
                if (text == null)
                    // we only have empty lines
                    text = "";
                else {
                    int line_idx = lines.IndexOf(text);
                    text = (line_idx > 0 && app.inst.show_paragraph_sign ? more + " " : "") + text.Trim() + (line_idx < lines.Count - 1 && app.inst.show_paragraph_sign ? " " + more : "");
                }
                return new formatted_text(text) { parts_ = parts };
            }

            // we have custom printing - first, see if we have typed search
            var relevant_print = parts.FirstOrDefault(x => x.is_typed_search || x.is_find_search);
            if (relevant_print == null)
                relevant_print = parts[0];

            // find the relevant line
            int start = 0;
            string relevant_line = null;
            foreach ( var line in lines)
                if (relevant_print.start < start + line.Length) {
                    relevant_line = line;
                    break;
                } else
                    start += line.Length;
            Debug.Assert(relevant_line != null);
            bool line_before = start > 0;
            bool line_after = start + relevant_line.Length < text.Length;
            // at this point, ignore enters
            relevant_line = relevant_line.Trim();
            int len = relevant_line.Length;
            // ... just take the print infos for the relevant line
            parts = parts.Where(x => (start <= x.start && x.start < start + len) || (start <= x.start + x.len && x.start + x.len < start + len) ).ToList();
            if (parts.Count > 0) {
                // adjust first and last - they might be outside our line
                if (parts[0].start < start) 
                    parts[0] = new text_part(start, parts[0].len - (start - parts[0].start), parts[0]);
                var last = parts[parts.Count - 1];
                if ( last.start + last.len > start + len) 
                    parts[parts.Count - 1] = new text_part(last.start, start + len - last.start, last);
            }

            if (!app.inst.show_paragraph_sign)
                line_before = line_after = false;

            // convert all the indexes into the relevant line
            for ( int i = 0; i < parts.Count; ++i)
                parts[i] = new text_part(parts[i].start - start + (line_before ? 2 : 0), parts[i].len, parts[i]);

            text = (line_before ? more + " " : "") + relevant_line + (line_after ? " " + more : "");

            return new formatted_text(text) { parts_ = parts };
        }

        public List<text_part> parts(text_part default_) {
            List<text_part> result = new List<text_part>();
            int last_idx = 0;
            for (int print_idx = 0; print_idx < parts_.Count; ++print_idx) {
                int cur_idx = parts_[print_idx].start, cur_len = parts_[print_idx].len;
                string before =  text_.Substring(last_idx, cur_idx - last_idx);
                if (before != "") 
                    result.Add( new text_part(last_idx, cur_idx - last_idx, default_) { text = before });

                result.Add(new text_part(cur_idx, cur_len, parts_[print_idx]) { text = text_.Substring(cur_idx, cur_len) } );
                last_idx = cur_idx + cur_len;
            }
            last_idx = parts_.Count > 0 ? parts_.Last().start + parts_.Last().len : 0;
            if (last_idx < text_.Length) 
                result.Add(new text_part(last_idx, text_.Length - last_idx, default_) { text = text_.Substring(last_idx) } );

            return result;
        } 


    }
}
