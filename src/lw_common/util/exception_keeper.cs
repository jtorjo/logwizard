using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common
{
    public class exception_keeper
    {
        private static exception_keeper inst_ = new exception_keeper();

        private readonly count_last_period last_errors_ = new count_last_period(10000);

        private const int MAX_ERRORS = 10;

        private exception_keeper() {
            is_fatal = false;
        }

        public static exception_keeper inst {
            get { return inst_; }
        }

        public bool is_fatal { get; internal set; }

        public void add_error() {
            last_errors_.add_now();
        }

        public bool too_many_errors {
            get { return last_errors_.count() > MAX_ERRORS; }
        }
    }
}
