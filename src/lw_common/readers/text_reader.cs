/* 
 * Copyright (C) 2014-2016 John Torjo
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
using lw_common.readers.entry;

namespace lw_common
{
    public abstract class text_reader : IDisposable
    {
        public const string UNKNOWN_SYNTAX = find_log_syntax.UNKNOWN_SYNTAX;
        private bool disposed_ = false;

        private log_parser parser_ = null;

        private readonly log_settings_string settings_ ;

        protected error_list_keeper errors_ = new error_list_keeper();

        private bool reverse_order_ = false;

        protected text_reader(log_settings_string sett) {
            settings_ = sett;
            settings_.on_changed += on_settings_changed;
            on_settings_changed("");
        }


        // 1.3.5+ - know when the log has been fully read at least once - useful to know whether to send "new_lines" events
        public abstract bool fully_read_once { get; }

        // 1.0.57+ - if true, the log has been rewritten from scratch
        public virtual bool has_it_been_rewritten {
            get { return false; }
        }

        // returns true when the log is fully read at this point
        public virtual bool is_up_to_date() {
            return true;
        }

        public virtual string progress {
            get { return ""; }
        }

        public virtual void force_reload(string reason) {
        }

        public virtual void on_dispose() {
        }

        public virtual string friendly_name {
            get { return name; }
        }

        // 1.5.6+ if this returns true, the settings are incomplete and the user needs to fill something
        //
        // case in point - when the user wants to view event logs from another machine, he needs to enter his password (we don't save it)
        public virtual bool are_settings_complete {
            get { return true; }
        }

        // 1.6.16 if true, the element are held internally in reverse order - last element is held first (such as, you can do this for Event Log)
        public virtual bool are_elements_in_reverse_order {
            get { return false; }
        }


        /////////////////////////////////////////////////////////////////////////////////////
        // non-overridables

        // 1.6.10+ - if true, we reverse the order of items read
        //           each reader can decide what to do with this
        //
        //           at this time, we are using this in Event Log viewer, where new items are shown at the top by default
        public bool show_elements_in_reverse_order {
            get { return reverse_order_; }
        }

        private void on_settings_changed(string sett_name) {
            if ( sett_name != settings_.name.name)
                settings_.name.set(friendly_name);
            reverse_order_ = settings_.reverse;
        }

        public string name {
            get { return settings.name; }
        }

        // 1.5.6+ - returns encountered errors, if any (to be able to show them visually)
        public List< Tuple<string,error_list_keeper.level_type> > errors {
            get { return errors_.errors; }
        }

        internal void add_error(string err, error_list_keeper.level_type level = error_list_keeper.level_type.error) {
            errors_.add(err, level);
        }

        internal void on_set_parser(log_parser parser) {
            // call this only once!
            Debug.Assert(parser_ == null);
            parser_ = parser;                
        }

        public string guid {
            get {
                string guid = settings_.guid;
                Debug.Assert(guid != "");
                return guid;
            }
        }


        protected bool disposed {
            get { return disposed_; }
        }

        protected log_parser parser {
            get { return parser_; }
        }

        public log_settings_string_readonly settings {
            get { return settings_; }
        }

        // FIXME I don't like allowing writeable access to log settings - i need to think if i can accomplish this in another way, 
        //       like, allow some settings to be writable
        public log_settings_string write_settings {
            get { return settings_; }
        }


        public void Dispose() {
            disposed_ = true;
            on_dispose();
            if ( parser_ != null)
                parser_.on_text_reader_dispose();
        }

        public static log_type type(text_reader reader) {
            if (reader is file_text_reader)
                return log_type.file;

            if (reader is inmem_text_reader)
                return log_type.file;

            if (reader is event_log_reader)
                return log_type.event_log;
            if (reader is debug_text_reader)
                return log_type.debug_print;
            if (reader is database_table_reader)
                return log_type.db;

            Debug.Assert(false);
            return log_type.file;
        }


    }
}
