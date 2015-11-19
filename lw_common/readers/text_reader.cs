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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace lw_common
{
    public abstract class text_reader : IDisposable
    {
        public const string UNKNOWN_SYNTAX = find_log_syntax.UNKNOWN_SYNTAX;
        private bool disposed_ = false;

        private log_parser parser_ = null;

        private readonly settings_as_string settings_ = new settings_as_string("");

        protected error_list_keeper errors_ = new error_list_keeper();

        public string name {
            get { return settings.get("name"); }
        }

        // 1.5.6+ - returns encountered errors, if any (to be able to show them visually)
        public List<string> errors {
            get { return errors_.errors; }
        }

        internal void on_set_parser(log_parser parser) {
            // call this only once!
            Debug.Assert(parser_ == null);
            parser_ = parser;                
        }

        public string unique_id {
            get {
                string guid = settings_.get("guid");
                Debug.Assert(guid != "");
                return guid;
            }
        }



        // 1.3.5+ - know when the log has been fully read at least once - useful to know whether to send "new_lines" events
        public abstract bool fully_read_once { get; }

        // 1.0.57+ - if true, the log has been rewritten from scratch
        public virtual bool has_it_been_rewritten {
            get { return false; }
        }

        // returns true when the read is fully read
        public virtual bool is_up_to_date() {
            return true;
        }


        public virtual void force_reload() {            
        }






        protected bool disposed {
            get { return disposed_; }
        }

        protected log_parser parser {
            get { return parser_; }
        }

        public settings_as_string_readonly settings {
            get { return settings_; }
        }

        internal void set_setting(string name, string value) {
            settings_.set(name, value);
        }

        public void merge_setings(string settings_str) {
            settings_.merge(settings_str);
        }

        public virtual void on_dispose() {
        }

        public void Dispose() {
            disposed_ = true;
            on_dispose();
            if ( parser_ != null)
                parser_.on_text_reader_dispose();
        }


        public static string type(text_reader reader) {
            if (reader is file_text_reader)
                return "file";

            if (reader is inmem_text_reader)
                return "file";

            if (reader is event_log_reader)
                return "event_log";
            if (reader is debug_text_reader)
                return "debug_print";

            Debug.Assert(false);
            return "file";
        }


    }
}
