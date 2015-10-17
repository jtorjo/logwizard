using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lw_common.parse.parsers;

namespace lw_common.parse {
    public class line_by_line_syntax : syntax_base {
        public string line_syntax = "";

        // if true, if a line does not match the syntax, assume it's from previous line
        public bool if_line_does_not_match_assume_from_prev_line = false;
    }
}
