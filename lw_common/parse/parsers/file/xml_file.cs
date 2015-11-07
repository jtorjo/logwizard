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
using System.Windows.Forms.VisualStyles;
using System.Xml;
using lw_common.parse.parsers.file;
using lw_common.parse.parsers.file.xml;
using LogWizard;
using MultiLanguage;

namespace lw_common.parse.parsers {
    class xml_file : file_parser_base {
        // wait to read at least something meaningful
        private const int MIN_LEN = 512;

        string_builder_reader xml_reader_ = new string_builder_reader();
        private XmlTextReader xml_text_reader_ = null;

        private string delimeter_name_ = "";
        private int last_valid_pos_ = 0;

        public xml_file(file_text_reader reader, settings_as_string sett) : base(reader, sett) {
        }

        protected override void on_new_lines(string next) {
            XmlTextReader text_reader;
            lock (this) {
                xml_reader_.append(next);
                if (xml_reader_.raw_string.Length < MIN_LEN)
                    return;

                xml_reader_.pos = last_valid_pos_;

                // make sure we mark this file as XML
                string prefix = "<?xml version=";
                string full_prefix = "<?xml version='1.0' ?>";
                if (xml_reader_.raw_string.ToString(0, prefix.Length) != prefix)
                    xml_reader_.prepend(full_prefix);

                if ( xml_text_reader_ == null)
                    xml_text_reader_ = new XmlTextReader(xml_reader_);
                text_reader = xml_text_reader_;
            }

            // FIXME read all attributes , and save them as name.attr_name ; if name contains "xxx:", ignore that
            // timestamp -> date + time
            log_entry_line entry = new log_entry_line();
            while (text_reader.Read()) {
                if (text_reader.NodeType == XmlNodeType.Element) {
                    string name = text_reader.Name;
                    string text = text_reader.ReadString();
                    bool contains_already = entry.names.Contains(name);
                    bool entry_fully_read = contains_already || name == delimeter_name_;
                    if (!entry_fully_read) {
                        if ( text != "" && text != null)
                            entry.add(name, text);
                    } else {
                        delimeter_name_ = name;
                        // we read a full object
                        lock (this) {
                            entries_.Add(entry);
                            string_.add_preparsed_line(entry.ToString());
                        }
                        last_valid_pos_ = xml_reader_.pos;
                        entry = new log_entry_line();
                    }
                }
            }
            // FIXME not sure?
            // ???????? text_reader.ResetState();
        }


        public override void force_reload() {
            base.force_reload();
            lock (this) {
                xml_reader_.clear();
                last_valid_pos_ = 0;
                xml_text_reader_ = null;
                delimeter_name_ = "";
            }
        }
    }
}
