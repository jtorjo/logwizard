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
using System.Linq;

namespace lw_common {
    public enum info_type { 
        // 1.6.13+ IMPORTANT: in log_view, I'm persisting them as ints - thus, I can't change the order (or assign constant values to each)
        date, time, level, 

        // added in 1.3.8
        thread, 
        
        file, func, class_, 
        
        ctx1, ctx2, ctx3, 

        // added in 1.3.8
        ctx4, ctx5, ctx6, ctx7, ctx8, ctx9, ctx10, ctx11, ctx12, ctx13, ctx14, ctx15,

        msg,

        max, 
        
        // 1.5.4+ - special field, used only in full log - I need this only to have a one-to-one correspondence from column index to info_type
        view ,
        // 1.5.4+ - special field - I need this only to have a one-to-one correspondence from column index to info_type
        line,
    }


    // ctxX -> context about the message (other than file/func/class)
    //
    public static class info_type_io {

        private static info_type[] searchable_ = searchable_types();

        private static info_type[] searchable_types() {
            List<info_type> types = new List<info_type>();
            foreach (info_type type in Enum.GetValues(typeof(info_type)))
                if ( is_searchable(type))
                    types.Add(type);
            return types.ToArray();
        }

        // returns the types that are searchable via Find (Ctrl-F)
        public static info_type[] searchable {
            get { return searchable_; }
        }

        // #26 - returns the importance of each column (should it be shown in the view by default?)
        //       the idea is to keep just a few in the view, not to clog it, and the rest to show them in the Details pane
        //
        //       I return an integer - the higher it is, the more important the column is
        public static int show_in_view_by_default(info_type type) {
            switch (type) {
            case info_type.msg:     return Int32.MaxValue;

            case info_type.line:    return 100;
            case info_type.view:    return 99;

            case info_type.time:    return 90;
            case info_type.level:   return 89;

            case info_type.thread:  return 87;

            case info_type.file:    return 70;
            case info_type.func:    return 69;
            case info_type.class_:  return 68;

            case info_type.ctx1:    return 60;
            case info_type.ctx2:    return 59;
            case info_type.ctx3:    return 58;

            case info_type.ctx4:    return 57;
            case info_type.ctx5:    return 56;
            case info_type.ctx6:    return 55;
            case info_type.ctx7:    return 54;
            case info_type.ctx8:    return 53;
            case info_type.ctx9:    return 52;
            case info_type.ctx10:   return 51;
            case info_type.ctx11:   return 50;
            case info_type.ctx12:   return 49;
            case info_type.ctx13:   return 48;
            case info_type.ctx14:   return 47;
            case info_type.ctx15:   return 46;

            // 1.7.35+ the idea is that time column should contain everything
            case info_type.date:    return 40;

            case info_type.max:     return -1;
            default:
                Debug.Assert(false);
                return -1;
            }
        }

        // for matching a type into a column, based on importance
        // for now, using this for matching database columns to logwizard columns
        public static info_type[] order_of_importance {
            get {
                return new[] {

                    info_type.ctx1,
                    info_type.ctx2, info_type.ctx3, info_type.ctx4,
                    info_type.ctx5, info_type.ctx6, info_type.ctx7, 
                    info_type.ctx8, info_type.ctx9, info_type.ctx10, info_type.ctx11, info_type.ctx12,
                    info_type.ctx13, info_type.ctx14, info_type.ctx15, 

                    info_type.msg,
                    
                    info_type.date, info_type.time, info_type.level, info_type.thread, info_type.file, info_type.func, info_type.class_, 


                    // should never arrive here
                    info_type.view, info_type.line, 
                    
                    info_type.max, 
                };
            }
        }

        // matches the database column names to logwizard columns
        // as input, you have the original match - if Item2 is not info_type.max, then it's pre-matched.
        public static List<Tuple<string, info_type>> match_db_column_to_lw_column(List<Tuple<string, info_type>> columns) {
            List<Tuple<string, info_type>> result = new List<Tuple<string, info_type>>();

            var unused = order_of_importance.ToList();
            foreach ( var col in columns)
                if (col.Item2 != info_type.max)
                    unused.Remove(col.Item2);

            // looking for common names:      date, time, timestamp, time_stamp, msg, messsage
            int time_column_count = columns.Select(x => x.Item1.ToLower()).Count(x => x == "date" || x == "time" || x == "timestamp" || x == "time_stamp");
            foreach ( var col in columns)
                if (col.Item2 != info_type.max)
                    // pre-matched
                    result.Add(col);
                else {
                    string col_name = col.Item1.ToLower();
                    bool is_time = col_name == "date" || col_name == "time" || col_name == "timestamp" || col_name == "time_stamp";
                    bool is_msg = col_name == "message" || col_name == "msg";
                    var use_now = unused[0];
                    if (is_msg)
                        use_now = info_type.msg;
                    else if (is_time) {
                        if (time_column_count == 1)
                            // in this case, easy, it's always the time column
                            use_now = info_type.time;
                        else
                            use_now = col_name == "date" ? info_type.date : info_type.time;
                    }
                    else if (col_name == "logger")
                        use_now = info_type.class_;
                    else if (col_name == "level")
                        use_now = info_type.level;

                    if (!unused.Contains(use_now))
                        use_now = unused[0];

                    result.Add(new Tuple<string,info_type>(col.Item1, use_now));
                    unused.Remove(use_now);
                }

            // if msg is never found, it will be the LAST column that was not pre-matched
            if (unused.Contains(info_type.msg)) {
                var last_unused = columns.LastOrDefault(x => x.Item2 == info_type.max);
                if (last_unused != null) {
                    int idx = result.FindIndex(x => x.Item1 == last_unused.Item1);
                    Debug.Assert(idx >= 0);
                    result[idx] = new Tuple<string, info_type>(last_unused.Item1, info_type.msg);
                }
            }

            return result;
        }

        public static bool is_searchable(info_type type) {
            switch (type) {
            case info_type.max:

            case info_type.date:
            case info_type.time:
            case info_type.level:
            case info_type.view:
            case info_type.line:
                return false;

            case info_type.thread:
            case info_type.file:
            case info_type.func:
            case info_type.class_:

            case info_type.ctx1:
            case info_type.ctx2:
            case info_type.ctx3:
            case info_type.ctx4:
            case info_type.ctx5:
            case info_type.ctx6:
            case info_type.ctx7:
            case info_type.ctx8:
            case info_type.ctx9:
            case info_type.ctx10:
            case info_type.ctx11:
            case info_type.ctx12:
            case info_type.ctx13:
            case info_type.ctx14:
            case info_type.ctx15:
            case info_type.msg:
                return true;

            default:
                Debug.Assert(false);
                return false;
            }
        }

        public static bool can_be_category(info_type type) {
            switch (type) {
            case info_type.max:

            case info_type.date:
            case info_type.time:
            case info_type.view:
            case info_type.line:
            case info_type.msg:
                return false;

            case info_type.level:
            case info_type.thread:
            case info_type.file:
            case info_type.func:
            case info_type.class_:

            case info_type.ctx1:
            case info_type.ctx2:
            case info_type.ctx3:
            case info_type.ctx4:
            case info_type.ctx5:
            case info_type.ctx6:
            case info_type.ctx7:
            case info_type.ctx8:
            case info_type.ctx9:
            case info_type.ctx10:
            case info_type.ctx11:
            case info_type.ctx12:
            case info_type.ctx13:
            case info_type.ctx14:
            case info_type.ctx15:
                return true;

            default:
                Debug.Assert(false);
                return false;
            }
        }

        public static bool can_be_multi_line(info_type type) {
            if (app.inst.force_text_as_multi_line)
                return true;

            switch (type) {
            case info_type.date:
            case info_type.time:
            case info_type.level:
            case info_type.thread:
            case info_type.file:
            case info_type.func:
            case info_type.class_:
            case info_type.view:
            case info_type.line:
                return false;

            case info_type.ctx1:
            case info_type.ctx2:
            case info_type.ctx3:
            case info_type.ctx4:
            case info_type.ctx5:
            case info_type.ctx6:
            case info_type.ctx7:
            case info_type.ctx8:
            case info_type.ctx9:
            case info_type.ctx10:
            case info_type.ctx11:
            case info_type.ctx12:
            case info_type.ctx13:
            case info_type.ctx14:
            case info_type.ctx15:
            case info_type.msg:
                return true;

            case info_type.max:
            default:
                Debug.Assert(false);
                return false;
            }
        }

        public static info_type from_str(string type_str) {
            info_type type = info_type.max;
            switch (type_str) {
            case "msg":
                type = info_type.msg;
                break;

            case "time":
                type = info_type.time;
                break;
            case "date":
                type = info_type.date;
                break;
            case "level":
                type = info_type.level;
                break;
            case "class":
                type = info_type.class_;
                break;
            case "file":
                type = info_type.file;
                break;
            case "func":
                type = info_type.func;
                break;
            case "thread":
                type = info_type.thread;
                break;

            case "ctx1":
                type = info_type.ctx1;
                break;
            case "ctx2":
                type = info_type.ctx2;
                break;
            case "ctx3":
                type = info_type.ctx3;
                break;

            case "ctx4":
                type = info_type.ctx4;
                break;
            case "ctx5":
                type = info_type.ctx5;
                break;
            case "ctx6":
                type = info_type.ctx6;
                break;
            case "ctx7":
                type = info_type.ctx7;
                break;
            case "ctx8":
                type = info_type.ctx8;
                break;
            case "ctx9":
                type = info_type.ctx9;
                break;
            case "ctx10":
                type = info_type.ctx10;
                break;

            case "ctx11":
                type = info_type.ctx11;
                break;
            case "ctx12":
                type = info_type.ctx12;
                break;
            case "ctx13":
                type = info_type.ctx13;
                break;
            case "ctx14":
                type = info_type.ctx14;
                break;
            case "ctx15":
                type = info_type.ctx15;
                break;

            case "view":
                type = info_type.view;
                break;
            case "line":
                type = info_type.line;
                break;
            default:
                break;
            }
            return type;
        }

        public static string to_friendly_str(info_type i) {
            switch (i) {
            case info_type.time:
                return "Time";
            case info_type.date:
                return "Date";
            case info_type.level:
                return "Level";
            case info_type.thread:
                return "Thread";
            case info_type.class_:
                return "Class";
            case info_type.file:
                return "File";
            case info_type.func:
                return "Func";
            case info_type.ctx1:
                return "Ctx1";
            case info_type.ctx2:
                return "Ctx2";
            case info_type.ctx3:
                return "Ctx3";
            case info_type.ctx4:
                return "Ctx4";
            case info_type.ctx5:
                return "Ctx5";
            case info_type.ctx6:
                return "Ctx6";
            case info_type.ctx7:
                return "Ctx7";
            case info_type.ctx8:
                return "Ctx8";
            case info_type.ctx9:
                return "Ctx9";
            case info_type.ctx10:
                return "Ctx10";
            case info_type.ctx11:
                return "Ctx11";
            case info_type.ctx12:
                return "Ctx12";
            case info_type.ctx13:
                return "Ctx13";
            case info_type.ctx14:
                return "Ctx14";
            case info_type.ctx15:
                return "Ctx15";
            case info_type.msg:
                return "Message";

            case info_type.view:
                return "View(s)";
            case info_type.line:
                return "Line";
            default:
            case info_type.max:
                Debug.Assert(false);
                return "";
            }
        }
    }
}