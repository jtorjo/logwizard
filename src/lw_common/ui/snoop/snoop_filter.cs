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

        private class snoop_form_info {
            public snoop_around_form form = null;
            // when was it snooped - at what selection was it? (so that i know whether to snoop again)
            // note that different columns can be snooped at different locations
            public int snoop_sel = -1;

            // what the user snooped - if empty, don't filter at all
            public HashSet<string> snoop_selection = new HashSet<string>();

            // when did I apply this snoop? Each extra snoop is given an index (increased from zero upwards)
            // when the selection for a snoop changes:
            // - all those that weren't snooped, are cleared
            // - all those with a higher index, are cleared (the selection changed, thus, they need to be re-done)
            public int apply_index = -1;

            public void clear() {
                Debug.Assert(form != null);
                form.clear();
                snoop_sel = -1;
                apply_index = -1;
                snoop_selection.Clear();
            }
        }

        private Dictionary<info_type, snoop_form_info> snoops_ = new Dictionary<info_type, snoop_form_info>();
        private List<snoop_form_info> unused_ = new List<snoop_form_info>();
        private bool disposed_ = false;

        private log_view view_;

        internal snoop_filter(log_view view) {
            view_ = view;
            // add the most of possible snoops
            foreach ( info_type type in Enum.GetValues(typeof(info_type)))
                if (info_type_io.is_snoopable(type)) {
                    var form = new snoop_around_form();
                    form.on_apply = on_apply;
                    form.on_snoop = on_snoop;
                    unused_.Add( new snoop_form_info { form = form } );
                }
        }

        private snoop_form_info info(snoop_around_form self) {
            lock (this) {
                var find = snoops_.Values.FirstOrDefault(x => x.form == self);
                Debug.Assert(find != null);
                return find;
            }
        }
        private info_type info_key(snoop_around_form self) {
            lock (this) {
                var find = snoops_.FirstOrDefault(x => x.Value.form == self);
                return find.Key;
            }
        }

        private void on_snoop(snoop_around_form self, ref bool keep_running) {
            var info = this.info(self);
            if (info.snoop_selection.Count > 0) {
                // in this case, user selected at least something,
                // and then applied the selection - thus, no re-snooping
                self.reuse_last_values();
                return;
            }

            // we cannot be visible if users doesn't have any rows
            int sel = view_.sel_row_idx;
            if (sel <= 0)
                sel = 0;
            if ( info.snoop_sel >= 0)
                if (Math.Abs(info.snoop_sel - sel) <= app.inst.reuse_snoop_surrounding) {
                    // we already snooped near by, reuse that
                    self.reuse_last_values();
                    return;
                }

            // here, I know for sure I need to (re)snoop
            int all = view_.item_count;
            bool snoop_all = all <= app.inst.snoop_all_if_entries_less_than;
            int min = 0, max = all;
            if (!snoop_all) {
                var surrounding = util.surrounding(sel, app.inst.snoop_surrounding_entries, 0, all);
                min = surrounding.Item1;
                max = surrounding.Item2;
            }

            var type = info_key(self);
            int snoop_idx = 0;
            Dictionary< string, int> values = new Dictionary<string, int>();
            // update 
            int snoop_update_ui_step = (max - min) / app.inst.snoop_update_ui_times;
            for (int idx = min; idx < max; ++idx) {
                var i = view_.item_at(idx) as filter.match;
                var cur_value = i.line.part(type);
                if ( !values.ContainsKey(cur_value))
                    values.Add(cur_value, 0);
                ++values[cur_value];

                if (++snoop_idx % snoop_update_ui_step == 0)
                    if (keep_running) 
                        // ... set to a copy, since we're modifying this one
                        self.set_values(values.ToDictionary(x => x.Key, x => x.Value), false, false);
                    else 
                        break;
            }

            if ( keep_running)
                self.set_values(values, true, snoop_all);
        }

        private void on_apply(snoop_around_form self, List<string> selection) {
            var new_sel = new HashSet<string>(selection);
            var info = this.info(self);
            if (info.snoop_selection.SetEquals(new_sel))
                return; // nothing changed

            lock (this) {
                info.snoop_selection = new_sel;

                bool has_sel = new_sel.Count > 0;
                if (info.apply_index >= 0)
                    // all snoops with a higher index, are cleared (the selection changed, thus, they need to be re-done)
                    foreach (var snoop in snoops_.Values)
                        if (snoop.apply_index > info.apply_index)
                            snoop.clear();

                if (!has_sel)
                    // user cleared this snoop
                    info.clear();

                if (has_sel && info.apply_index < 0) {
                    // need to find out the apply_index
                    int max_apply = snoops_.Values.Max(x => x.apply_index);
                    info.apply_index = max_apply + 1;
                }

                // all snoops that did not have a selection yet, they need re-doing
                foreach (var snoop in snoops_.Values)
                    if (snoop.apply_index < 0)
                        snoop.clear();
            }
            view_.reapply_quick_filter();
        }

        public void clear() {
            foreach (var snoop in snoops_.Values)
                snoop.clear();            
        }

        public void on_new_log() {
            clear();
        }

        internal bool matches(match_item item) {
            lock(this)
                foreach ( var snoop in snoops_)
                    if (snoop.Value.snoop_selection.Count > 0) {
                        var cur_value = (item as filter.match).line.part(snoop.Key);
                        if (!snoop.Value.snoop_selection.Contains(cur_value))
                            return false;
                    }
            return true;
        }

        internal bool matches_all() {
            lock(this)
                foreach ( var snoop in snoops_.Values)
                    if (snoop.snoop_selection.Count > 0)
                        return false;
            return true;
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
                return snoops_[type].form;
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
                                unuse_now.form.is_visible = false;
                            }
                        }            
        }


        public void Dispose() {
            if (disposed_)
                return;
            disposed_ = true;
            foreach ( var snoop in snoops_.Values)
                snoop.form.Dispose();
            foreach (var snoop in unused_)
                snoop.form.Dispose();

            snoops_.Clear();
            unused_.Clear();
        }
    }
}
