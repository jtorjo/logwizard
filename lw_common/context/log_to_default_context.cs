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
using lw_common;

namespace LogWizard {
    // find out information on the file/log - from its header
    public class log_to_default_context {
        /*
        private static Dictionary<string, string> file_to_context_ = new Dictionary<string, string>() {
            { "HM2 Version: 2.", "HM2" },
            { "HM3 Version=3", "HM3" },
            { "[find] Starting ScrapeFindPot", "TN2ScrapeFindPot" },
            { "Welcome to TableNinja!", "TableNinja" },
            { "This is a LogWizard Setup sample!", "SetupSample" },
            //{ "", "" },
        }; 
        */
        public static string file_to_context(string name) {
            string file_header = util.read_beginning_of_file(name, 8192);
            foreach ( var ftc in app.inst.file_to_context)
                if (file_header.Contains(ftc.Key))
                    return ftc.Value;

            return null;
        }
    }
}
