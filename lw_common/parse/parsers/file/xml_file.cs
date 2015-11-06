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
using lw_common.parse.parsers.file.xml;
using LogWizard;
using MultiLanguage;

namespace lw_common.parse.parsers {
    class xml_file : log_parser_base {
        // wait to read at least something meaningful
        private const int MIN_LEN = 512;

        private file_text_reader reader_;

        private settings_as_string sett_;
        private aliases aliases_;

        string_builder_reader xml_reader_ = new string_builder_reader();
        private XmlTextReader xml_text_reader_ = null;

        // this contains the full string
        private large_string string_ = new large_string();

        private memory_optimized_list<log_entry_line> entries_ = new memory_optimized_list<log_entry_line>() { name = "parser-entries-xml"};

        private bool up_to_date_ = false;
        private string delimeter_name_ = "";

        private int last_valid_pos_ = 0;
        private List<string> column_names_ = new List<string>(); 

        public xml_file(file_text_reader reader, settings_as_string sett) {
            sett_ = sett;
            reader_ = reader;
            read_settings();
        }
        public override void on_settings_changed(string settings) {
            sett_ = new settings_as_string(settings);
            read_settings();
            // FIXME
        }

        private void read_settings() {            
            aliases_ = new aliases(sett_.get("aliases"));
        }

        public override void read_to_end() {

            ulong old_len = reader_.full_len;
            reader_.compute_full_length();
            ulong new_len = reader_.full_len;
            // when reader's position is zero -> it's either the first time, or file was re-rewritten
            if (old_len > new_len || reader_.pos == 0) 
                // file got re-written
                force_reload();

            bool fully_read = old_len == new_len && reader_.is_up_to_date();

            if ( !reader_.has_more_cached_text()) {
                lock (this) 
                    up_to_date_ = fully_read;
                return;
            }
            
            lock (this)
                up_to_date_ = false;

            string next = reader_.read_next_text();
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
            text_reader.ResetState();
        }

        public override int line_count {
            get { lock(this) return entries_.Count; }
        }

        public override line line_at(int idx) {
            lock (this) {
                if (idx < entries_.Count) {
                    var entry = entries_[idx];
                    var l = new line( new sub_string(string_, idx), entry.idx_in_line(aliases_)  );
                    return l;
                } else {
                    // this can happen, when the log has been re-written, and everything is being refreshed
                    throw new line.exception("invalid line request " + idx + " / " + entries_.Count);
                }
            }
        }

        public override void force_reload() {
            lock (this) {
                xml_reader_.clear();
                last_valid_pos_ = 0;
                xml_text_reader_ = null;
                delimeter_name_ = "";
                string_.clear();
            }
        }

        public override bool up_to_date {
            get { return up_to_date_; }
        }
    }
}
