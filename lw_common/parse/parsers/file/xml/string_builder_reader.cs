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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace lw_common.parse.parsers.file.xml {
    /* 
        the reason i have this: I want to constantly append to it, as the file grows (in case we go real-time)


        1.4.8k+ apparently, i don't need this, since I can use a parser context, and can constantly parse new fragments
    */
    public class string_builder_reader : TextReader 
    {
        private StringBuilder string_ = new StringBuilder(); 
        private int pos_;

        private int len {
            get { return string_.Length;  }
        }

        public StringBuilder raw_string {
            get { return string_; }
        }

        public int pos {
            get { return pos_; }
            set { pos_ = value; }
        }

        public string_builder_reader() {
        } 
 
        public override void Close() {
            Dispose(true);
        }
 
        protected override void Dispose(bool disposing) {
            string_.Clear(); 
            pos_ = 0; 
            base.Dispose(disposing); 
        }

        public void append(string s) {
            string_.Append(s);
        }

        public void prepend(string s) {
            string_.Insert(0, s);
        }

        public void clear() {
            string_.Clear();
            pos_ = 0;
        }


        public override int Peek() {
            if (pos_ == len) return -1;
            return string_[pos_];
        } 

        public override int Read() { 
            if (pos_ == len) return -1;
            return string_[pos_++]; 
        }
 
        public override int Read(char[] buffer, int index, int count) {
            Debug.Assert(buffer != null);
            Debug.Assert(index >= 0 && count >= 0);

            if (buffer.Length - index < count)
                throw new ArgumentException();

    		int n = len - pos_; 
            if (n > 0) { 
                if (n > count) n = count;
                string_.CopyTo(pos_, buffer, index, n); 
                pos_ += n;
            }
            return n;
        } 

        public override string ReadToEnd() 
        { 
            string s;
            if (pos_==0)
                s = string_.ToString();
            else 
                s = string_.ToString(pos_, len - pos_);
            pos_ = len; 
            return s; 
        }
 
        public override string ReadLine() { 
            int i = pos_;
            while (i < len) {
                char ch = string_[i];
                if (ch == '\r' || ch == '\n') { 
                    var result = string_.ToString(pos_, i - pos_);
                    pos_ = i + 1; 
                    if (ch == '\r' && pos_ < len && string_[pos_] == '\n') pos_++; 
                    return result;
                } 
                i++;
            }
            if (i > pos_) {
                var result = string_.ToString(pos_, i - pos_); 
                pos_ = i;
                return result; 
            } 
            return null;
        } 
    }
}
