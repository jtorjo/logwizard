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
using System.IO;
using System.Diagnostics;
using System.Threading;
using LogWizard.readers;

#if old_code
namespace LogWizard
{
    /*
     1.0.14+ now, the file_text_reader can handle logs that are being appended to, and that are re-written

     1.0.41 made thread-safe

     1.0.72 the file_text_reader now has better memory footprint (or so it should :D)
    */
    class simple_file_text_reader2 : text_reader
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string file_;

        // for now, make it simple - read everything 
        string full_log_ = "";

        private ulong pos_ = 0;
        private ulong full_len_now_ = 0;

        private string cached_syntax_ = "";

        // note: these don't need to be thread-safe - only used in the read_all_file_thread
        private byte[] full_log_buffer_ = null;
        private ulong full_log_read_bytes_ = 0;

        private bool has_it_been_rewritten_ = false;

        private Encoding file_encoding_ = null;
        
        public simple_file_text_reader2(string file) {
            try {
                // get absolute path - normally, this should be the absolute path, but just to be sure
                file_ = new FileInfo(file).FullName;
            } catch {
                file_ = file;
            }
            pos_ = 0;
            new Thread(read_all_file_thread) {IsBackground = true}.Start();
        }

        private void read_all_file_thread() {
            while (!disposed) {
                Thread.Sleep(100);
                read_all_file();
            }            
        }

        public override string name {
            get { return file_; }
        }

        public override string read_next_text(int len) {
            lock (this) {
                if (pos_ > (ulong) full_log_.Length - 1)
                    pos_ = (ulong) full_log_.Length - 1;
                if (pos_ + (ulong) len > (ulong) full_log_.Length)
                    len = full_log_.Length - (int) pos_;
                Debug.Assert(len >= 0);
                string next = full_log_.Substring((int) pos_, len);
                pos_ += (ulong) len;
                return next;
            }
        }

        public override void compute_full_length() {
            lock (this)
                full_len_now_ = (ulong) full_log_.Length;
        }

        public override ulong full_len {
            get { lock(this) return full_len_now_; }
        }
        
        public override ulong pos {
            get { return pos_;  }
            set { pos_ = value; } 
        }

        private void allocate_buffer(long new_len) {
            long existing_len = full_log_buffer_ != null ? full_log_buffer_.Length : 0;
            if (new_len > existing_len) {
                // we should clearly allocate more - so that not at every write we would need to reallocate
                double STEP = 1.2;
                long MIN_SIZE = 16 * 1024;
                long resize = Math.Max( existing_len, MIN_SIZE); // at least a decent size
                while (resize < new_len)
                    resize = (long) (resize * STEP);

                byte[] new_ = new byte[resize];
                if ( full_log_buffer_ != null)
                    Array.Copy(full_log_buffer_, new_, (int)full_log_read_bytes_);
                full_log_buffer_ = new_;
            }
        }

        private void read_all_file() {
            try {
                if (file_encoding_ == null) {
                    var encoding = util.file_encoding(file_);
                    if ( encoding != null)
                        lock (this)
                            file_encoding_ = encoding;
                }

                long len = new FileInfo(file_).Length;
                allocate_buffer(len);
                using (var fs = new FileStream(file_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    if (len > (long)full_log_read_bytes_) {
                        long offset = (long)full_log_read_bytes_;
                        long count = (len - (long) full_log_read_bytes_);
                        logger.Debug("[file] reading file " + file_ + " at " + offset + ", " + count  + " bytes.");
                        fs.Seek(offset, SeekOrigin.Begin);
                        int read_bytes = fs.Read(full_log_buffer_, (int)offset, (int)count);
                        if ( read_bytes >= 0)
                            full_log_read_bytes_ += (ulong)read_bytes;
                        lock(this)
                            full_log_ = file_encoding_.GetString(full_log_buffer_, 0, (int)full_log_read_bytes_);
                    }
                else if ( len == (long)full_log_read_bytes_) {
                    // file not changed - nothing to do
                }
                else 
                    on_rewritten_file();
            } catch(Exception e) {
                logger.Error("[file] can't read file - " + file_ + " : " + e.Message);
            }
        }

        public override bool is_up_to_date() {
            try {
                long len = new FileInfo(file_).Length;
                lock (this)
                    return full_log_read_bytes_ == (ulong)len;
            } catch (Exception e) {
                return false;
            }
        }

        public override bool has_it_been_rewritten {
            get {
                bool has;
                lock (this) {
                    has = has_it_been_rewritten_;
                    has_it_been_rewritten_ = false;
                }
                return has;
            }
        }

        // file got rewritten from scratch - note: we preserve the log syntax
        private void on_rewritten_file() {
            logger.Info("[file] file rewritten - " + file_);
            lock (this) {
                has_it_been_rewritten_ = true;
                full_log_read_bytes_ = 0;
                // restart from beginning
                pos_ = 0;
            }
            read_all_file();
        }

        public override string try_to_find_log_syntax() {
            if (cached_syntax_ != "")
                return cached_syntax_;

            try {
                using (var fs = new FileStream(file_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    string found = new find_log_syntax().try_find_log_syntax(fs);
                    if (found != UNKNOWN_SYNTAX)
                        cached_syntax_ = found;
                    return found;
                }
            } catch {
                return UNKNOWN_SYNTAX;
            }
        }


    }
}
#endif
