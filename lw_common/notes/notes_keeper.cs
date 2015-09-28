using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace lw_common {
    /* what this does:
            - keeps all the notes the user makes (each log file matches a certain notes file)
            - makes sure that different hashes for the same file will point to the same notes file
              (for instance, the Fast- hash should point to the same file as the Slow- hash)

        This basically does nothing as long as you never change the way we deal with how md5 is computed (fast/slow/by-file-name).
        When you do change it, it makes sure we can still find the notes files for the new type of md5
        (since basically a different md5 computing method will result in a new md5)
    */
    public class notes_keeper {
        private static notes_keeper inst_ = new notes_keeper();

        public static notes_keeper inst {
            get { return inst_; }
        }

        private string dir_ = "";
        private settings_file sett_ = null;

        private Dictionary<string,string> md5_to_notes_file_ = new Dictionary<string, string>(); 
        private md5_log_keeper.md5_type prefer_md5_method_ = md5_log_keeper.md5_type.fast;

        public void init(string dir, md5_log_keeper.md5_type prefer) {
            // only call once!
            Debug.Assert(sett_ == null);
            try {
                Directory.CreateDirectory(dir);
            } catch {
            }

            dir_ = dir;
            prefer_md5_method_ = prefer;
            sett_ = new settings_file(dir + "\\notes.txt");

            int count = int.Parse(sett_.get("file_count", "0"));
            for (int i = 0; i < count; ++i) {
                string md5 = sett_.get("file." + i + ".md5");
                string file = sett_.get("file." + i + ".name");
                if ( md5 != "" && file != "")
                    md5_to_notes_file_.Add(md5, file);
            }
        }

        public string notes_file_for_file(string file) {
            var local = md5_log_keeper.inst.local_md5s_for_file(file);
            foreach ( string md5 in local)
                if (md5_to_notes_file_.ContainsKey(md5))
                    // we already know the notes-file for this specific file
                    return dir_ + "\\" + md5_to_notes_file_[md5];

            // it's a new file
            string file_md5 = md5_log_keeper.inst.get_md5_for_file(file, prefer_md5_method_);

            // reload the md5s - computing the md5 for this file might add another md5 to the local md5s
            // (when this md5 method was not used before)
            local = md5_log_keeper.inst.local_md5s_for_file(file);
            foreach ( string md5 in local)
                if (md5_to_notes_file_.ContainsKey(md5))
                    // we already know the notes-file for this specific file
                    return dir_ + "\\" + md5_to_notes_file_[md5];

            string guid = "{" + Guid.NewGuid().ToString() + "}.txt";
            md5_to_notes_file_.Add(file_md5, guid);

            // always add the fast method - just in case in the future we switch from slow to fast
            string md5_fast = md5_log_keeper.inst.get_md5_for_file(file, md5_log_keeper.md5_type.fast);
            if ( !md5_to_notes_file_.ContainsKey(md5_fast))
                md5_to_notes_file_.Add(md5_fast, guid);

            save();

            return dir_ + "\\" + guid;
        }

        private void save() {
            if (sett_ == null)
                return;

            sett_.set("file_count", "" + md5_to_notes_file_.Count);
            int idx = 0;
            foreach (var entry in md5_to_notes_file_) {
                sett_.set("file." + idx + ".md5", entry.Key);
                sett_.set("file." + idx + ".name", entry.Value);
                ++idx;
            }
            sett_.save();
        }



    }
}
