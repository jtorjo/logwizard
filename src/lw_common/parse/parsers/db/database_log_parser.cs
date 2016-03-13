using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common.readers.entry;

namespace lw_common.parse.parsers.db
{
    class database_log_parser : generic_entry_log_parser
    {
        public database_log_parser(database_table_reader reader) : base(reader) {
        }
    }
}
