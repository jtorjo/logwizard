using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogWizard.readers;

namespace LogWizard {
    // in-memory text - mainly for guessing the log syntax
    //
    class inmem_text_reader : text_reader {
        private string lines_;
        private ulong len_;

        public inmem_text_reader(string lines) {
            lines_ = lines;
            len_ = (ulong)lines.Length;
        }

        public override bool has_more_cached_text() {
            return lines_.Length > 0;
        }

        public override ulong try_guess_full_len {
            get { return len_; }
        }

        public override string name {
            get { return base.name; }
        }

        public override bool has_it_been_rewritten {
            get { return false; }
        }

        public override bool is_up_to_date() {
            return lines_.Length == 0;
        }

        public override string try_to_find_log_syntax() {
            return find_log_syntax.UNKNOWN_SYNTAX;
        }

        public override void on_dispose() {
            base.on_dispose();
        }

        public override void force_reload() {
            base.force_reload();
        }

        public override string read_next_text() {
            string next = lines_;
            lines_ = "";
            return next;
        }

        public override void compute_full_length() {
        }

        public override ulong full_len {
            get { return len_; }
        }

        public override ulong pos {
            get { return lines_.Length > 0 ? 0 : len_ ; }
        }
    }
}
