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
using LogWizard.readers;

namespace LogWizard {
    // moved here on 1.0.14 - this was the simple way to read all file
    // now, the file_text_reader can handle logs that are being appended to, and that are re-written
    class simple_file_text_reader : text_reader {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly string file;

        FileStream fs = null;

        // for now, make it simple - read everything 
        string full_log = "";

        private ulong pos_ = 0;


        private string cached_syntax_ = "";

        public simple_file_text_reader(string file) {
            this.file = file;
            try {
                fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            } catch { fs = null; }
            pos_ = 0;
            read_all_file();
        }

        public override string read_next_text(int len) {
            if ( pos_ > (ulong)full_log.Length - 1)
                pos_ = (ulong)full_log.Length - 1;
            if ( pos_ + (ulong)len > (ulong)full_log.Length)
                len = full_log.Length - (int)pos_;
            string next = full_log.Substring((int)pos_, len);
            pos_ += (ulong)len;
            return next;
        }

        public override void compute_full_length() {
        }

        public override ulong full_len {
            // not tested
            get { return (ulong) full_log.Length; }
        }

        //public override bool has_more_text() { return pos_ < (ulong)full_log.Length - 1; }
        public override ulong pos {
            get { return pos_;  }
            set { pos_ = value; } 
        }

        private void read_all_file() {
            logger.Debug("reading file " + file);
            try {
                int len = (int)new FileInfo(file).Length;
                fs.Seek(0, SeekOrigin.Begin);
            
                // read a few lines from the beginning
                byte[] readBuffer = new byte[len];
                int bytes = fs.Read(readBuffer, 0, len);
                full_log = System.Text.Encoding.Default.GetString(readBuffer, 0, bytes);
            } catch {
            }
        }

        public override string try_to_find_log_syntax() {
            if (cached_syntax_ != "")
                return cached_syntax_;

            string found = new find_log_syntax().try_find_log_syntax(fs);
            if (found != UNKNOWN_SYNTAX)
                cached_syntax_ = found;
            return found;
        }
    }
}
