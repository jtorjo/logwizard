using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace lw_common {
    // keeps the different types of MD5s for each of our logs
    public class md5_log_keeper {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static md5_log_keeper inst_ = new md5_log_keeper();
        public static md5_log_keeper inst {
            get { return inst_; }
        }

        public enum md5_type {
            // fast: last 8192 + first 8192 chars, + file name + file size
            fast, 
            // slow: 100% accurate, md5 of all file
            slow,             
            // by file name - for files that could change
            by_file_name
        }

        // note: each of these md5s - in the name itself, identify which type of md5 it is
        private class md5_info {
            public string fast = "";
            public string slow = "";
            public string by_file_name = "";

            public string by_md5(md5_type type) {
                switch (type) {
                case md5_type.fast:
                    return fast;
                case md5_type.slow:
                    return slow;
                case md5_type.by_file_name:
                    return by_file_name;
                default:
                    Debug.Assert(false);
                    return "";
                }
            }
        }

        private Dictionary<string, md5_info> file_md5_ = new Dictionary<string, md5_info>();

        public List<string> local_md5s_for_file(string file) {
            file = new FileInfo(file).FullName;
            if (!file_md5_.ContainsKey(file))
                compute_default_md5s_for_file(file);

            md5_info md5 = file_md5_[file];
            List<string> result = new List<string>();
            if (md5.fast != "")
                result.Add(md5.fast);
            if (md5.slow != "")
                result.Add(md5.slow);
            if (md5.by_file_name != "")
                result.Add(md5.by_file_name);
            return result;
        }

        public string get_md5_for_file(string file, md5_type type) {
            file = new FileInfo(file).FullName;
            if (!file_md5_.ContainsKey(file))
                compute_default_md5s_for_file(file);

            if (file_md5_[file].by_md5(type) == "")
                compute_md5_for_file(file, type);

            Debug.Assert( file_md5_[file].by_md5(type) != "");
            return file_md5_[file].by_md5(type);
        }


        // finds all files matching the given MD5s
        //
        // you will call this with all the MD5s received from another user's notes
        public List<string> find_files_with_md5(string[] md5s) {
            Dictionary<string, List<string>> found = new Dictionary<string, List<string>>();
            foreach (string search_md5 in md5s)
                if (search_md5 != "") {
                    found.Add(search_md5, new List<string>());
                    foreach (var md5 in file_md5_)
                        // 1.1.5+ - until i properly think the "by_file_name" through, ignore it
                        //         I think by_file_name should be context dependent
                        if (md5.Value.fast == search_md5 || md5.Value.slow == search_md5) // || md5.Value.by_file_name == search_md5)
                            found[search_md5].Add(md5.Key);

                    if (found[search_md5].Count == 0)
                        // we found no files matching this md5
                        found.Remove(search_md5);
                }

            // at this point, we can have several files matching the md5s - take an intersection of them
            if (found.Count > 0) {
                List<string> result = found.Values.First();
                foreach (var files in found)
                    result = result.Intersect(files.Value).ToList();
                return result;
            } else
                return new List<string>();
        }

        public void compute_default_md5s_for_file(string file) {
            compute_md5_for_file(file, md5_type.by_file_name);
            compute_md5_for_file(file, md5_type.fast);

            // testing
            //if ( util.is_debug)
            //  compute_md5_for_file(file, md5_type.slow);
        }

        public void compute_md5_for_file(string file, md5_type type) {
            file = new FileInfo(file).FullName;

            if (!file_md5_.ContainsKey(file))
                file_md5_.Add(file, new md5_info());
            md5_info md5 = file_md5_[file];

            switch (type) {
            case md5_type.fast:
                if ( md5.fast == "")
                    md5.fast = compute_md5_for_file_fast(file);
                logger.Debug("[md5] computed md5 for " + file + " -> " + md5.fast);
                break;
            case md5_type.slow:
                if (md5.slow == "")
                    md5.slow = compute_md5_for_file_slow(file);
                logger.Debug("[md5] computed md5 for " + file + " -> " + md5.slow);
                break;
            case md5_type.by_file_name:
                md5.by_file_name = compute_md5_for_file_filename(file);
                logger.Debug("[md5] computed md5 for " + file + " -> " + md5.by_file_name);
                break;
            default:
                Debug.Assert(false);
                break;
            }
        }

        private string compute_md5_for_file_fast(string file) {
            // fast: last 8192 + first 8192 chars, + file name + file size
            const int block = 16834;
            try {
                long size = new FileInfo(file).Length;

                int buff_size = Math.Min(block * 2, (int) size);
                int start_block = buff_size >= block * 2 ? block : (int) size;
                int end_block = buff_size >= block * 2 ? block : 0;

                byte[] buff = new byte[buff_size];

                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek(0, SeekOrigin.Begin);
                int read_block = fs.Read(buff, 0, start_block);
                bool all_good = false;
                if (read_block == start_block) {
                    if (end_block > 0) {
                        fs.Seek(size - end_block, SeekOrigin.Begin);
                        read_block = fs.Read(buff, start_block, end_block);
                        if (read_block == end_block)
                            all_good = true;
                    } else
                        all_good = true;
                }

                if (!all_good) {
                    logger.Error("[md5] can't compute md5-fast for " + file + " - could not read from it");
                    return "";
                }

                // 1.1.5c -IMPORTANT: don't include the file name, since our file name might be different
                //         (for instance, say you send the file to someone, and he renames it)
                string md5 = util.md5_hash(buff) + "-" + size ;
                return "Fast-" + md5;
            } catch (Exception e) {
                logger.Error("[md5] can't compute md5-fast for " + file + " : " + e.Message);
                return "";
            }
        }

        private string compute_md5_for_file_slow(string file) {
            const long block = 4 * 1024 * 1024;
            string md5_blocks = "";
            try {
                long size = new FileInfo(file).Length;

                byte[] buff = new byte[block];
                long remaining = size;

                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek(0, SeekOrigin.Begin);

                while (remaining > 0) {
                    long cur_block_size = Math.Min(block, remaining);
                    if (cur_block_size < block)
                        buff = new byte[block]; // last block
                    int read_block = fs.Read(buff, 0, (int) cur_block_size);
                    if (read_block == (int) cur_block_size)
                        md5_blocks += util.md5_hash(buff);
                    else
                        logger.Error("[md5] did not read what we expected from " + file + ", expected=" + cur_block_size + ", read=" + read_block);

                    remaining -= cur_block_size;
                }

                string md5 = util.md5_hash(md5_blocks);
                return "Slow-" + md5;
            } catch (Exception e) {
                logger.Error("[md5] can't compute md5-fast for " + file + " : " + e.Message);
                return "";
            }
        }

        private string compute_md5_for_file_filename(string file) {
            return "Name-" + new FileInfo(file).Name;
        }
    }
}
