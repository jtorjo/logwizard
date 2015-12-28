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
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;
using lw_common.parse.parsers;
using lw_common.parse.parsers.system;
using LogWizard;

namespace lw_common.parse {
    public class factory {

        // these are the settings that are to be saved in the context
        static public settings_as_string get_context_dependent_settings(text_reader reader, settings_as_string_readonly settings) {
            if (reader is file_text_reader) 
                return settings.sub(new []{ "syntax", "aliases", "description_template" });
            
            return new settings_as_string( settings.ToString());
        }

        // these are the settings that are to be saved per-log
        static public settings_as_string get_log_dependent_settings(text_reader reader, settings_as_string_readonly settings) {
            return new settings_as_string( settings.ToString());
        }

        static public text_reader create_text_reader(settings_as_string settings) {
            Debug.Assert(settings.get("guid") != "");

            switch (settings.get("type", "file")) {
            case "file":        return new file_text_reader(settings);
            case "event_log":   return new event_log_reader(settings);
            case "debug_print": return new debug_text_reader(settings);
            default:
                Debug.Assert(false);
                return null;
            }

        }

        static internal log_parser_base create_parser(text_reader reader) {
            reader.set_setting("type", text_reader.type(reader));
            if ( reader.settings.get("guid") == "")
                reader.set_setting("guid", Guid.NewGuid().ToString());

            if (reader is file_text_reader) 
                return create_file_parser(reader as file_text_reader);
            
            if (reader is inmem_text_reader) 
                // for testing syntax
                return new text_file_line_by_line(reader as inmem_text_reader);

            if (reader is event_log_reader) {
                if ( reader.settings.get("event.log_type") == "")
                    reader.set_setting("event.log_type", "Application|System");
                return new event_viewer(reader as event_log_reader);
            }

            if (reader is debug_text_reader) 
                return new debug_print(reader as debug_text_reader);
            

            Debug.Assert(false);
            return null;
        }

        public static string guess_file_type(string file_name) {
            if (file_name == "")
                return "line-by-line";
            
            file_name = file_name.ToLower();
            if (file_name.EndsWith(".xml"))
                return "xml";
            if (file_name.EndsWith(".csv"))
                return "csv";

            if (text_file_part_on_single_line.is_single_line(file_name, new settings_as_string("")))
                return "part-by-line";

            return "line-by-line";
        }

        public static bool is_file_line_by_line(string file_name, string sett) {
            file_name = file_name.ToLower();
            var all = new settings_as_string(sett);
            var file_type = all.get("file_type");

            switch (file_type) {
            case "line-by-line":
                return true;
            case "":
                // best guess
                return guess_file_type(file_name) == "line-by-line";
            default:
                return false;
            }
            
        }

        private static log_parser_base create_file_parser(file_text_reader reader) {
            string file_name = reader.name.ToLower();

            var file_type = reader.settings.get("file_type");
            switch (file_type) {
            case "line-by-line":
                return new text_file_line_by_line(reader);
            case "part-by-line":
                return new text_file_part_on_single_line(reader);
            case "xml":
                return new xml_file(reader);
            case "csv":
                return new csv_file(reader);
            case "":
                // best guess
                break;
            default:
                Debug.Assert(false);
                break;
            }

            if ( file_name.EndsWith(".xml"))
                return new xml_file(reader);
            if ( file_name.EndsWith(".csv"))
                return new csv_file(reader);

            string syntax = reader.settings.get("syntax");
            if ( syntax == "" || syntax == find_log_syntax.UNKNOWN_SYNTAX)
                if ( text_file_part_on_single_line.is_single_line(reader.name, reader.settings))
                    return new text_file_part_on_single_line(reader);

            return new text_file_line_by_line(reader);
        }
    }
}
