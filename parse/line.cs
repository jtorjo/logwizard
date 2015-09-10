using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LogWizard {
    // ctxX -> context about the message (other than file/func/class)
    enum info_type { 
        time, date, level, 
        // not implemented yet
        thread, 
        
        class_, file, func, ctx1, ctx2, ctx3, 

        msg,

        max 
    }

    class line {
        private sub_string sub_;
        // note: I could theoretically keep the length as a ubyte - and only the length of the message as a short
        //       however, I don't think it's worth doing, since if in the future, I would parse more complicated logs,
        //       we could end up with several parts of the message being bigger than 255 chars, so we'd have to come back to this again
        //
        //      thus, not worth doing
        private short[] parts = new short[(int)info_type.max * 2];

        // if not minvalue, it's the time this message was written
        public DateTime time = DateTime.MinValue;

        // for debugging
        public override string ToString() {
            return sub_.msg;
        }

        public string full_line {
            get {
                string full = "";
                foreach (info_type i in Enum.GetValues(typeof (info_type))) 
                    if ( i != info_type.max) {
                        string sub = part(i);
                        if (sub != "")
                            full += sub + " ";
                    }
                return full;
            }
        }

        public line(sub_string sub, Tuple<int, int>[] idx_in_line) {
            sub_ = sub;
            Debug.Assert(idx_in_line.Length == (int)info_type.max);
            string msg = sub.msg;
            // ... indexes a short can hold
            Debug.Assert(msg.Length < 65536);

            for (int part_idx = 0; part_idx < idx_in_line.Length; ++part_idx) {
                var index = idx_in_line[part_idx];
                parts[part_idx * 2] = parts[part_idx * 2 + 1] = -1;

                if ( index.Item1 >= 0)
                    if ((index.Item2 >= 0 && msg.Length >= index.Item1 + index.Item2) || (index.Item2 < 0 && msg.Length >= index.Item1)) {

                        short start, len;
                        if (index.Item2 >= 0) {
                            start = (short) index.Item1;
                            len = (short) index.Item2;
                        } else {
                            start = (short) index.Item1;
                            len = (short) (msg.Length - index.Item1);
                        }

                        bool needs_trim = part_idx != (int) info_type.msg;
                        if ( needs_trim)
                            while (len > 0)
                                if (Char.IsWhiteSpace(msg[start + len - 1]))
                                    --len;
                                else
                                    break;

                        parts[part_idx * 2] = start;
                        parts[part_idx * 2 + 1] = len;
                    }
            }

            // normalize time - so that we can do proper comparisons when "Go to Line"
            var time_str = part(info_type.time);
            if (time_str != "")
                time = util.str_to_normalized_time(time_str);
        }

        public string part(info_type i) {
            Debug.Assert(i < info_type.max);
            if (parts[(int) i * 2] < 0 || parts[(int) i * 2 + 1] <= 0)
                return "";

            string msg = sub_.msg;
            var result = msg.Substring( parts[(int) i * 2], parts[(int) i * 2 + 1]);
            return result;
        }
    }






    /*  old implementation, before optimizing for memory

    class line {
        private string[] parts = new string[(int)info_type.max];

        // if not minvalue, it's the time this message was written
        public DateTime time = DateTime.MinValue;

        public string full_line {
            get {
                string full = "";
                foreach ( string p in parts)
                    if (p != null)
                        full += p + " ";
                return full;
            }
        }

        public line(sub_string sub, Tuple<int, int>[] idx_in_line) {
            Debug.Assert(idx_in_line.Length == (int)info_type.max);
            string msg = sub.sub;

            for (int part_idx = 0; part_idx < idx_in_line.Length; ++part_idx) {
                var index = idx_in_line[part_idx];
                if ( index.Item1 >= 0)
                    if ((index.Item2 >= 0 && msg.Length >= index.Item1 + index.Item2) || (index.Item2 < 0 && msg.Length >= index.Item1)) {
                        string result = index.Item2 >= 0 ? msg.Substring(index.Item1, index.Item2) : msg.Substring(index.Item1);
                        parts[part_idx] = part_idx == (int)info_type.msg ? result : result.Trim();
                    }
            }

            // normalize time - so that we can do proper comparisons when "Go to Line"
            var time_str = parts[(int) info_type.time];
            if (time_str != "" && time_str != null)
                time = util.str_to_normalized_time(time_str);
        }

        public string part(info_type i) {
            Debug.Assert(i < info_type.max);
            var result = parts[(int) i];
            return result ?? "";
        }
    }
    */
}
