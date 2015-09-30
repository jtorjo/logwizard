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
    class log_to_default_syntax {
        /*
        private static Dictionary<string, string> file_to_syntax_ = new Dictionary<string, string>() {
            { "HM2 Version: 2.", "$time[0,12] $ctx1['[','-'] $func[' ',']'] $ctx2['[[','] ]'] $msg" },
            {"HM3 Version=3", "$time[0,12] $ctx1['[',']'] $ctx2['[','] '] $msg|$time[0,12] $ctx1['[',']'] $msg['---']|$time[0,12] $ctx1['[',']'] $msg['  ']"},

            { "This is a LogWizard Setup sample", "$time[0,12] $ctx1[13,10] $level[24,5] $class[' ','- '] $msg" },
            { "Welcome to TableNinja! debug", "$file[0,': '] $time['',12] $ctx1[' ',10] $level[' ','- '] $msg" },
            //{ "", "" },
            { "logging started: \nCalling process: \nmsiexec.exe ===", "$ctx1['MSI (',') '] $ctx2['(',')'] $time['[',']: ']  $msg" },
        }; 
        */
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
    }
}
