using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common {
    public abstract class file_text_reader_base : text_reader {

        // reads text at position - and updates position
        public abstract string read_next_text() ;

        public virtual bool has_more_cached_text() {
            return false;
        }






        // 1.0.14+ - this computes the full length of the reader - until we call it again
        //           (since this can be costly CPU-wise)
        //           after this call, we can rely on full_len being constant until this is called again
        public abstract void compute_full_length();

        // 1.0.14+ - returns the length computed in compute_full_length()
        public abstract ulong full_len { get; }

        // 1.0.76+ - if != maxvalue, we try to guess the length of the log to read (so we can optimize our internal memory consumption)
        public virtual ulong try_guess_full_len {
            get { return ulong.MaxValue;  }
        }

        // the position in the log_parser (in bytes)
        // 1.0.72+ - made readonly
        public abstract ulong pos { get; }

        public virtual string try_to_find_log_syntax() {
            return UNKNOWN_SYNTAX;
        }

    }
}
