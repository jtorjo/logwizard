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
using System.IO;
using System.Linq;
using System.Text;
using lw_common;

namespace LogWizard.context {
    // find out information on the file/log - from its header
    public class log_to {
        public static string file_to_syntax(string name) {
            string file_header = util.read_beginning_of_file(name, 8192);
            foreach (var fts in app.inst.file_to_syntax) {
                var phrases = fts.Key.Split('|');
                int count = 0;
                foreach (string sub in phrases)
                    if (file_header.Contains(sub))
                        ++count;

                if ( count == phrases.Count())
                    return fts.Value;
            }

            return null;
        }

        public static string file_to_context(string name) {
            string file_header = util.read_beginning_of_file(name, 8192);
            foreach ( var ftc in app.inst.file_to_context)
                if (file_header.Contains(ftc.Key))
                    return ftc.Value;

            return null;
        }

        public static string log_to_settings(string name) {
            if (app.inst.file_to_settings.ContainsKey(name))
                return app.inst.file_to_settings[name];
            return "";
        }
    }
}
