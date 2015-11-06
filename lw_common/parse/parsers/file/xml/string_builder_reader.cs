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
