/* 
 * Copyright (C) 2014-2016 John Torjo
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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using log4net.Repository.Hierarchy;
using lw_common.parse.parsers.file;
using lw_common.parse.parsers.file.xml;
using LogWizard;
using MultiLanguage;

namespace lw_common.parse.parsers {
    class xml_file : file_parser_base {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // wait to read at least something meaningful
        private const int MIN_LEN = 512;

        private XmlParserContext xml_parse_context_ = null;

        private string user_set_delimeter_name_ = "";
        // if true, for the xml attributes from the delimeter entry, when computing OUR entry name, we only use the attribute name 
        private bool ignore_delimeter_name_on_log_entry_ = true;

        private string delimeter_name_ = "";
        private string last_ = "";

        public xml_file(file_text_reader reader) : base(reader) {
            XmlNamespaceManager mngr = new XmlNamespaceManager( new NameTable() );
            mngr.AddNamespace( "xsi", "http://www.w3.org/2001/XMLSchema-instance" );
            mngr.AddNamespace( "xsd", "http://www.w3.org/2001/XMLSchema" );
            xml_parse_context_ = new XmlParserContext( null, mngr, null, XmlSpace.None );
        }

        protected override void on_updated_settings() {
            base.on_updated_settings();
            // user can for what entry to look for!
            user_set_delimeter_name_ = sett_.xml_delimiter;
            if ( user_set_delimeter_name_ != "")
                lock (this)
                    delimeter_name_ = user_set_delimeter_name_;
        }

        // for now, I assume an XML has multi-line columns
        public override bool has_multi_line_columns {
            get { return true; }
        }

        protected override void on_new_lines(string next) {
            string now = "";
            string delimeter;
            bool needs_set_column_names;
            lock (this) {
                last_ += next;
                if (last_.Length < MIN_LEN && delimeter_name_ == "")
                    return;
                last_ = last_.TrimStart();

                if (delimeter_name_ == "") {
                    if (last_.StartsWith("<?xml ")) {
                        // we need to ignore xml prefix when searching for delimeter
                        int ignore = last_.IndexOf(">");
                        last_ = last_.Substring(ignore + 1).TrimStart();
                    }

                    int delimeter_idx = last_.IndexOfAny( new []{'>',' ', '\n', '\r', '\t'});
                    delimeter_name_ = last_.Substring(1, delimeter_idx - 1);
                    logger.Debug("[parse] parsing xml by " + delimeter_name_);
                }

                string end = "/" + delimeter_name_;
                int last_idx = last_.LastIndexOf(end);
                if (last_idx >= 0) {
                    // we can fully parse at least one entry
                    int xml_end = last_.IndexOf('>', last_idx);
                    if (xml_end > 0) {
                        now = last_.Substring(0, xml_end + 1);
                        last_ = last_.Substring(xml_end + 1);
                    }
                }

                if ( now == "")
                    // there's not enought text to parse a single log entry
                    return;
                delimeter = delimeter_name_;
                needs_set_column_names = column_names.Count < 1;
            }
            
            XmlTextReader reader = new XmlTextReader(now, XmlNodeType.Element, xml_parse_context_) { Namespaces = false };
            // FIXME read all attributes , and save them as name.attr_name ; if name contains "xxx:", ignore that
            // timestamp -> date + time
            log_entry_line entry = new log_entry_line();
            string last_element = "";
            List<string> column_names_now = null;
            try {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        string element_name = reader.Name;
                        last_element = simple_element_name(element_name);

                        // read all its attributes                    
                        for (int i = 0; i < reader.AttributeCount; ++i) {
                            reader.MoveToAttribute(i);
                            string name = last_element + "." + reader.Name;
                            string text = (reader.Value ?? "").Trim();

                            if (ignore_delimeter_name_on_log_entry_)
                                if (element_name == delimeter)
                                    name = reader.Name;
                            entry.analyze_and_add(name, text);
                        }
                    } else if (reader.NodeType == XmlNodeType.Text) {
                        Debug.Assert(last_element != "");
                        string text = (reader.Value ?? "").Trim();
                        entry.analyze_and_add(last_element, text);
                    } else if (reader.NodeType == XmlNodeType.EndElement) {
                        if (reader.Name == delimeter) {
                            // we read a full object
                            if (needs_set_column_names && column_names_now == null)
                                column_names_now = entry.names;
                            lock (this) {
                                entries_.Add(entry);
                                string_.add_preparsed_line(entry.ToString());
                            }
                            entry = new log_entry_line();
                            last_element = "";
                        }
                    }
                }

                if ( column_names_now != null)
                    column_names = column_names_now;
            } catch (Exception e) {
                logger.Fatal("[parse] could not parse xml: " + e);
            }
        }




        private string simple_element_name(string name) {
            int idx = name.IndexOf(':');
            if (idx >= 0)
                name = name.Substring(idx + 1);
            return name;
        }

        public override void force_reload() {
            base.force_reload();
            lock (this) {
                delimeter_name_ = user_set_delimeter_name_;
                last_ = "";
            }
        }
    }
}
