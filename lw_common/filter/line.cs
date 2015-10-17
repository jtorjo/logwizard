using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace lw_common {
    // ctxX -> context about the message (other than file/func/class)
    //
    public enum info_type { 
        date, time, level, 

        // added in 1.3.8
        thread, 
        
        file, func, class_, 
        
        ctx1, ctx2, ctx3, 

        // added in 1.3.8
        ctx4, ctx5, ctx6, ctx7, ctx8, ctx9, ctx10, ctx11, ctx12, ctx13, ctx14, ctx15,

        msg,

        max 
    }

    public static class info_type_io {

        public static info_type from_str(string type_str) {
            info_type type = info_type.max;
            switch (type_str) {
            case "msg":     type = info_type.msg; break;

            case "time":    type = info_type.time; break;
            case "date":    type = info_type.date; break;
            case "level":   type = info_type.level; break;
            case "class":   type = info_type.class_; break;
            case "file":    type = info_type.file; break;
            case "func":    type = info_type.func; break;
            case "thread":  type = info_type.thread; break;

            case "ctx1":    type = info_type.ctx1; break;
            case "ctx2":    type = info_type.ctx2; break;
            case "ctx3":    type = info_type.ctx3; break;

            case "ctx4":    type = info_type.ctx4; break;
            case "ctx5":    type = info_type.ctx5; break;
            case "ctx6":    type = info_type.ctx6; break;
            case "ctx7":    type = info_type.ctx7; break;
            case "ctx8":    type = info_type.ctx8; break;
            case "ctx9":    type = info_type.ctx9; break;
            case "ctx10":    type = info_type.ctx10; break;

            case "ctx11":    type = info_type.ctx11; break;
            case "ctx12":    type = info_type.ctx12; break;
            case "ctx13":    type = info_type.ctx13; break;
            case "ctx14":    type = info_type.ctx14; break;
            case "ctx15":    type = info_type.ctx15; break;

            default:
                break;
            }
            return type;
        }

        public static string to_friendly_str(info_type i) {
            switch (i) {
            case info_type.time:                return "Time";
            case info_type.date:                return "Date";
            case info_type.level:               return "Level";
            case info_type.thread:              return "Thread";
            case info_type.class_:              return "Class";
            case info_type.file:                return "File";
            case info_type.func:                return "Func";
            case info_type.ctx1:                return "Ctx1";
            case info_type.ctx2:                return "Ctx2";
            case info_type.ctx3:                return "Ctx3";
            case info_type.ctx4:                return "Ctx4";
            case info_type.ctx5:                return "Ctx5";
            case info_type.ctx6:                return "Ctx6";
            case info_type.ctx7:                return "Ctx7";
            case info_type.ctx8:                return "Ctx8";
            case info_type.ctx9:                return "Ctx9";
            case info_type.ctx10:               return "Ctx10";
            case info_type.ctx11:               return "Ctx11";
            case info_type.ctx12:               return "Ctx12";
            case info_type.ctx13:               return "Ctx13";
            case info_type.ctx14:               return "Ctx14";
            case info_type.ctx15:               return "Ctx15";
            case info_type.msg:                 return "Message";

            default:
            case info_type.max:
                Debug.Assert(false);
                return "";
            }
        }
    }


    public class line {
        private sub_string sub_;
        // note: I could theoretically keep the length as a ubyte - and only the length of the message as a short
        //       however, I don't think it's worth doing, since if in the future, I would parse more complicated logs,
        //       we could end up with several parts of the message being bigger than 255 chars, so we'd have to come back to this again
        //
        //      thus, not worth doing
        private short[] parts = new short[(int) info_type.max * 2];

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
                    if (i != info_type.max) {
                        string sub = part(i);
                        if (sub != "")
                            full += sub + " ";
                    }
                return full;
            }
        }

        private line() {
            sub_ = new sub_string(null, 0);
        }

        public static line empty_line() {
            line l = new line();
            for (int i = 0; i < l.parts.Length / 2; ++i) {
                l.parts[i * 2] = -1;
                l.parts[i * 2 + 1] = 0;
            }
            return l;
        }

        public line(sub_string sub, Tuple<int, int>[] idx_in_line) {
            sub_ = sub;
            Debug.Assert(idx_in_line.Length == (int) info_type.max);
            string msg = sub.msg;
            // ... indexes a short can hold
            Debug.Assert(msg.Length < 65536);

            for (int part_idx = 0; part_idx < idx_in_line.Length; ++part_idx) {
                var index = idx_in_line[part_idx];
                parts[part_idx * 2] = parts[part_idx * 2 + 1] = -1;

                if (index.Item1 >= 0)
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
                        if (needs_trim)
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
            var result = msg.Substring(parts[(int) i * 2], parts[(int) i * 2 + 1]);
            return result;
        }
    }
}
