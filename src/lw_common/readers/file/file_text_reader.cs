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
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace lw_common
{
    /*
    1.0.14+ now, the file_text_reader can handle logs that are being appended to, and that are re-written

    1.0.41 made thread-safe

    1.0.72
    - decrease memory footprint
    - very important assumption: we always assume that when the file is "encoding-complete" - in other words,
      we never end up in the scenario where a character (that can occupy several bytes) is not fully written into the file
    */
    public class file_text_reader : file_text_reader_base
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string file_;

        // 1.0.72+ note: for now, assume the encoding is 4-padded, and we never end up splitting a char in the middle
        private readonly ulong MAX_READ_IN_ONE_GO_UNTIL_FULLY_READ = app.inst.no_ui.file_max_read_in_one_go;
        // ... after the file has been fully read, we don't expect that much info to be written in a few milliseconds
        //     thus, make the muffer MUCH smaller
        private readonly ulong MAX_READ_IN_ONE_GO_AFTER_FULLY_READ = app.inst.no_ui.file_max_read_in_one_go_after_fully_read;

        // if our kept string exceeds this, stop reading from the file - let the parser first read this, then continue
        private readonly ulong MAX_STRING_SIZE = 32 * 1024 * 1024;

        private byte[] buffer_ = null;

        private StringBuilder last_part_ = new StringBuilder();

        private ulong full_len_now_ = 0;

        private ulong offset_ = 0;
        private ulong read_byte_count_ = 0;

        private string cached_syntax_ = "";

        private bool has_it_been_rewritten_ = false;
        // useful for using a smaller buffer for reading 
        private bool fully_read_once_ = false;

        private Encoding file_encoding_ = null;

        private DateTime cannot_read_until_this_time_ = DateTime.MinValue;
        
        public file_text_reader(string file) : this(new log_settings_string("name=" + file)) {
        }

        public file_text_reader(log_settings_string sett) : base(sett) {
            string file = sett.name;
            buffer_ = new byte[max_read_in_one_go];
            try {
                // get absolute path - normally, this should be the absolute path, but just to be sure
                file_ = new FileInfo(file).FullName;
            } catch {
                file_ = file;
            }

            var thread = app.inst.use_file_monitoring_api ? new Thread(read_all_file_thread_file_monitoring_api) {IsBackground = true}
                                                          : new Thread(read_all_file_thread) {IsBackground = true};
            thread.Start();
        }

        private ulong max_read_in_one_go {
            get { lock (this) return fully_read_once_ ? MAX_READ_IN_ONE_GO_AFTER_FULLY_READ : MAX_READ_IN_ONE_GO_UNTIL_FULLY_READ; }
        }

        private void read_all_file_thread() {
            while (!disposed) {
                Thread.Sleep( app.inst.check_new_lines_interval_ms);
                read_file_block();
            }
        }

        private void read_all_file_thread_file_monitoring_api() {
            string dir = Path.GetDirectoryName(file_);
            var monitor = win32.FindFirstChangeNotification(dir, false, win32.FILE_NOTIFY_CHANGE_SIZE | win32.FILE_NOTIFY_CHANGE_FILE_NAME);
            if (monitor == IntPtr.Zero) {
                logger.Error("[reader] can't monitor file " + file_ );
                read_all_file_thread();
                return;
            }

            // we need to first read it all
            while (!disposed && !fully_read_once_) {
                Thread.Sleep( app.inst.check_new_lines_interval_ms);
                read_file_block();
            }

            while (!disposed) {
                bool change = win32.WaitForSingleObject(monitor, 1000) == win32.WAIT_OBJECT_0;
                if (change) {
                    logger.Info("[reader] file api change - " + file_);
                    read_file_block();
                    win32.FindNextChangeNotification(monitor);
                }
            }
        }

        // this should read all text, returns it, and reset our buffer - len is not needed
        public override string read_next_text() {
            lock (this) {
                string now = last_part_.ToString();
                //last_part_ = new StringBuilder();
                // IMPORTANT : we don't want to create a new object - we want the .Capacity to stay the same
                last_part_.Clear();
                offset_ = read_byte_count_;
                return now;
            }
        }

        public override bool has_more_cached_text() {
            lock (this)
                return last_part_.Length > 0;
        }

        public override void compute_full_length() {
            lock(this)
                if (last_part_.Length >= (long)MAX_STRING_SIZE)
                    return;
            ulong full;
            try {
                full = (ulong)new FileInfo(file_).Length;
            } catch (FileNotFoundException) {
                full = 0;
            }
            catch(Exception) {
                // if we can't read the file length, something probably happened - either it got re-written, or locked
                // wait until next time
                return;
            }

            lock (this) 
                full_len_now_ = Math.Min( read_byte_count_ + max_read_in_one_go, full); 
        }

        public override ulong full_len {
            get { lock(this) return full_len_now_; }
        }

        public override ulong try_guess_full_len {
            get {
                try {
                    return (ulong)new FileInfo(file_).Length;
                } catch (FileNotFoundException) {
                    return ulong.MaxValue;
                }                
            }
        }

        public override ulong pos {
            get { lock(this) return offset_;  }
        }

        public override bool fully_read_once {
            get {
                lock(this) return fully_read_once_; 
            }
        }

        public override void force_reload(string reason) {
            lock (this) 
                if (offset_ == 0)
                    // we're already at beginning of file
                    return;
            on_rewritten_file();
        }

        private void update_buffer() {
            int max_read = (int) max_read_in_one_go;
            lock (this) {
                if (buffer_.Length == max_read)
                    return;
                buffer_ = new byte[max_read];
            }
            // we just released the old buffer
            GC.Collect();
        }

        // reads a file block from the file (as much as we can, in a single go)
        private void read_file_block() {
            if (DateTime.Now < cannot_read_until_this_time_)
                return;

            if (file_.EndsWith(".evtx")) {
                errors_.add("Donot add Event Log files like this. Please open via Ctrl-O, and select 'Windows Event Log' as the type.");
                cannot_read_until_this_time_ = DateTime.MaxValue;
                return;
            }

            try {
                if (file_encoding_ == null) {
                    var encoding = util.file_encoding(file_);
                    if (encoding != null)
                        lock (this)
                            file_encoding_ = encoding;
                }

                lock (this)
                    if (last_part_.Length >= (long) MAX_STRING_SIZE)
                        return;

                long len = new FileInfo(file_).Length;
                if (len == 0 && util.is_debug)
                    // when testing, I can manually add lines to a log - if in notepad++, this will temporarily set file length to zero
                    return;

                bool file_rewritten = false;
                long offset;
                lock (this) offset = (long) read_byte_count_;
                bool genenerate_new_lines_event = false;
                using (var fs = new FileStream(file_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    if (len > offset) {
                        update_buffer();
                        long read_now = Math.Min((long) max_read_in_one_go, len - offset);
                        logger.Debug("[file] reading file " + file_ + " at " + offset + ", " + read_now + " bytes.");
                        fs.Seek(offset, SeekOrigin.Begin);
                        int read_bytes = fs.Read(buffer_, 0, (int) read_now);
                        if (read_bytes >= 0) {
                            string now = file_encoding_.GetString(buffer_, 0, read_bytes);
                            lock (this) {
                                read_byte_count_ += (ulong) read_bytes;
                                last_part_.Append(now);
                                genenerate_new_lines_event = fully_read_once_ && parser != null;
                            }
                        }
                    } else if (len == offset) {
                        // file not changed - nothing to do
                        bool fully_read_now;
                        lock (this) {
                            fully_read_now = !fully_read_once_;
                            fully_read_once_ = true;
                        }
                        if (fully_read_now)
                            update_buffer();
                    } else
                        file_rewritten = true;

                if (file_rewritten) {
                    on_rewritten_file();
                    genenerate_new_lines_event = true;
                }

                if (genenerate_new_lines_event)
                    parser.on_log_has_new_lines(file_rewritten);
            } catch (FileNotFoundException) {
                // file may have been erased
                if (read_byte_count_ > 0) {
                    on_rewritten_file();
                    lock (this)
                        fully_read_once_ = true;
                    parser.on_log_has_new_lines(true);
                }
            } catch (UnauthorizedAccessException) {
                errors_.clear();
                errors_.add("Access Denied: " + file_ + " at " + DateTime.Now.ToString("HH:mm:ss") + ". Waiting 5 seconds");
                logger.Error("access denied " + file_);
                cannot_read_until_this_time_ = DateTime.Now.AddSeconds(5);
            } catch (Exception e) {
                logger.Error("[file] can't read file - " + file_ + " : " + e.Message);
            }
        }

        public override bool is_up_to_date() {
            try {
                long len = new FileInfo(file_).Length;
                lock (this)
                    return read_byte_count_ == (ulong) len;
            } catch (FileNotFoundException) {
                // file may have been erased
                lock (this)
                    return read_byte_count_ == 0;
            }             
            catch (Exception) {
                // in this case, maybe the file is locked - we'll try again next time
                return true;
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
                // restart from beginning
                has_it_been_rewritten_ = true;
                read_byte_count_ = 0;
                offset_ = 0;
                fully_read_once_ = false;
            }
            read_file_block();
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
