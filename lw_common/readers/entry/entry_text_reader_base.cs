using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lw_common.parse;

namespace lw_common {
    public abstract class entry_text_reader_base : text_reader {


        internal abstract List<log_entry_line> read_next_lines();

        protected entry_text_reader_base(settings_as_string sett) : base(sett) {
        }
    }
}
