using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class status_ctrl : rich_label_ctrl {
        // the status(es) to be shown
        public enum status_type {
            msg, warn, err
        }
        private List< Tuple<string, status_type, DateTime>> statuses_ = new List<Tuple<string, status_type, DateTime>>();
        // what to be shown behind ALL statuses
        private string status_prefix_ = "";

        public status_ctrl() {
            InitializeComponent();
        }

        private void refresh_Tick(object sender, EventArgs e) {
            update_status_text();
        }

        // sets the status for a given period - after that ends, the previous status is shown
        // if < 0, it's forever
        public void set_status(string msg, status_type type = status_type.msg, int set_status_for_ms = 7500) {
            if (set_status_for_ms <= 0)
                statuses_.Clear();

            if (type == status_type.err)
                // show errors longer
                set_status_for_ms = Math.Max(set_status_for_ms, 15000);
            statuses_.Add(new Tuple<string, status_type, DateTime>(msg, type, set_status_for_ms > 0 ? DateTime.Now.AddMilliseconds(set_status_for_ms) : DateTime.MaxValue));
            show_last_status();

            if (type == status_type.err)
                util.beep(util.beep_type.err);
        }

        private void show_last_status() {
            if (statuses_.Count < 1) 
                return;
            var last = statuses_.Last();
            var type = last.Item2;
            var msg = last.Item1;

            string color_prefix = "";
            if (status_color(type) != util.transparent)
                color_prefix += " <fg " + util.color_to_str(status_color(type)) + "> ";
            if (status_bg_color(type) != util.transparent)
                color_prefix += " <bg " + util.color_to_str(status_bg_color(type)) + "> ";
            set_text( color_prefix + status_prefix_ + msg);
        }

        public void set_status_forever(string msg) {
            set_status(msg, status_type.msg, -1);
        }

        public void set_prefix(string prefix) {
            status_prefix_ = prefix;
            update_status_text(true);
        }

        public status_type update_status_text(bool force = false) {
            bool needs_update = false;
            while (statuses_.Count > 0 && statuses_.Last().Item3 < DateTime.Now) {
                statuses_.RemoveAt(statuses_.Count - 1);
                needs_update = true;
            }

            if (needs_update || force) 
                show_last_status();

            bool is_err = statuses_.Count > 0 && statuses_.Last().Item2 == status_type.err;
            if (is_err)
                return status_type.err;
            return status_type.msg;
        }

        private Color status_color(status_type type) {
            return type == status_type.err ? Color.DarkRed : util.transparent;
        }

        private Color status_bg_color(status_type type) {
            return type == status_type.err ? Color.Yellow : util.transparent;
        }

    }
}
