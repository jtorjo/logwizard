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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    // Show by time - not implemented yet
    public partial class note_ctrl : UserControl {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string author_name_ = "", author_initials_ = "";
        private Color author_color_ = util.transparent;

        public class note {
            public string author_name = "";
            public string author_initials = "";
            public Color author_color = util.transparent;

            public string note_text = "";

            // NOT persisted: if this were persisted, all notes would end up as being added by current user
            //
            // if true, we always use the existing author name/initials/color
            // the reason we have this: if this was made by current user, always use the most up-to-date name/initials/color
            public bool made_by_current_user = false;

            // NOT persisted
            //
            // if true, it's a note added in this session 
            // IMPORTANT: the notes added in this session are always added at the end (if they are new notes, to new lines)
            public bool is_new = false;
        }

        public class line {
            protected bool Equals(line other) {
                return idx == other.idx && string.Equals(view_name, other.view_name) && string.Equals(msg, other.msg);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((line) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    var hashCode = idx;
                    hashCode = (hashCode * 397) ^ (view_name != null ? view_name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (msg != null ? msg.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public static bool operator ==(line left, line right) {
                return Equals(left, right);
            }

            public static bool operator !=(line left, line right) {
                return !Equals(left, right);
            }

            public override string ToString() {
                return "" + idx + "/" + view_name + " " + msg;
            }

            public void copy_from(line other) {
                idx = other.idx;
                view_name = other.view_name;
                msg = other.msg;
            }

            public void clear() {
                idx = -1;
                view_name = msg = "";
            }

            public int idx = -1;
            public string view_name = "";
            public string msg = "";
        }

        private class note_item {
            public readonly note_ctrl self = null;

            // if true, someone else made this note, and we just merged it
            public bool is_merged = false;

            // any note or line can be deleted
            public bool deleted = false;

            // if null, it's the header
            public note the_note = null;

            // each note has a unique ID -this can be used when adding a note that is in reply to someone
            public readonly string note_id;

            // in case this is a note that is in reply to another note, this contains the ID of the other note we're replying to;
            // if empty, it's not a reply
            public readonly string reply_id;

            // pointer to a line - each line has a unique ID (different from its index)
            private string line_id_;

            // when was this note added (could be useful when sorting by time)
            public DateTime utc_added = DateTime.UtcNow;

            // for debugging
            public string friendly_note {
                get {
                    string sub = "";
                    if (the_note != null)
                        sub = the_note.note_text.Length > 10 ? the_note.note_text.Substring(0, 10) : the_note.note_text;
                    string friendly_n = the_note != null ? "[" + sub + "...]" : "l-" + self.lines_[line_id].idx;
                    return friendly_n;
                }
            }

            // if true, this is the header (information about the line itself)
            public bool is_note_header {
                get { return the_note == null; }
            }

            public bool is_cur_line {
                get { return line_id_ == self.cur_line_id_; }
            }

            public string line_id {
                get { return line_id_; }
                set {
                    // you can only reset the line ID - if it is the current line
                    Debug.Assert(line_id_ == self.cur_line_id_);
                    line_id_ = value;
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public note_item(note_ctrl self, note n, string unique_id, string reply_id, string line_id, bool deleted) {
                Debug.Assert( self.lines_.ContainsKey(line_id));

                the_note = n;
                this.self = self;
                note_id = unique_id;
                line_id_ = line_id;
                this.reply_id = reply_id;
                this.deleted = deleted;
            }

            public note_item(note_ctrl self, string unique_id, string line_id, bool deleted) {
                Debug.Assert( self.lines_.ContainsKey(line_id));

                this.self = self;
                note_id = unique_id;
                line_id_ = line_id;
                reply_id = "";
                this.deleted = deleted;
                the_note = null;
            }
            /////////////////////////////////////////////////////////////////////////////////////////
            // remaining - getters

            public string the_idx {
                get {
                    if (is_note_header)
                        return "";
                    int idx = 0;

                    for (int other_idx = 0; other_idx < self.notesCtrl.GetItemCount(); ++other_idx) {
                        var i = self.notesCtrl.GetItem(other_idx).RowObject as note_item;
                        if (i == this)
                            break;

                        if (!i.is_note_header)
                            ++idx;
                    }

                    return "" + (idx + 1);
                }
            }

            public string the_line {
                get {
                    return the_note != null ? the_note.author_initials : "" + (self.lines_[line_id].idx + 1);
                }
            }


            public int indent {
                get {
                    int ind = 0;
                    note_item cur = this;
                    while (cur.reply_id != "") {
                        ++ind;
                        cur = self.notes_sorted_by_line_index_.FirstOrDefault(x => x.note_id == reply_id);
                        if (cur == null)
                            break;
                    }
                    return ind;
                }
            }


            public string the_text {
                get {
                    if (the_note != null) {
                        // gets the note that is printed on screen - on the list
                        // ignore #... at beginning of msg
                        return the_note.note_text;
                    } else
                        return self.lines_[line_id].msg;
                }
            }

            public Color text_fg {
                get {
                    // look into # at beginning of msg
                    return Color.Blue;                     
                }
            }
            public Color line_fg {
                get {
                    return Color.Blue;                     
                }
            }
        }

        private Dictionary<string, line> lines_ = new Dictionary<string,line>();

        // the are kept as if they were sorted by line, then time - first the line (header), then all the notes related to that line
        private List<note_item> notes_sorted_by_line_index_ = new List<note_item>();

        // special ID for the current line
        private string cur_line_id_ = "cur_line";

        private string file_name_ = "";

        private int ignore_change_ = 0;

        private line cur_line {
            get { return lines_[cur_line_id_]; }
        }

        private bool dirty_ = false;

        public delegate void on_note_selected_func(int line_idx, string msg);

        public on_note_selected_func on_note_selected;

        // use friendly GUIDs - use numbers instead
        private static bool use_friendly_guids = util.is_debug;
        private static long next_guid_ = DateTime.Now.Ticks;
        private static string next_guid {
            get {
                string next = use_friendly_guids ? "" + ++next_guid_ : "{" + Guid.NewGuid().ToString() + "}";
                return next;
            }
        }



        public note_ctrl() {
            InitializeComponent();
            lines_.Add(cur_line_id_, new line());
            update_cur_note_controls();
        }

        private void note_ctrl_Load(object sender, EventArgs e) {
        }

        // the controls we can navigate through with TAB hotkey
        public List<Control> tab_navigatable_controls {
            get {
                var nav = new List<Control>();
                nav.Add(notesCtrl);
                if ( curNote.Enabled)
                    nav.Add(curNote);
                return nav;
            }
        }

        public void set_author(string author_name, string author_initials, Color notes_color) {
            author_name_ = author_name;
            author_initials_ = author_initials;
            author_color_ = notes_color;
            // update anything that was added by me before the changes were made
            foreach (note_item n in notes_sorted_by_line_index_)
                if (n.the_note != null && n.the_note.made_by_current_user) {
                    n.the_note.author_name = author_name;
                    n.the_note.author_initials = author_initials;
                    n.the_note.author_color = notes_color;
                }
            notesCtrl.Refresh();
        }

        // the only time this can get called is when loading an already saved configuration
        private note_item add_note_header(string line_id, string note_id, bool deleted) {
            Debug.Assert(note_id != "");
            Debug.Assert(line_id != cur_line_id_ && lines_.ContainsKey(line_id));
            var new_ = new note_item(this, note_id, line_id, deleted );
            notes_sorted_by_line_index_.Add(new_);
            add_note_to_ui(new_);
            return new_;
        }

        // helper - add to the currently selected line
        private note_item add_note(note n, string note_id, string reply_to_note_id, bool deleted) {
            Debug.Assert(cur_line != null);
            if (cur_line == null) {
                Debug.Assert(false);
                return null;
            }

            line new_ = new line();
            new_.copy_from(cur_line);
            return add_note(new_, n, note_id, reply_to_note_id, deleted);
        }


        private void log_notes_sorted_idx(int idx, note_item n) {
            if (idx >= 0) 
                logger.Info("[notes] insert " + n.friendly_note + " before " + notes_sorted_by_line_index_[idx].friendly_note);
            else 
                logger.Info("[notes] adding " + n.friendly_note + " at the end");
        }

        // returns the ID assigned to this note that we're adding;
        // if null, something went wrong
        //
        // note_id - the unique ID of the note, or "" if a new unique ID is to be created
        //           the reason we have this is that we can persist notes (together with their IDs)
        private note_item add_note(line l, note n, string note_id, string reply_to_note_id, bool deleted) {
            // a note: can be added on a line, or as reply to someone
            // new notes: always at the end

            string line_id = "";
            // ... first, try to see if we already have a valid line
            var found_line = lines_.FirstOrDefault(x => x.Key != cur_line_id_ && x.Value == l);
            if ( found_line.Value == null)
                found_line = lines_.FirstOrDefault(x => x.Value == l);
            Debug.Assert(found_line.Value != null);
            if (found_line.Key == cur_line_id_) {
                // it's the current line
                line_id = next_guid;
                lines_.Add(line_id, l);
            } 
            else 
                line_id = found_line.Key;

            note_id = note_id != "" ? note_id : next_guid;
            // ... this note should not exist already
            Debug.Assert( notes_sorted_by_line_index_.Count(x => x.note_id == note_id) == 0);
            if ( reply_to_note_id != "")
                // note we're replying to should exist already
                Debug.Assert( notes_sorted_by_line_index_.Count(x => x.note_id == reply_to_note_id) == 1);

            var new_ = new note_item(this, n, note_id, reply_to_note_id, line_id, deleted);
            bool inserted = false;
            if (reply_to_note_id != "") {
                // add this note as the last reply to this reply note
                var note = notes_sorted_by_line_index_.FirstOrDefault(x => x.line_id == line_id);
                // when it's a reply, we need to find the original note!
                Debug.Assert(note != null);
                if (note != null) {
                    Debug.Assert(note.is_note_header);
                    // everything following this, is related to this line (until we get to next line)
                    int idx_note = notes_sorted_by_line_index_.IndexOf(note);
                    for (; idx_note < notes_sorted_by_line_index_.Count && !inserted; idx_note++)
                        if (notes_sorted_by_line_index_[idx_note].line_id == line_id) {
                            if (reply_to_note_id != "") {
                                // in this case, look for this note (that we're replying to)
                                if (notes_sorted_by_line_index_[idx_note].note_id == reply_to_note_id) {
                                    log_notes_sorted_idx(idx_note + 1, new_);
                                    notes_sorted_by_line_index_.Insert(idx_note + 1, new_);                                    
                                    inserted = true;
                                }
                            } else {
                                // look for the last note about this line, and insert it after that
                                if ( idx_note < notes_sorted_by_line_index_.Count - 1)
                                    if (notes_sorted_by_line_index_[idx_note + 1].line_id != line_id) {
                                        // found the last note that relates to this line
                                        notes_sorted_by_line_index_.Insert(idx_note + 1, new_);
                                        log_notes_sorted_idx(idx_note + 1, new_);
                                        inserted = true;                                        
                                    }
                            }
                        } else
                            // went to next line
                            break;
                }
            }
            else {
                var note = notes_sorted_by_line_index_.FirstOrDefault(x => x.line_id == line_id);
                if (note != null) {
                    // in this case, there may be other notes - we're adding it to the end (as last note on this line)
                    Debug.Assert(note.is_note_header);
                    int idx = notes_sorted_by_line_index_.IndexOf(note);
                    for ( ; idx < notes_sorted_by_line_index_.Count; ++idx)
                        if ( idx < notes_sorted_by_line_index_.Count - 1)
                            if (notes_sorted_by_line_index_[idx + 1].line_id != line_id)
                                break;
                    if (idx < notes_sorted_by_line_index_.Count) {
                        log_notes_sorted_idx(idx + 1, new_);
                        notes_sorted_by_line_index_.Insert(idx + 1, new_);
                    } else {
                        log_notes_sorted_idx(-1, new_);
                        notes_sorted_by_line_index_.Add(new_);
                    }
                    inserted = true;
                } else {
                    // this is the first entry that relates to this line
                    // ... find the note before which we should insert ourselves
                    int line_index = lines_[line_id].idx;
                    note = notes_sorted_by_line_index_.FirstOrDefault(x => lines_[x.line_id].idx > line_index);

                    var header = new note_item(this, next_guid, line_id, deleted);
                    if (note != null) {
                        int idx = notes_sorted_by_line_index_.IndexOf(note);
                        bool has_header = note.is_note_header && note.line_id == line_id;
                        if (!has_header) {
                            log_notes_sorted_idx(idx, header);
                            notes_sorted_by_line_index_.Insert(idx, header);
                            log_notes_sorted_idx(idx + 1, new_);
                            notes_sorted_by_line_index_.Insert(idx + 1, new_);
                        } else {
                            // header is already added
                            if (header.deleted && !deleted)
                                // in this case, the header was deleted (probably we removed all items related to this line a while ago,
                                // and now we have added a new note here)
                                header.deleted = false;
                            log_notes_sorted_idx(idx, new_);
                            notes_sorted_by_line_index_.Insert(idx, new_);
                        }
                    } else {
                        log_notes_sorted_idx(-1, header);
                        log_notes_sorted_idx(-1, new_);
                        notes_sorted_by_line_index_.Add(header);
                        notes_sorted_by_line_index_.Add(new_);
                    }
                    inserted = true;
                }
            }
            Debug.Assert(inserted);

            // update the UI
            if (inserted)
                add_note_to_ui(new_);

            dirty_ = true;
            return new_;
        }

        private void add_note_to_ui(note_item last_note) {
            bool add_to_ui = !last_note.deleted || (last_note.deleted && showDeletedLines.Checked);
            if (!add_to_ui)
                return;

            int insert_idx = -1;
            if (last_note.reply_id != "") {
                // it's a note on reply to another note
                for (int note_idx = 0; note_idx < notesCtrl.GetItemCount() && insert_idx < 0; ++note_idx) {
                    var i = notesCtrl.GetItem(note_idx).RowObject as note_item;
                    if (i.note_id == last_note.reply_id) 
                        insert_idx = note_idx + 1;
                }
            } else {
                // new note
                if (!last_note.is_note_header)
                    for (int note_idx = 0; note_idx < notesCtrl.GetItemCount(); ++note_idx) {
                        var i = notesCtrl.GetItem(note_idx).RowObject as note_item;
                        string line_id = i.line_id != cur_line_id_ ? i.line_id : find_cur_line_id();

                        if (line_id == last_note.line_id) {
                            insert_idx = note_idx + 1;
                            bool insert_at_the_end = i.line_id == cur_line_id_;
                            if (insert_at_the_end) {
                                // we need to update the line, so that it does not point to current-line anymore
                                i.line_id = line_id;
                            }
                        }
                    }
                else
                    // it's a new note header, add at the end
                    insert_idx = notesCtrl.GetItemCount();
            }
            Debug.Assert( insert_idx >= 0);
            if (insert_idx >= 0) {
                ++ignore_change_;
                notesCtrl.InsertObjects(insert_idx, new object[] {last_note});
                // now, select it as well
                notesCtrl.SelectObject(last_note);
                // ... so that it recomputes its UI index correctly
                notesCtrl.RefreshObject(last_note);
                --ignore_change_;
                notesCtrl.EnsureVisible(insert_idx);
                refresh_note(last_note);
            }
        }

        private string find_cur_line_id() {
            // the last line - is always the last
            Debug.Assert(lines_.Count >= 2);
            foreach ( var l in lines_)
                if (l.Key != cur_line_id_ && l.Value == cur_line)
                    return l.Key;
            // we could not find it!
            Debug.Assert(false);
            return cur_line_id_;
        }


        private void toggle_del_note(string note_id) {
            var toggle_note = notes_sorted_by_line_index_.FirstOrDefault(x => x.note_id == note_id);
            if (toggle_note == null) {
                Debug.Assert(false);
                return;
            }

            dirty_ = true;

            string line_id = notes_sorted_by_line_index_.First(x => x.note_id == note_id).line_id;
            int same_line_count = notes_sorted_by_line_index_.Count(x => x.line_id == line_id);
            // at least the line + a note on that line
            Debug.Assert(same_line_count >= 2); 
            bool is_single = same_line_count == 2;
            if (is_single) {
                foreach (var n in notes_sorted_by_line_index_)
                    if (n.line_id == line_id)
                        n.deleted = !n.deleted;
            }
            else if (toggle_note.the_note == null) {
                // in this case, pressed on a line header - toggle all its sub-children
                foreach (var n in notes_sorted_by_line_index_)
                    if (n.line_id == toggle_note.line_id && n.the_note != null)
                        n.deleted = !n.deleted;
            } else {
                // pressed on a note - find all replies on that note
                List<string> ids = new List<string>();
                ids.Add(note_id);
                bool reply_found = true;
                while (reply_found) {
                    reply_found = false;
                    foreach ( var n in notes_sorted_by_line_index_)
                        if (n.reply_id != "" && ids.Contains(n.reply_id)) {
                            ids.Add(n.note_id);
                            reply_found = true;
                        }
                }
                int delete_count = notes_sorted_by_line_index_.Count(x => ids.Contains(x.note_id) && !x.deleted);
                bool do_delete = delete_count == ids.Count;
                foreach ( var n in notes_sorted_by_line_index_)
                    if (ids.Contains(n.note_id) && n.the_note != null)
                        n.deleted = do_delete;
            }

            readd_everything();

            // note: delete someone else's note -> it's grayed (hidden)
            //       delete your note: it's fully deleted + copied to clipboard -> ALL THE notes you delete - keep them internally and the user should be able to access them
            //       (just in case he did a mistake)

            // if i delete a note from line ,and no other notes on that line:
            // if it was from current user, delete the line note as well ; otherwise, just hide it
        }

        private void readd_everything() {
            // at this point, I have all the notes in "sorted notes" set up correctly - I need to show them in the UI
            var copy = notes_sorted_by_line_index_.ToList();

            var sel_note_id = notesCtrl.SelectedIndex >= 0 ? (notesCtrl.GetItem(notesCtrl.SelectedIndex).RowObject as note_item).note_id : "";
            var sel_note_id_above = find_note_id_above_selection();

            ++ignore_change_;
            notes_sorted_by_line_index_.Clear();
            notesCtrl.ClearObjects();

            // ... re-add everything
            foreach ( var n in copy) {                
                if (n.is_note_header)
                    add_note_header(n.line_id, n.note_id, n.deleted);
                else
                    add_note(lines_[n.line_id], n.the_note, n.note_id, n.reply_id, n.deleted);
            }
            Debug.Assert(notes_sorted_by_line_index_.Count == copy.Count);

            // set current line
            if (showDeletedLines.Checked) {
                var cur_line_copy = new line();
                cur_line_copy.copy_from(cur_line);
                cur_line.clear();
                set_current_line(cur_line_copy);
            } else
                cur_line.clear();

            string new_sel = showDeletedLines.Checked ? sel_note_id : sel_note_id_above;
            if ( new_sel != "")
                for ( int idx = 0; idx < notesCtrl.GetItemCount(); ++idx)
                    if ((notesCtrl.GetItem(idx).RowObject as note_item).note_id == new_sel) {
                        notesCtrl.SelectedIndex = idx;
                        break;
                    }

            --ignore_change_;

            refresh_note_indexes();
        }


        private string find_note_id_above_selection() {
            if (notesCtrl.SelectedIndex < 0) {
                // nothing was selected
                if (notesCtrl.GetItemCount() > 0)
                    return (notesCtrl.GetItem(notesCtrl.GetItemCount() - 1).RowObject as note_item).note_id;
                return "";
            }

            var sel_note = notesCtrl.SelectedItem.RowObject as note_item;
            for ( int i = 0; i < notesCtrl.GetItemCount() - 1; ++i)
                if (notesCtrl.GetItem(i + 1).RowObject == sel_note)
                    return (notesCtrl.GetItem(i).RowObject as note_item).note_id ;

            if (notesCtrl.GetItemCount() > 0)
                return (notesCtrl.GetItem(notesCtrl.GetItemCount() - 1).RowObject as note_item).note_id;
            else
                return "";
        }

        // this sets the line the user is viewing - by default, this is what the user is commenting on
        public void set_current_line(line l) {
            if (cur_line.idx == l.idx && cur_line.view_name == l.view_name)
                return; // we're already there
            cur_line.copy_from(l);

            /* Possibilities:
            
            - case 1 - there are already notes on this line

                1a - there are already notes on this line, and the selection is already one of them
                1b - there are already notes on this line, and the selection is on another note (or nowhere)

            - case 2 - there are no notes on this line
            */
            note_item header = null;
            for (int idx = 0; idx < notesCtrl.GetItemCount() && header == null ; ++idx) {
                var i = notesCtrl.GetItem(idx).RowObject as note_item;
                if (i.is_note_header && lines_[i.line_id].idx == l.idx && !i.is_cur_line) 
                    header = i;
            }

            bool last_note_is_cur_line = notesCtrl.GetItemCount() > 0 && (notesCtrl.GetItem(notesCtrl.GetItemCount() - 1).RowObject as note_item).is_cur_line;
            if (header != null) {
                // already a note on this line, not needed now
                if (last_note_is_cur_line)
                    notesCtrl.RemoveObject(notesCtrl.GetItem(notesCtrl.GetItemCount() - 1).RowObject);

                int sel = notesCtrl.SelectedIndex;
                var sel_item = sel >= 0 ? notesCtrl.GetItem(sel).RowObject as note_item : null;
                if (sel_item != null && sel_item.line_id == header.line_id)
                    // 1a - on a note from this line
                    return;

                // 1b - on a note from another line
                // I will select the last note from this user
                var last = ui_find_last_note_from_cur_user(header.line_id);
                notesCtrl.SelectObject( last);
                notesCtrl.EnsureModelVisible(last);
            } else {
                // 2 - no notes on this line
                if ( !last_note_is_cur_line) 
                    notesCtrl.AddObject( new note_item(this, next_guid, cur_line_id_, false));
                var last = notesCtrl.GetItem(notesCtrl.GetItemCount() - 1).RowObject as note_item;
                refresh_note(last);

                bool focus_on_notes = win32.focused_ctrl() == notesCtrl;
                if (notesCtrl.SelectedIndex < 0 || !focus_on_notes) {
                    // select the "last line"
                    notesCtrl.SelectedIndex = notesCtrl.GetItemCount() - 1;
                    notesCtrl.EnsureVisible( notesCtrl.GetItemCount() - 1);
                }
            }

            update_cur_note_controls();
        }

        private note_item ui_find_last_note_from_cur_user(string line_id) {
            note_item last_note = null;
            for (int idx = 0; idx < notesCtrl.GetItemCount(); ++idx) {
                var i = notesCtrl.GetItem(idx).RowObject as note_item;
                if (i.line_id == line_id) {
                    if (last_note != null) {
                        if (i.utc_added > last_note.utc_added)
                            last_note = i;
                    } else
                        last_note = i;
                }
            }
            Debug.Assert(last_note != null);
            return last_note;
        }




        // notifies the views of what the user has selected (what line / view)
        private void sync_to_views() {
            int sel = notesCtrl.SelectedIndex;
            if (sel < 0)
                return;
            var i = notesCtrl.GetItem(sel).RowObject as note_item;
            if (i.is_cur_line)
                // always ignore "current line"
                return;

            int line_idx = lines_[i.line_id].idx;
            string msg = lines_[i.line_id].msg;
            on_note_selected(line_idx, msg);
        }


        public void undo() {
            // no need - just toggle "Show Deleted Notes"
        }

        public void load(string file_name) {
            ++ignore_change_;
            if (file_name_ != "")
                // in this case, we're loading the notes from somewhere else, save existing first
                save();

            file_name_ = file_name;

            notes_sorted_by_line_index_.Clear();
            notesCtrl.ClearObjects();
            cur_line.clear();
            
            lines_.Clear();
            lines_.Add(cur_line_id_, new line());

            settings_file sett = new settings_file(file_name_);
            // first, load lines
            int line_count = int.Parse(sett.get("line_count", "0"));
            for (int idx = 0; idx < line_count; ++idx) {
                string prefix = "line." + idx + ".";
                line l = new line();
                l.idx = int.Parse(sett.get(prefix + "index", "0"));
                l.view_name = sett.get(prefix + "view_name");
                l.msg = sett.get(prefix + "msg");
                var id = sett.get(prefix + "id", "");
                lines_.Add(id, l);
            }

            // load notes
            // if author name = ourselves -> made_by_current_user = true
            int note_count = int.Parse(sett.get("note_count", "0"));
            for (int idx = 0; idx < note_count; ++idx) {
                string prefix = "note." + idx + ".";
                note n = null;
                string author_name = sett.get(prefix + "author_name");
                if (author_name != "") {
                    n = new note() {author_name = author_name};
                    n.author_initials = sett.get(prefix + "author_initials");
                    n.author_color = util.str_to_color(sett.get(prefix + "author_color"));
                    n.note_text = sett.get(prefix + "note_text");
                }
                bool deleted = sett.get(prefix + "deleted", "0") != "0";
                var note_id = sett.get(prefix + "note_id", "");
                var reply_id = sett.get(prefix + "reply_id", "");
                var line_id = sett.get(prefix + "line_id", "");
                long ticks = long.Parse(sett.get(prefix + "added_at", "0"));
                note_item note;
                if (n != null)
                    note = add_note(lines_[line_id], n, note_id, reply_id, deleted);
                else
                    note = add_note_header(line_id, note_id, deleted);
                note.utc_added = new DateTime(ticks);
            }
        
            // update last_note_id and last_line_id
            dirty_ = false;
            --ignore_change_;

            refresh_note_indexes();
            notesCtrl.Refresh();
        }

        public void save() {
            // needs to be loaded first
            if (file_name_ == "")
                return;
            if (!dirty_)
                return;

            settings_file sett = new settings_file(file_name_) { log_each_set = false };

            // first, save lines - don't save the cur_line
            sett.set("line_count", "" + (lines_.Count - 1));
            int line_idx = 0;
            foreach ( var l in lines_)
                if (l.Key != cur_line_id_) {
                    string prefix = "line." + line_idx + ".";
                    sett.set(prefix + "id", "" + l.Key);
                    sett.set(prefix + "index", "" + l.Value.idx);
                    sett.set(prefix + "view_name", l.Value.view_name);
                    sett.set(prefix + "msg", l.Value.msg);
                    ++line_idx;
                }

            // save notes
            sett.set("note_count", "" + notes_sorted_by_line_index_.Count);
            for (int idx = 0; idx < notes_sorted_by_line_index_.Count; ++idx) {
                string prefix = "note." + idx + ".";
                note_item n = notes_sorted_by_line_index_[idx];
                if (n.the_note != null) {
                    sett.set(prefix + "author_name", n.the_note.author_name);
                    sett.set(prefix + "author_initials", n.the_note.author_initials);
                    sett.set(prefix + "author_color", util.color_to_str(n.the_note.author_color));
                    sett.set(prefix + "note_text", n.the_note.note_text);
                }
                else 
                    // clear the author name, so we know it's not a note (it's a line)
                    sett.set(prefix + "author_name", "");

                sett.set(prefix + "deleted", n.deleted ? "1" : "0");
                sett.set(prefix + "note_id", "" + n.note_id);
                sett.set(prefix + "reply_id", "" + n.reply_id);
                sett.set(prefix + "line_id", "" + n.line_id);
                sett.set(prefix + "added_at", "" + n.utc_added.Ticks);
            }
            sett.save();

            dirty_ = false;
        }

        private void refresh_note(note_item i) {
            notesCtrl.RefreshObject(i);
            // FIXME fg + bg - see about whether is deleted or not
        }

        private void refresh_note_indexes() {
            // so that the UI indexes (first column) are recomputed - in case of an insert
            for ( int idx = 0; idx < notesCtrl.GetItemCount(); ++idx)
                notesCtrl.RefreshObject( notesCtrl.GetItem(idx).RowObject );
        }

        private void curNote_KeyDown(object sender, KeyEventArgs e) {
            string s = util.key_to_action(e);
            if (s == "return") {
                // add note
                string note_text = curNote.Text.Trim();
                if (note_text == "")
                    return;
                // see if I'm on a user's note (thus, i'm updating), or on a line (thus, i'm adding a note for that line)
                string reply_to_note_id = "";
                int sel = notesCtrl.SelectedIndex;
                if (sel >= 0) {
                    var i = notesCtrl.GetItem(sel).RowObject as note_item;
                    bool is_edit = i.the_note != null && i.the_note.made_by_current_user;
                    if (is_edit) {
                        dirty_ = true;
                        i.the_note.note_text = note_text;
                        // user modified the note
                        i.utc_added = DateTime.UtcNow;
                        refresh_note(i);
                        return;
                    }
                    bool is_reply = i.the_note != null && !i.the_note.made_by_current_user;
                    if (is_reply)
                        reply_to_note_id = i.note_id;
                }

                note new_ = new note() { author_name = author_name_, author_initials = author_initials_, 
                    author_color = author_color_, note_text = note_text, made_by_current_user = true, is_new = true };
                add_note(new_, "", reply_to_note_id, false);
            }
        }

        private void curNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            string s = util.key_to_action(e);
            if (s == "return")
                e.IsInputKey = true;
        }
        private void curNote_KeyUp(object sender, KeyEventArgs e) {
            string s = util.key_to_action(e);
            if (s == "return") {
                refresh_note_indexes();
                notesCtrl.Focus();
            }
        }

        private void notesCtrl_SelectedIndexChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            update_cur_note_controls();
            if (win32.focused_ctrl() == notesCtrl)
                sync_to_views();
        }

        private void update_cur_note_controls() {
            int sel = notesCtrl.SelectedIndex;

            ++ignore_change_;
            // note: the only time when sel < 0 is possible only when the user hasn't selected any line yet
            //       (thus, even cur_line is pointing nowhere)
            addNoteToLine.Visible = sel >= 0;
            addNoteToLineLabel.Visible = sel >= 0;

            addNoteToLine.Text = "";
            if (sel >= 0) {
                var i = notesCtrl.GetItem(sel).RowObject as note_item;
                if (i.the_note != null)
                    addNoteToLineLabel.Text = i.the_note.made_by_current_user ? "Edit Note to Line" : "Reply to Note from " + i.the_note.author_initials;
                else
                    addNoteToLineLabel.Text = "Add Note to Line";
                addNoteToLine.Text = "[" + (lines_[i.line_id].idx + 1) + "]";
                curNote.Text = i.the_note != null ? i.the_note.note_text : "";
                // if note made by someone else, don't allow changing it
                curNote.Enabled = i.the_note == null || i.the_note.made_by_current_user;
            } else {
                curNote.Enabled = false;
                curNote.Text = "";
            }

            --ignore_change_;
        }

        private void saveTimer_Tick(object sender, EventArgs e) {
            if (dirty_)
                save();
        }

        private void notesCtrl_Enter(object sender, EventArgs e) {
            curNote.BackColor = Color.AliceBlue;
        }

        private void notesCtrl_Leave(object sender, EventArgs e) {
            curNote.BackColor = Color.White;
        }

        private void showDeletedLines_CheckedChanged(object sender, EventArgs e) {
            // optimize - if nothing deleted
            int del_count = notes_sorted_by_line_index_.Count(x => x.deleted);
            if (del_count == 0)
                return;

            readd_everything();
        }

        private void notesCtrl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            string key = util.key_to_action(e);
            bool del = key == "delete" || key == "back";
            if (del)
                e.IsInputKey = true;
        }

        private void notesCtrl_KeyDown(object sender, KeyEventArgs e) {
            string key = util.key_to_action(e);
            bool del = key == "delete" || key == "back";
            if ( del)
                if ( notesCtrl.SelectedIndex >= 0)
                    toggle_del_note( (notesCtrl.GetItem(notesCtrl.SelectedIndex).RowObject as note_item).note_id);
        }


    }
}
