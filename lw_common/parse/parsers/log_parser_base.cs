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

namespace lw_common.parse.parsers {
    internal abstract class log_parser_base : IDisposable {
        protected bool disposed_ = false;

        protected settings_as_string sett_;
        protected aliases aliases_;

        public log_parser_base(settings_as_string sett) {
            sett_ = sett;
            on_updated_settings();            
        }

        public abstract void read_to_end();

        public abstract int line_count { get; }

        public abstract line line_at(int idx);

        public abstract void force_reload();

        public abstract bool up_to_date { get; }

        // column names - parsed from the log (if any)
        public virtual  List<string> column_names {
            get { return new List<string>(); }
        }

        public void on_settings_changed(string settings) {
            sett_ = new settings_as_string(settings);
            on_updated_settings();            
        }

        protected virtual void on_updated_settings() {            
            aliases_ = new aliases(sett_.get("aliases"));
        }

        public void Dispose() {
            disposed_ = true;
        }

    }
}
