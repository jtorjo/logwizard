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
        static public log_settings_string get_context_dependent_settings(text_reader reader, log_settings_string_readonly settings) {
            if (reader is file_text_reader) 
                return settings.sub(new []{ settings.syntax.get(), settings.aliases, settings.description_template });
            
            return new log_settings_string( settings.ToString());
        }

        // these are the settings that are to be saved per-log
        static public log_settings_string get_log_dependent_settings(text_reader reader, log_settings_string_readonly settings) {
            return new log_settings_string( settings.ToString());
        }

        static public text_reader create_text_reader(log_settings_string settings) {
            Debug.Assert(settings.guid != "");

            switch (settings.type.get()) {
            case log_type.file:        return new file_text_reader(settings);
            case log_type.event_log:   return new event_log_reader(settings);
            case log_type.debug_print: return new debug_text_reader(settings);
            default:
                Debug.Assert(false);
                return null;
            }

        }

        static internal log_parser_base create_parser(text_reader reader) {
            var reader_sett = reader.write_settings;
            reader_sett.type.set( text_reader.type(reader));

            if ( reader_sett.guid == "")
                reader_sett.guid.set( Guid.NewGuid().ToString());

            if (reader is file_text_reader) 
                return create_file_parser(reader as file_text_reader);
            
            if (reader is inmem_text_reader) 
                // for testing syntax
                return new text_file_line_by_line(reader as inmem_text_reader);

            if (reader is event_log_reader) 
                return new event_viewer(reader as event_log_reader);

            if (reader is debug_text_reader) 
                return new debug_print(reader as debug_text_reader);
            

            Debug.Assert(false);
            return null;
        }

        public static file_log_type guess_file_type(string file_name) {
            if (file_name == "")
                return file_log_type.line_by_line;
            
            file_name = file_name.ToLower();
            if (file_name.EndsWith(".xml"))
                return file_log_type.xml;
            if (file_name.EndsWith(".csv"))
                return file_log_type.csv;

            if (text_file_part_on_single_line.is_single_line(file_name, new log_settings_string("")))
                return file_log_type.part_to_line;

            return file_log_type.line_by_line;
        }

        public static bool is_file_line_by_line(string file_name, string sett) {
            file_name = file_name.ToLower();
            var all = new log_settings_string(sett);

            switch (all.file_type.get()) {
            case file_log_type.line_by_line:
                return true;
            case file_log_type.best_guess:
                // best guess
                return guess_file_type(file_name) == file_log_type.line_by_line;
            default:
                return false;
            }
            
        }

        private static log_parser_base create_file_parser(file_text_reader reader) {
            string file_name = reader.name.ToLower();

            switch (reader.settings.file_type.get()) {
            case file_log_type.line_by_line:
                return new text_file_line_by_line(reader);
            case file_log_type.part_to_line:
                return new text_file_part_on_single_line(reader);
            case file_log_type.xml:
                return new xml_file(reader);
            case file_log_type.csv:
                return new csv_file(reader);
            case file_log_type.best_guess:
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

            string syntax = reader.settings.syntax;
            if ( syntax == "" || syntax == find_log_syntax.UNKNOWN_SYNTAX)
                if ( text_file_part_on_single_line.is_single_line(reader.name, reader.settings))
                    return new text_file_part_on_single_line(reader);

            return new text_file_line_by_line(reader);
        }
    }
}
