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
using System.Drawing;
using System.Linq;
using System.Text;
using LogWizard;

namespace lw_common.parse.parsers {


    class text_file_part_on_single_line  : log_parser_base {

        private settings_as_string sett_;

        private char separator_ = ':';

        public text_file_part_on_single_line(text_reader reader, settings_as_string sett) {
            sett_ = sett;
        }

        public static bool is_single_line(string file, settings_as_string sett) {
            string[] lines = util.read_beginning_of_file(file, 16834).Split( '\n' );
            for (int index = 0; index < lines.Length; index++) 
                lines[index] = lines[index].Replace("\r", "");

            int empty_lines = 0;
            string separator = sett.get("separator");
            if (separator == "")
                separator = ":";
            int contains_separator = 0;
            foreach ( string line in lines)
                if (line == "")
                    ++empty_lines;
                else if (line.Contains(separator))
                    ++contains_separator;

            // at least 3 entries
            if ( empty_lines > 3)
                if (contains_separator + empty_lines == lines.Length)
                    return true;

            return false;
        }


        public override void read_to_end() {
        }

        public override int line_count {
            get { return 0; }
        }

        public override line line_at(int idx) {
            return null;
        }

        public override void force_reload() {
        }

        public override bool up_to_date {
            get { return false; }
        }
    }
}
