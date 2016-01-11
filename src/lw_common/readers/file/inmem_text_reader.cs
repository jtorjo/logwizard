/* 
 * Copyright (C) 2014-2015 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com 
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogWizard;

namespace lw_common {
    // in-memory text - mainly for guessing the log syntax
    //
    public class inmem_text_reader : file_text_reader_base {
        private string lines_;
        private ulong len_;

        public inmem_text_reader(string lines, string syntax) : base(new log_settings_string("")) {
            lines_ = lines;
            len_ = (ulong)lines.Length;
            write_settings.syntax.set(syntax);
        }

        public override bool has_more_cached_text() {
            return lines_.Length > 0;
        }

        public override ulong try_guess_full_len {
            get { return len_; }
        }

        public override bool fully_read_once {
            get { return lines_.Length == 0; }
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
