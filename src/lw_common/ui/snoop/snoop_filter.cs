using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common.parse;
using lw_common.ui;

namespace lw_common {
    public class snoop_filter : IDisposable {
        private Dictionary<info_type, snoop_around_form> snoops_ = new Dictionary<info_type, snoop_around_form>();
        private List<snoop_around_form> unused_ = new List<snoop_around_form>();
        private bool disposed_ = false;

        public snoop_filter() {
            // add the most of possible snoops
            foreach ( info_type type in Enum.GetValues(typeof(info_type)))
                if ( info_type_io.is_snoopable(type))
                    unused_.Add(new snoop_around_form());
        }

        public void on_new_log() {
            foreach (var snoop in snoops_.Values)
                snoop.clear();
        }

        public snoop_around_form snoop_for(info_type type) {
            Debug.Assert(!disposed_);
            Debug.Assert(info_type_io.is_snoopable(type));
            lock (this) {
                if (!snoops_.ContainsKey(type)) {
                    var use_now = unused_[0];
                    unused_.RemoveAt(0);
                    use_now.clear();
                    snoops_.Add(type, use_now);
                }
                return snoops_[type];
            }
        }

        public void on_aliases(aliases aliases) {
            lock(this)
                foreach ( info_type type in Enum.GetValues(typeof(info_type)))
                    if ( info_type_io.is_snoopable(type))
                        if (aliases.has_column(type))
                            snoop_for(type).is_visible = false;
                        else {
                            // this column is not visible for this log
                            if (snoops_.ContainsKey(type)) {
                                var unuse_now = snoops_[type];
                                snoops_.Remove(type);
                                unused_.Add(unuse_now);
                                unuse_now.is_visible = false;
                            }
                        }            
        }


        public void Dispose() {
            if (disposed_)
                return;
            disposed_ = true;
            foreach ( var snoop in snoops_.Values)
                snoop.Dispose();
            foreach (var snoop in unused_)
                snoop.Dispose();

            snoops_.Clear();
            unused_.Clear();
        }
    }
}
