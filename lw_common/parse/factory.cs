using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using lw_common.parse.parsers;
using LogWizard;

namespace lw_common.parse {
    public class factory {

        // these are the settings that are to be saved in the context
        static public string get_context_dependent_settings(text_reader reader, string settings) {
            if (reader is file_text_reader) {
                return new settings_as_string(settings).sub(new []{ "syntax", "aliases" }).ToString();
            }

            return settings;
        }
        // these are the settings that are to be saved per-log
        static public string get_log_dependent_settings(text_reader reader, string settings) {
            return settings;
        }

        // note: log-dependent settings always override the context
        static public string merge_settings(string context, string log) {
            var sett = new settings_as_string(context);
            sett.merge(new settings_as_string(log));
            return sett.ToString() ;
        }

        static public string get_single_setting(string settings, string name) {
            return new settings_as_string(settings).get(name);
        }


        static internal log_parser_base create(text_reader reader, string settings) {
            
            if (reader is file_text_reader)
                return create_file_parser(reader as file_text_reader, settings);

            Debug.Assert(false);
            return null;
        }

        private static log_parser_base create_file_parser(file_text_reader reader, string sett) {
            string file_name = reader.name.ToLower();
            var all = new settings_as_string(sett);

            if ( file_name.EndsWith(".xml"))
                return new xml_file(reader, all);
            if ( file_name.EndsWith(".csv"))
                return new csv_file(reader, all);

            string syntax = all.get("syntax");
            if ( syntax == "" || syntax == find_log_syntax.UNKNOWN_SYNTAX)
                if ( text_file_part_on_single_line.is_single_line(reader.name, all))
                    return new text_file_part_on_single_line(reader, all);

            return new text_file_line_by_line(reader, all);
        }
    }
}
