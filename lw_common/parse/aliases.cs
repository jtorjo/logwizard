using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.parse {
    class aliases {

        public info_type to_info_type(string alias) {
            // FIXME others

            switch (alias) {
            case "time" :   return info_type.time;
            case "date":    return info_type.date;
            case "level":   return info_type.level;
            case "thread":  return info_type.thread;
            case "file":    return info_type.file;
            case "func":    return info_type.func;
            case "class":   return info_type.class_;
            case "msg":     return info_type.msg;

            case "ctx1":    return info_type.ctx1;
            case "ctx2":    return info_type.ctx2;
            case "ctx3":    return info_type.ctx3;
            case "ctx4":    return info_type.ctx4;
            case "ctx5":    return info_type.ctx5;
            case "ctx6":    return info_type.ctx6;
            case "ctx7":    return info_type.ctx7;
            case "ctx8":    return info_type.ctx8;
            case "ctx9":    return info_type.ctx9;
            case "ctx10":    return info_type.ctx10;

            case "ctx11":    return info_type.ctx11;
            case "ctx12":    return info_type.ctx12;
            case "ctx13":    return info_type.ctx13;
            case "ctx14":    return info_type.ctx14;
            case "ctx15":    return info_type.ctx15;
            }

            return 0;
        }
    }
}
