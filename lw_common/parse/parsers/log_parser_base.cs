using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.parse.parsers {
    internal abstract class log_parser_base : IDisposable {
        protected bool disposed_ = false;

        public abstract void read_to_end();

        public abstract int line_count { get; }

        public abstract line line_at(int idx);

        public abstract void force_reload();

        public abstract bool up_to_date { get; }

        public void Dispose() {
            disposed_ = true;
        }
    }
}
