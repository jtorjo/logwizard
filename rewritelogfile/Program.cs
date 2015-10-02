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
using System.Threading;

namespace rewritelogfile {
    internal class Program {
        private static void Main(string[] args) {
            if (args.Length > 0) {
                string source_file = args[0];
                string dest_file = source_file + ".rewritten.log";
                int write_lines = args.Length > 1 ? int.Parse(args[1]) : 100;
                int wait_secs = args.Length > 2 ? int.Parse(args[2]) : 0;
                rewrite_file(source_file, dest_file, write_lines, wait_secs);
            }
        }

        private static void rewrite_file(string source, string dest, int write_lines, int wait_secs) {
            if (!File.Exists(source))
                return;
            var lines = File.ReadAllLines(source).ToList();
            for (int i = 0; i < lines.Count; i += write_lines) {
                int write_now = i + write_lines < lines.Count ? write_lines : lines.Count - i;
                Console.WriteLine("writing " + i + " , " + write_now + " lines");
                if ( i == 0)
                    File.WriteAllLines(dest, lines.GetRange(i, write_now));
                else 
                    File.AppendAllLines(dest, lines.GetRange(i, write_now));
                if (wait_secs > 0)
                    Thread.Sleep(wait_secs);
                else
                    Console.ReadLine();
            }

        }







#if old_code
    /*  takes a file, and rewrites it over and over, writing a certain amount of text at a certain time interval.

            we're doing this to test that LogWizard updates correctly:
            * once more info is written to the file
            * once the file gets re-written

        Arguments:
            1. the file to use as source (the target = source_file.rewritten.log)
            2. how much to write each time (default = 50K)
            3. how much to wait after each write (default = 1000ms)
            4. how much to wait after a complete write (default = 10000ms)
        */

        static void Main(string[] args) {
            if (args.Length > 0) {
                string source_file = args[0];
                string dest_file = source_file + ".rewritten.log";
                int write_bytes = args.Length > 1 ? int.Parse(args[1]) : 50 * 1024;
                int wait_after_each_write_ms = args.Length > 2 ? int.Parse(args[2]) : 1000;
                int wait_after_full_write_ms = args.Length > 3 ? int.Parse(args[3]) : 10000;
                rewrite_file(source_file, dest_file, write_bytes, wait_after_each_write_ms, wait_after_full_write_ms);
            }
        }

        private static void rewrite_file(string source, string dest, int write_bytes, int wait_after_each_write_ms, int wait_after_full_write_ms) {
            if (!File.Exists(source))
                return;

            string text = File.ReadAllText(source);
            bool needs_create = true;
            int idx = 0;
            while (true) {
                int part_len = idx + write_bytes <= text.Length ? write_bytes : text.Length - idx;
                string part = text.Substring(idx, part_len);
                Console.WriteLine("writing " + idx + " -> " + part_len);
                idx += part_len;
                if (needs_create)
                    File.WriteAllText(dest, part);
                else 
                    File.AppendAllText(dest, part);
                needs_create = false;
                bool at_end = idx >= text.Length;
                if (at_end) {
                    Thread.Sleep(wait_after_full_write_ms);
                    needs_create = true;
                    idx = 0;
                }
                else 
                    Thread.Sleep(wait_after_each_write_ms);
            }
        }
#endif
    }
}
