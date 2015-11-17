using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace lw_common.readers.entry {
    /* inspiration: 
            http://www.codeproject.com/Articles/23776/Mechanism-of-OutputDebugString
            https://sumeshvv.wordpress.com/2010/12/02/how-the-outputdebugstring-api-works/
            http://www.unixwiz.net/techtips/outputdebugstring.html
            http://blogs.msdn.com/b/reiley/archive/2011/07/30/a-debugging-approach-to-outputdebugstring.aspx
            http://stackoverflow.com/questions/6384785/how-can-i-receive-outputdebugstring-from-service
            http://www.michaelfcollins3.me/blog/2013/06/01/understanding-outputdebugstring.html

    */
    class capture_all_debug_events : IDisposable {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool disposed_ = false;
        private bool use_global_ = false;

        MemoryMappedFile memory_file_ = null;
        EventWaitHandle buffer_ready_ = null;
        EventWaitHandle data_ready_ = null;

        private error_list_keeper errors_ = new error_list_keeper();

        public class debug_event {
            public int unique_id = 0;
            public int process_id = 0;
            // application name (in lower-case)
            public string lo_process_name = "";
            public string msg = "";
            public DateTime date = DateTime.Now;
        }
        // 1.5.6 - at this time, I keep all events forever
        //         it's a sorted list, by unique id
        private memory_optimized_list<debug_event> events_ = new memory_optimized_list<debug_event>();
        private int next_unique_id = 0;

        private Thread read_thread_ = null;

        // conver PIDs to user-friendly name
        private Dictionary<int, string> pid_to_name_ = new Dictionary<int, string>(); 

        private static readonly capture_all_debug_events capture_global_ = new capture_all_debug_events() {use_global_ = true};
        private static readonly capture_all_debug_events capture_local_ = new capture_all_debug_events() {use_global_ = false};

        private capture_all_debug_events() {
            
        }

        public error_list_keeper errors {
            get { return errors_; }
        }

        public static capture_all_debug_events capture_global {
            get { return capture_global_; }
        }

        public static capture_all_debug_events capture_local {
            get { return capture_local_; }
        }


        public List<debug_event> get_events(int least_id) {
            if (read_thread_ == null) {
                read_thread_ = new Thread(read_events_thread) { IsBackground = true};
                read_thread_.Start();
            }

            lock (this) {
                if (events_.Count > 0) 
                    Debug.Assert(events_.Last().unique_id == events_.Count - 1);
                if (least_id < events_.Count)
                    return events_.GetRange(least_id, events_.Count - least_id);
                else 
                    return new List<debug_event>();
            }
        }


        private void read_events_thread() {

            string prefix = use_global_ ? "Global\\" : "";
            try {
                memory_file_ = MemoryMappedFile.CreateNew(prefix + "DBWIN_BUFFER", 4096L);

                bool created = false;
                buffer_ready_ = new EventWaitHandle( false, EventResetMode.AutoReset, prefix + "DBWIN_BUFFER_READY", out created);
                if (!created) 
                    errors_.add("Can't create the DBWIN_BUFFER_READY event/" + use_global_);

                if (created) {
                    data_ready_ = new EventWaitHandle(false, EventResetMode.AutoReset, prefix + "DBWIN_DATA_READY", out created);
                    if (!created) 
                        errors_.add("Can't create the DBWIN_DATA_READY event/" + use_global_);
                }

                if (created) {
                    buffer_ready_.Set();
                    while (!disposed_) {
                        if (!data_ready_.WaitOne(1000))
                            continue;

                        using (var stream = memory_file_.CreateViewStream()) {
                            using (var reader = new BinaryReader(stream, Encoding.Default)) {
                                var process_id = (int)reader.ReadUInt32();
                                var raw = reader.ReadChars(4092);
                                var idx = Array.IndexOf(raw, '\0');
                                var msg = new string(raw, 0, idx);
                                find_process_id(process_id);
                                string process_name = pid_to_name_.ContainsKey(process_id) ? pid_to_name_[process_id] : "";
                                lock (this)
                                    events_.Add(new debug_event {
                                        unique_id = next_unique_id++, process_id = process_id, msg = msg, lo_process_name = process_name
                                    });
                            }
                        }
                
                        buffer_ready_.Set();
                    }
                }
            } catch (Exception e) {
                logger.Fatal("Can't read debug events " + e.Message);
                errors_.add("Error reading debug events " + e.Message);
            }

            if ( memory_file_ != null)
                memory_file_.Dispose();
            if ( data_ready_ != null)
                data_ready_.Dispose();
            if ( buffer_ready_ != null)
                buffer_ready_.Dispose();

        }

        private void find_process_id(int pid) {
            if (pid_to_name_.ContainsKey(pid))
                return;
            try {
                string name = Process.GetProcessById(pid).ProcessName;
                pid_to_name_.Add(pid, name.ToLower());
            } catch {
                // could not get the process name
            }
        }

        public void Dispose() {
            disposed_ = true;
        }
    }
}
