using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lw_common.ui
{
    // note: the reason this is a form is that we should not tie it to a control - it might end up exceeding the control's boundaries
    public partial class snoop_around_form : Form {
        
        // the control this form conceptually belongs to - the coordinates we relate to are always given relative to this
        private Control logical_parent_;

        private Rectangle logical_parent_rect_ = new Rectangle();

        // the expander form
        private readonly snoop_around_expander_form expander_;

        private bool expanded_ = false;

        private Rectangle parent_size_on_move_ = new Rectangle();
        private DateTime parent_last_move_ = DateTime.MinValue;
        private bool was_visible_when_started_moving = false;

        public delegate void on_apply_func(bool apply);

        public on_apply_func on_apply;

        // this contains the "visible" property of the whole form + expander form 
        //
        // when we're collapsed, we're partially visible (this form is invisible, but the expander is visible)
        // when this is set to false, both form+explaner form are invisible, regardless of expanded/collapsed
        private bool is_visible_ = true;

        private readonly int min_width_;

        private bool snoop_running_ = false;
        private bool snoop_paused_ = false;

        // run_until - timeout for the snoop function.
        //             note that the snoop function needs to care about whether it's paused - when paused, do nothing
        //             the pause can happen when the user goes somewhere else, while we're snooping (thus, we're collapsed)
        public delegate void do_snoop_func(DateTime run_until, ref bool pause);

        public do_snoop_func do_snoop;

        // how many values do we allow? (we don't visually show more than this)
        private int max_distinct_values_count_ = 50;

        private bool too_many_distinct_values_ = false;

        private bool finished_ = false;
        private bool snooped_all_rows_ = false;

        private class snoop_item {
            private int number_ = 0;
            private string value_ = "";
            private string count_ = "";
            private bool is_checked_ = false;

            public int number {
                get { return number_; }
                set { number_ = value; }
            }

            public string value {
                get { return value_; }
                set { value_ = value; }
            }

            public string count {
                get { return count_; }
                set { count_ = value; }
            }

            public bool is_checked {
                get { return is_checked_; }
                set { is_checked_ = value; }
            }
        }

        public snoop_around_form(Control logical_parent) {
            logical_parent_ = logical_parent;
            InitializeComponent();
            expander_ = new snoop_around_expander_form(this);

            min_width_ = clear.Right - all.Left;
            logical_parent.Move += (sender, args) => on_parent_move();
            logical_parent.Resize += (sender, args) => on_parent_move();

            // I need to force this form to be visible for a bit, so that the handle is created, so that .Invoke works correctly
            Location = new Point(-100000, -100000);
            Visible = true;
            util.postpone(() => Visible = false, 1);

            is_visible = true;
        }

        public Rectangle logical_parent_rect {
            get { return logical_parent_rect_; }
            set {
                logical_parent_rect_ = value;
                update_pos();
            }
        }

        public Rectangle screen_logical_parent_rect {
            get { return logical_parent_.RectangleToScreen(logical_parent_rect_); }
        }

        public bool expanded {
            get { return expanded_; }
            set {
                if (expanded_ == value)
                    return;
                expanded_ = value;
                expander_.show_filter = expanded_ ? false : can_apply_filter();
                update_pos();
            }
        }

        public bool is_visible {
            get { return is_visible_; }
            set {
                is_visible_ = value;
                update_visible();
            }
        }

        public int max_distinct_values_count {
            get { return max_distinct_values_count_; }
            set { max_distinct_values_count_ = value; }
        }

        protected override bool ShowWithoutActivation { get { return true; } } // stops the window from stealing focus
        const int WS_EX_NOACTIVATE = 0x08000000;
        protected override CreateParams CreateParams {
            get {
                var Params = base.CreateParams;
                Params.ExStyle |= WS_EX_NOACTIVATE ;
                return Params;                
            }
        }

        private bool can_apply_filter() {
            if (list.GetItemCount() < 1)
                return false; // nothing to filter

            int is_checked = 0, is_unchecked = 0;
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                var i = list.GetItem(idx).RowObject as snoop_item;
                if (i.is_checked)
                    is_checked++;
                else
                    is_unchecked++;
            }
            int all = is_checked + is_unchecked;
            // at least one needs to be checked, and at least one needs to unchecked
            return is_checked > 0 && is_checked < all;
        }

        private void update_visible() {
            Visible = is_visible_ && expanded_ && logical_parent_.Width > 0;
            expander_.Visible = is_visible_ && logical_parent_.Width > 0;            
        }

        public void set_values(Dictionary< string, int> values, bool finished, bool snooped_all_rows) {
            this.async_call(() => set_values_impl(values, finished, snooped_all_rows) );
        }

        // important: we visually sort them!
        private void set_values_impl(Dictionary< string, int> values, bool finished, bool snooped_all_rows) {
            too_many_distinct_values_ = values.Count > max_distinct_values_count;
            finished_ = finished;
            snooped_all_rows_ = snooped_all_rows;

            while (values.Count > max_distinct_values_count) {
                int min = values.Values.Min();
                var erase = values.First(x => x.Value == min).Key;
                values.Remove(erase);
            }

            list.SuspendLayout();

            int sel = list.SelectedIndex;
            string former_sel = "";
            if (sel >= 0)
                former_sel = (list.GetItem(sel).RowObject as snoop_item).value;

            // find out the existing items 
            SortedDictionary<string,int> value_to_index = new SortedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            // just in case we're snooping, and there was a former snoop - we keep existing entries...
            // but, when the snoop is over, anything that found 0 results, is removed from the list
            int values_contain_existing_count = 0;
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                var value = (list.GetItem(idx).RowObject as snoop_item).value;
                if (values.ContainsKey(value))
                    ++values_contain_existing_count;
                value_to_index.Add(value, idx);
            }
            if (finished && values.Count > values_contain_existing_count) {
                // there were some rows with "zero" count - remove them completely
                list.Items.Clear();
                value_to_index.Clear();
            }

            foreach( string value in values.Keys)
                if ( !value_to_index.ContainsKey(value))
                    value_to_index.Add(value, -1);

            // snooped_all_rows - if false, we always append "+" to the count. Otherwise, we print only the count
            int offset = 0;
            int new_sel_idx = -1;
            foreach ( var val_and_idx in value_to_index) {
                string value = val_and_idx.Key;
                int count = values.ContainsKey(value) ? values[value] : 0;
                if (val_and_idx.Value < 0) 
                    list.InsertObjects(offset, new[] {new snoop_item() } );
                var i = list.GetItem(offset).RowObject as snoop_item;
                i.number = offset + 1;
                i.value = value;
                i.count = "" + count + (finished && snooped_all_rows ? "" : "+");
                if (value == former_sel)
                    new_sel_idx = offset;
                list.RefreshObject(i);
                ++offset;
            }

            if (new_sel_idx >= 0)
                list.SelectedIndex = new_sel_idx;

            list.ResumeLayout(true);
        }

        private void update_pos() {
            if (!is_visible_) {
                Debug.Assert(!Visible && !expander_.Visible);
                return;
            }

            update_visible();
            // update position of expander as well
            expander_.update_pos();

            var above = screen_logical_parent_rect;
            int left = above.Left;
            int top = above.Bottom;
            Location = new Point(left, top);
            Size = new Size( Math.Max( above.Width, min_width_), Height);

            if (expanded) {
                BringToFront();
                list.Focus();
            }
        }

        private void on_parent_move() {
            if (parent_last_move_.AddMilliseconds(750) < DateTime.Now)
                was_visible_when_started_moving = is_visible;
            parent_last_move_ = DateTime.Now;

            parent_size_on_move_ = logical_parent_.RectangleToScreen( logical_parent_.ClientRectangle);
            is_visible = false;
            util.postpone(check_parent_moving, 250);
        }

        private void check_parent_moving() {
            var rect_now = logical_parent_.RectangleToScreen( logical_parent_.ClientRectangle);
            if (rect_now == parent_size_on_move_) {
                // at this point, the move/resize stopped
                is_visible = was_visible_when_started_moving;
                update_pos();
            }
        }

        private void snoop_around_form_VisibleChanged(object sender, EventArgs e) {
        }

        internal void on_click_expand() {
            expanded = !expanded;
            // need to figure a way to continue an existing process or re-start a new one
        }

        internal void on_click_apply(bool apply) {
            expanded = false;
            on_apply(apply);
        }

        private void snoop_around_form_Deactivate(object sender, EventArgs e) {
            util.postpone(() => expanded = false, 10);
        }

        private void updateStatus_Tick(object sender, EventArgs e) {
            if (!expanded)
                return;

            List<string> is_checked = new List<string>(), is_unchecked = new List<string>();
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                var i = list.GetItem(idx).RowObject as snoop_item;
                if ( i.is_checked)
                    is_checked.Add(i.value);
                else 
                    is_unchecked.Add(i.value);
            }

            string status_text = too_many_distinct_values_ ? "Too Many Values! " : "";
            string status_tip = "";
            if (finished_) {
                status_text += "Filtering by: ";
                int all = is_checked.Count + is_unchecked.Count;
                if (is_checked.Count < all && is_unchecked.Count < all) {
                    status_text += is_checked.Count < is_unchecked.Count
                        ? (is_checked.Count == 1 ? "Value is " : "Any of ") + util.concatenate(is_checked, ", ") : 
                          (is_unchecked.Count == 1 ? "Value is NOT " : "None of ") + util.concatenate(is_unchecked, ", ");
                    status_tip = is_checked.Count < is_unchecked.Count
                        ? "Any of \r\n" + util.concatenate(is_checked, "\r\n") : "None of \r\n" + util.concatenate(is_unchecked, "\r\n");
                    int max_chars = 400;
                    status_tip = status_tip.Length > max_chars ? status_tip.Substring(0, max_chars) : status_tip;
                } else
                    status_text += "Nothing yet";
            } else
                status_text += "Snooping around" + util.ellipsis_suffix();
            status.Text = status_text;
            if (status_tip == "")
                status_tip = status_text;
            tip.SetToolTip(status, status_tip);
        }

        private void all_Click(object sender, EventArgs e) {
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                var i = list.GetItem(idx).RowObject as snoop_item;
                i.is_checked = true;
                list.RefreshObject(i);
            }
        }

        private void none_Click(object sender, EventArgs e) {
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                var i = list.GetItem(idx).RowObject as snoop_item;
                i.is_checked = false;
                list.RefreshObject(i);
            }
        }

        private void negate_Click(object sender, EventArgs e) {
            for (int idx = 0; idx < list.GetItemCount(); ++idx) {
                var i = list.GetItem(idx).RowObject as snoop_item;
                i.is_checked = !i.is_checked;
                list.RefreshObject(i);
            }
        }

        private void clear_Click(object sender, EventArgs e) {
            // clears the filter - shows all items
            Visible = false;
        }

        private void run_Click(object sender, EventArgs e) {
            // applies the current filter
        }
    }
}
