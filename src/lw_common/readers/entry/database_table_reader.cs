using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common.parse;

namespace lw_common.readers.entry
{
    class database_table_reader : entry_text_reader_base
    {
        public database_table_reader(log_settings_string sett) : base(sett) {
        }

        public override bool fully_read_once {
            get { return false; }
        }

        protected override List<log_entry_line> read_next_lines() {
            return null;
        }
    }
}
