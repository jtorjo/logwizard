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
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWizard
{
    public abstract class text_reader : IDisposable
    {
        public const string UNKNOWN_SYNTAX = find_log_syntax.UNKNOWN_SYNTAX;
        private bool disposed_ = false;

        public delegate void on_new_lines_func();
        // it's called: 
        //  - after the log has been fully read, when there are new lines appended to it
        public on_new_lines_func on_new_lines;


        public virtual string name {
            get { return ""; }
        }

        
        
        
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

        // the position in the log_line_parser (in bytes)
        // 1.0.72+ - made readonly
        public abstract ulong pos { get; }






        // 1.3.5+ - know when the log has been fully read at least once - useful to know whether to send "new_lines" events
        public abstract bool fully_read_once { get; }

        // 1.0.57+ - if true, the file has been rewritten from scratch
        public virtual bool has_it_been_rewritten {
            get { return false; }
        }

        // returns true when the read is fully read
        public virtual bool is_up_to_date() {
            return true;
        }

        public virtual string try_to_find_log_syntax() {
            return "$msg[0]";
        }

        public virtual void force_reload() {            
        }






        protected bool disposed {
            get { return disposed_; }
        }
        public virtual void on_dispose() {
        }

        public void Dispose() {
            disposed_ = true;
            on_dispose();
        }
    }
}
