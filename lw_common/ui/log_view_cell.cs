using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;

namespace lw_common.ui {
    internal class log_view_cell {
        internal static string cell_value(log_view.item i, int column_idx) {
            switch (column_idx) {
            case 0:
                return "" + i.line;

            case 1:
                return i.view;

            case 2:
                return i.date;
            case 3:
                return i.time;
            case 4:
                return i.level;
            case 5:
                return i.thread;

            case 6:
                return i.file;
            case 7:
                return i.func;
            case 8:
                return i.class_;
            case 9:
                return i.ctx1;
            case 10:
                return i.ctx2;
            case 11:
                return i.ctx3;

            case 12:
                return i.ctx4;
            case 13:
                return i.ctx5;
            case 14:
                return i.ctx6;
            case 15:
                return i.ctx7;
            case 16:
                return i.ctx8;
            case 17:
                return i.ctx9;
            case 18:
                return i.ctx10;
            case 19:
                return i.ctx11;
            case 20:
                return i.ctx12;
            case 21:
                return i.ctx13;
            case 22:
                return i.ctx14;
            case 23:
                return i.ctx15;

            case 24:
                return i.msg;
            default:
                Debug.Assert(false);
                return "";
            }
        }

        internal static string cell_value_by_type(log_view.item i, info_type type) {
            switch (type) {
            case info_type.msg:
                return i.msg;

            case info_type.time:
                return i.time;
            case info_type.date:
                return i.date;
            case info_type.level:
                return i.level;
            case info_type.thread:
                return i.thread;

            case info_type.class_:
                return i.class_;
            case info_type.file:
                return i.file;
            case info_type.func:
                return i.func;

            case info_type.ctx1:
                return i.ctx1;
            case info_type.ctx2:
                return i.ctx2;
            case info_type.ctx3:
                return i.ctx3;

            case info_type.ctx4:
                return i.ctx4;
            case info_type.ctx5:
                return i.ctx5;
            case info_type.ctx6:
                return i.ctx6;
            case info_type.ctx7:
                return i.ctx7;
            case info_type.ctx8:
                return i.ctx8;
            case info_type.ctx9:
                return i.ctx9;
            case info_type.ctx10:
                return i.ctx10;

            case info_type.ctx11:
                return i.ctx11;
            case info_type.ctx12:
                return i.ctx12;
            case info_type.ctx13:
                return i.ctx13;
            case info_type.ctx14:
                return i.ctx14;
            case info_type.ctx15:
                return i.ctx15;
            }
            Debug.Assert(false);
            return i.msg;
        }

        internal static OLVColumn column(log_view lv, info_type type) {
            switch (type) {
            case info_type.msg:
                return lv. msgCol;

            case info_type.time:
                return lv. timeCol;
            case info_type.date:
                return lv. dateCol;
            case info_type.level:
                return lv. levelCol;
            case info_type.thread:
                return lv. threadCol;

            case info_type.file:
                return lv. fileCol;
            case info_type.func:
                return lv. funcCol;
            case info_type.class_:
                return lv. classCol;


            case info_type.ctx1:
                return lv. ctx1Col;
            case info_type.ctx2:
                return lv. ctx2Col;
            case info_type.ctx3:
                return lv. ctx3Col;

            case info_type.ctx4:
                return lv. ctx4Col;
            case info_type.ctx5:
                return lv. ctx5Col;
            case info_type.ctx6:
                return lv. ctx6Col;
            case info_type.ctx7:
                return lv. ctx7Col;
            case info_type.ctx8:
                return lv. ctx8Col;
            case info_type.ctx9:
                return lv. ctx9Col;
            case info_type.ctx10:
                return lv. ctx10Col;

            case info_type.ctx11:
                return lv. ctx11Col;
            case info_type.ctx12:
                return lv. ctx12Col;
            case info_type.ctx13:
                return lv. ctx13Col;
            case info_type.ctx14:
                return lv. ctx14Col;
            case info_type.ctx15:
                return lv. ctx15Col;
            }
            Debug.Assert(false);
            return lv. msgCol;
        }
    }
}
