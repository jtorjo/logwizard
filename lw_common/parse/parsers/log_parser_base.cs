using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.parse.parsers {
    public class log_parser_base : IDisposable {
        protected bool disposed_ = false;

        public void Dispose() {
            disposed_ = true;
        }
    }
}
