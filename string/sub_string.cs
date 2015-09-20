using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWizard
{
    // represents a line in a large string
    struct sub_string {
        private large_string string_;
        private int line_idx_;

        public sub_string(large_string s, int line_idx) {
            string_ = s;
            line_idx_ = line_idx;
        }

        public string msg {
            get { return string_ != null ? string_.line_at(line_idx_) : ""; }
        }
    }
}
