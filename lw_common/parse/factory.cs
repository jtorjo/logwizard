using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using lw_common.parse.parsers;
using lw_common.parse.syntaxes.file;
using LogWizard;

namespace lw_common.parse {
    public class factory {
        private const string V2_PREFIX = "[v2]";

        // these are the settings that are to be saved in the context
        static public string get_context_dependent_settings(text_reader reader, string settings) {
            if (reader is file_text_reader) {
                return new settings_as_string(settings).sub(new []{ "syntax"}).ToString();
            }

            return settings;
        }
        // these are the settings that are to be saved per-log
        static public string get_log_dependent_settings(text_reader reader, string settings) {
            return settings;
        }

        // note: log-dependent settings always override the context
        static public string merge_settings(text_reader reader, string context, string log) {
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

        private static log_parser_base create_file_parser(file_text_reader reader, string settings) {
            string file_name = reader.name;
            string syntax = get_single_setting(settings, "syntax");


            return new text_file_line_by_line(reader, new line_by_line_syntax() {line_syntax = syntax});
        }
    }
}
