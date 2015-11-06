using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common.parse {
    // represents a "conceptual" line - contains a log entry, but it could be on several lines
    // however, we will compact it on a single line
    class log_entry_line {

        public void add(info_type info, string part) {
            infos_.Add(info, entry_.Length);
            entry_ += part;
        }

        public override string ToString() {
            return entry_;
        }

        Tuple<int, int>[] idx_in_line() {
            var idx  = new Tuple<int, int>[(int)info_type.max];
            for ( int i = 0; i < idx.Length; ++i)
                idx[i] = new Tuple<int, int>(-1,-1);

            var sorted = infos_.OrderBy(x => x.Value).Select(x => new Tuple<int,int>( (int)x.Key, x.Value) ). ToList();

            for (int i = 0; i < sorted.Count; ++i) {
                int len = i < sorted.Count - 1 ? sorted[i + 1].Item2 - sorted[i].Item2 : -1;
                idx[ sorted[i].Item1 ] = new Tuple<int, int>( sorted[i].Item2, len);
            }

            return idx;
        }

        private string entry_ = "";
        private Dictionary<info_type,int> infos_ = new Dictionary<info_type, int>();
    }
}
