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
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace lw_common.parse.parsers {
    internal abstract class log_parser_base : IDisposable {
        protected bool disposed_ = false;

        protected readonly log_settings_string_readonly sett_;
        private aliases aliases_;

        private List<string> column_names_ = new List<string>();

        private util.void_func on_aliases_changed_;

        protected log_parser_base(log_settings_string_readonly sett) {
            sett_ = sett;
            sett_.on_changed += on_settings_changed;
            on_updated_settings();            
        }

        public abstract void read_to_end();

        public abstract int line_count { get; }

        public abstract line line_at(int idx);

        public abstract void force_reload();

        public abstract bool up_to_date { get; }

        // returns true if we know we have columns that are multi-line
        public abstract bool has_multi_line_columns { get; }

        // column names - parsed from the log (if any)
        public List<string> column_names {
            get { lock(this) return column_names_; }
            internal set {
                lock(this)
                    column_names_ = value;
                if ( column_names_.Count > 0)
                    aliases_.on_column_names(column_names_);
            }
        }

        // used, for instance, for database column mappings
        internal List<Tuple<string, info_type>> column_names_to_info_type {
            set {
                lock (this)
                    column_names_ = value.Select(x => x.Item1).ToList();
                if ( column_names_.Count > 0)
                    aliases_.on_column_names(value);
            }
        } 

        internal aliases aliases {
            get { return aliases_; }
        }

        private void on_settings_changed(string name) {
            if (name == "name")
                // this is the friendly name assigned to this reader
                return;
            on_updated_settings();
        }

        protected virtual void on_updated_settings() {
            var new_aliases = new aliases(sett_.aliases);
            if (aliases_ != null && aliases_.to_enter_separated_string() == new_aliases.to_enter_separated_string())
                // nothing changed
                return;

            aliases_ = new_aliases;
            if ( column_names_.Count > 0)
                aliases_.on_column_names(column_names_);
        }

        internal log_settings_string_readonly settings {
            get { return sett_; }
        }

        internal util.void_func on_aliases_changed {
            get { return on_aliases_changed_; }
            set {
                on_aliases_changed_ = value;
                if (aliases_ != null)
                    aliases_.on_column_names_changed = on_aliases_changed_;
            }
        }

        public void Dispose() {
            disposed_ = true;
        }

    }
}
