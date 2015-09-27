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
using BrightIdeasSoftware;

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

            public override string ToString() {
                return "[" + author_initials + "] " + note_text;
            }

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
            public string line_id;

            // when was this note added (could be useful when sorting by time)
            public DateTime utc_last_edited = DateTime.UtcNow;

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
                get { return line_id == self.cur_line_id_; }
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public note_item(note_ctrl self, note n, string unique_id, string reply_id, string line_id, bool deleted, DateTime utc_added) {
                the_note = n;
                this.self = self;
                note_id = unique_id;
                this.line_id = line_id;
                this.reply_id = reply_id;
                this.deleted = deleted;
                this.utc_last_edited = utc_added;
            }

            public note_item(note_ctrl self, string unique_id, string line_id, bool deleted, DateTime utc_added) {
                this.self = self;
                note_id = unique_id;
                this.line_id = line_id;
                reply_id = "";
                this.deleted = deleted;
                the_note = null;
                this.utc_last_edited = utc_added;
            }
            /////////////////////////////////////////////////////////////////////////////////////////
            // remaining - getters

            // note: if any chidren of a deleted note/line is !deleted, we need to show that note/line (even if shown in gray)
            //       normally, this should not happen - however, I see this as a possibility when merging two notes files
            //       say X deletes note N, but Y replies on it - in this case, I want to see the note, even if marked "Deleted"
            public bool ui_deleted {
                get {
                    if (!deleted)
                        // in this case, we're clearly visible
                        return deleted;

                    // in this case, we're deleted - look for any descendants that are not deleted
                    foreach ( var n in descendants())
                        if (!n.deleted)
                            // found a non-deleted descendant
                            return false;

                    return deleted;
                }
            }

            public List<note_item> descendants() {

                if (the_note == null) {
                    List<note_item> desc = new List<note_item>();
                    // in this case, a line header 
                    foreach (var n in self.notes_sorted_by_line_index_)
                        if (n.line_id == line_id)
                            desc.Add(n);
                    return desc;
                }

                List<string> ids = new List<string>();
                ids.Add(note_id);
                bool reply_found = true;
                while (reply_found) {
                    reply_found = false;
                    foreach ( var n in self.notes_sorted_by_line_index_)
                        if (!ids.Contains(n.note_id) && ids.Contains(n.reply_id)) {
                            ids.Add(n.note_id);
                            reply_found = true;
                        }
                }
                // exclude itself
                ids.RemoveAt(0);
                return self.notes_sorted_by_line_index_.Where(x => ids.Contains(x.note_id)).ToList();
            } 

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
                    if (the_note == null)
                        return 0;

                    int ind = 0;
                    note_item cur = this;
                    while (cur.reply_id != "") {
                        ++ind;
                        cur = self.notes_sorted_by_line_index_.FirstOrDefault(x => x.note_id == cur.reply_id);
                        if (cur == null)
                            break;
                    }
                    return ind + 1;
                }
            }


            public string the_text {
                get {
                    string txt = the_note != null ? note_no_color() : self.lines_[line_id].msg;
                    int indent = this.indent;
                    if ( indent > 0)
                        txt = new string(' ', indent * 2) + txt;
                    return txt;
                }
            }

            private static Color deleted_fg = Color.LightGray;
            private static Color header_fg = Color.DarkViolet;
            private static Color msg_idx_fg = Color.DarkGray;

            private static Color author_bg = Color.FloralWhite;
            private static Color merged_bg = Color.WhiteSmoke;

            private Color note_color() {
                Debug.Assert(the_note != null);
                // look into # at beginning of msg
                string msg = the_note.note_text.Trim();
                if (msg.StartsWith("#")) {
                    int space = msg.IndexOf(' ');
                    if (space > 0) {
                        string color_str = msg.Substring(1, space - 1);
                        return util.str_to_color(color_str);
                    }
                }

                return util.transparent;
            }

            private string note_no_color() {
                Debug.Assert(the_note != null);
                // look into # at beginning of msg
                string msg = the_note.note_text.Trim();
                if (msg.StartsWith("#")) {
                    int space = msg.IndexOf(' ');
                    if (space > 0) {
                        string color_str = msg.Substring(1, space - 1);
                        if (util.str_to_color(color_str) != util.transparent) 
                            // in this case, there was a valid color set in the text
                            return msg.Substring(space + 1).Trim();                        
                    }
                }
                return the_note.note_text;
            }

            public Color bg {
                get {
                    int sel = self.notesCtrl.SelectedIndex;
                    if (sel >= 0) {
                        var i = self.notesCtrl.GetItem(sel).RowObject as note_item;
                        string author = i.the_note != null ? i.the_note.author_name : "" ;
                        if (author != "" && the_note != null && the_note.author_name == author)
                            return author_bg;
                    }

                    if (is_merged)
                        return merged_bg;

                    return Color.White;
                }
            }

            public Color idx_fg {
                get { return msg_idx_fg; }
            }

            public Color text_fg {
                get {
                    if (ui_deleted)
                        return deleted_fg;

                    if ( the_note == null)                         
                        return header_fg; // it's the note header

                    // it's a note
                    Color c = note_color();
                    if (c != util.transparent)
                        return c;

                    return self.author_color(the_note.author_name);
                }
            }
            public Color line_fg {
                get {
                    if (ui_deleted)
                        return deleted_fg;
                    if ( the_note == null)                         
                        return header_fg; // it's the note header
                    // it's a note

                    return self.author_color(the_note.author_name);
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

        private Font header_font_ = null;
        private Font note_font_ = null;

        private readonly Color[] default_author_colors_ = new Color[] {
            Color.Blue,
            Color.MediumPurple, 
            Color.Red,
            Color.Maroon,
            Color.RosyBrown, 
            Color.SteelBlue, 
            Color.Brown, 
            Color.Magenta, 
            Color.Chocolate,
            Color.DarkMagenta,
            Color.Coral, 
            Color.Violet, 
            Color.Pink, 
            Color.Green, 
            Color.DarkOrange,
            Color.Indigo, 
        };

        // what color to use for authors whose color clashes with ours 
        // - say I'm marked blue, and another user is also marked blue
        //   in this case, I will preserve myself as blue, and use another color for the other guy
        private SortedDictionary<string, Color> author_colors_ = new SortedDictionary<string, Color>(); 

        // if use friendly GUIDs - use numbers instead - this should never be on production!
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
            header_font_ = notesCtrl.Font;
            note_font_ = new Font("Tahoma", header_font_.Size, FontStyle.Bold);
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

            update_author_colors();
            notesCtrl.Refresh();
        }

        // the only time this can get called is when loading an already saved configuration
        private note_item add_note_header(string line_id, string note_id, bool deleted, DateTime utc_added) {
            Debug.Assert(note_id != "");
            Debug.Assert(line_id != cur_line_id_ && lines_.ContainsKey(line_id));
            var new_ = new note_item(this, note_id, line_id, deleted, utc_added );
            notes_sorted_by_line_index_.Add(new_);
            add_note_to_ui(new_);
            return new_;
        }

        // helper - add to the currently selected line
        private note_item add_note(note n, string note_id, string reply_to_note_id, bool deleted, DateTime utc_added) {
            Debug.Assert(cur_line != null);
            if (cur_line == null) {
                Debug.Assert(false);
                return null;
            }

            line new_ = new line();
            new_.copy_from(cur_line);
            return add_note(new_, n, note_id, reply_to_note_id, deleted, utc_added);
        }


        private void log_notes_sorted_idx(int idx, note_item n) {
            if (idx >= 0 && idx < notes_sorted_by_line_index_.Count) 
                logger.Info("[notes] insert " + n.friendly_note + " before " + notes_sorted_by_line_index_[idx].friendly_note);
            else 
                logger.Info("[notes] adding " + n.friendly_note + " at the end");
        }

        // returns the ID assigned to this note that we're adding;
        // if null, something went wrong
        //
        // note_id - the unique ID of the note, or "" if a new unique ID is to be created
        //           the reason we have this is that we can persist notes (together with their IDs)
        private note_item add_note(line l, note n, string note_id, string reply_to_note_id, bool deleted, DateTime utc_added) {
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

            var new_ = new note_item(this, n, note_id, reply_to_note_id, line_id, deleted, utc_added);
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

                    var header = new note_item(this, next_guid, line_id, deleted, utc_added);
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

        /* we NEVER delete any note. We mark the note as DELETED, and based on "Show Deleted Notes", we can show them as well, or not.
            This simply eliminates the need to UNDO
        */
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
            bool is_single = same_line_count <= 2;
            if (is_single) {
                // in this case - either pressed on the Note itself, or on the header -> should still delete both lines
                foreach (var n in notes_sorted_by_line_index_)
                    if (n.line_id == line_id)
                        n.deleted = !n.deleted;
            }
            else {
                // in this case, pressed on a line header - toggle all its sub-children
                bool deleted = !toggle_note.deleted;
                toggle_note.deleted = deleted;
                foreach (var n in toggle_note.descendants())
                    n.deleted = deleted;
            }

            readd_everything();
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
            //
            // note: if any chidren of a deleted note/line is !deleted, we need to show that note/line (even if shown in gray)
            //       normally, this should not happen - however, I see this as a possibility when merging two notes files
            //       say X deletes note N, but Y replies on it
            foreach ( var n in copy) {
                if (n.is_note_header)
                    add_note_header(n.line_id, n.note_id, n.ui_deleted, n.utc_last_edited).deleted = n.deleted;
                else
                    add_note(lines_[n.line_id], n.the_note, n.note_id, n.reply_id, n.ui_deleted, n.utc_last_edited).deleted = n.deleted;
            }
            Debug.Assert(notes_sorted_by_line_index_.Count == copy.Count);

            // set current line
            if (showDeletedLines.Checked) {
                var cur_line_copy = new line();
                cur_line_copy.copy_from(cur_line);
                cur_line.clear();
                set_current_line_impl(cur_line_copy);
            } else
                cur_line.clear();

            string new_sel = showDeletedLines.Checked ? sel_note_id : sel_note_id_above;
            if ( new_sel != "")
                for ( int idx = 0; idx < notesCtrl.GetItemCount(); ++idx)
                    if ((notesCtrl.GetItem(idx).RowObject as note_item).note_id == new_sel) {
                        notesCtrl.SelectedIndex = idx;
                        notesCtrl.EnsureVisible(idx);
                        break;
                    }

            --ignore_change_;

            refresh_notes();
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

        public void set_current_line(line l) {
            set_current_line_impl(l);
            // for some extremely strange reason, we need to refresh the notes
            // otherwise, they would be shown as gray
            refresh_notes();
        }


        // this sets the line the user is viewing - by default, this is what the user is commenting on
        private void set_current_line_impl(line l) {
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
                if (!last_note_is_cur_line) {
                    var new_ = new note_item(this, next_guid, cur_line_id_, false, DateTime.UtcNow);
                    notesCtrl.AddObject(new_);
                    refresh_note( new_);
                }
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
                        if (i.utc_last_edited > last_note.utc_last_edited)
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

        private void add_color_for_author(string author_name, Color default_color) {
            if (author_colors_.ContainsKey(author_name))
                // already have it
                return;

            // these are the colors that are not used by existing authors at this point
            List<Color> new_colors = default_author_colors_.Where(x => x.ToArgb() != author_color_.ToArgb() && !author_colors_.Values.Contains(x) ).ToList();

            if (new_colors.Count > 0) {
                author_colors_.Add(author_name, new_colors[0]);
            } else 
                // at this point - way too many authors - we don't have enough colors - just use what he originally set
                author_colors_.Add(author_name, default_color);
        }

        private void update_author_colors() {
            // set author name / color first!
            Debug.Assert(author_name_ != "" && author_color_ != util.transparent);

            /*  by default, we prefer the current user's author color
                for the rest of the colors - just use them from the following array

                the reason I'm not using the author's selected color: the problem with that is that someone could select
                some color very close to another existing color, like - something realy close to blue, and the current user's color is blue.

                In this case, we'd end up having two DIFFERENT colors, but visually they would look very the same, thus the user 
                would need to look very careful to distinguish his notes from the other user's.

                In my solution, I just preserve the user's color. The other colors - use them from this list.
            */

            // any color used more than once - we'll have to end up using another color
            author_colors_.Clear();
            // current author - preserves his color
            author_colors_.Add(author_name_, author_color_);
            foreach ( var n in notes_sorted_by_line_index_)
                if (n.the_note != null) {
                    if (author_colors_.ContainsKey(n.the_note.author_name))
                        // we already know the color for this author
                        continue;

                    // it's a new author
                    add_color_for_author(n.the_note.author_name, n.the_note.author_color);
                }
        }

        private static Tuple<Dictionary<string, line>, List<note_item>> load_settings_file(note_ctrl self, string file_name) {
            Dictionary<string, line> lines = new Dictionary<string, line>();
            List<note_item> notes = new List<note_item>();

            settings_file sett = new settings_file(file_name);
            // first, load lines
            int line_count = int.Parse(sett.get("line_count", "0"));
            for (int idx = 0; idx < line_count; ++idx) {
                string prefix = "line." + idx + ".";
                line l = new line();
                l.idx = int.Parse(sett.get(prefix + "index", "0"));
                l.view_name = sett.get(prefix + "view_name");
                l.msg = sett.get(prefix + "msg");
                var id = sett.get(prefix + "id", "");
                lines.Add(id, l);
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
                    note = new note_item(self, n, note_id, reply_id, line_id, deleted, new DateTime(ticks));
                else
                    note = new note_item(self, note_id, line_id, deleted, new DateTime(ticks));
                notes.Add(note);
            }
            return new Tuple<Dictionary<string, line>, List<note_item>>(lines, notes);
        }

        public void load(string file_name) {
            // set the author color first!
            Debug.Assert(author_color_ != util.transparent);

            ++ignore_change_;
            if (file_name_ != "")
                // in this case, we're loading the notes from somewhere else, save existing first
                save();

            file_name_ = file_name;

            notes_sorted_by_line_index_.Clear();
            notesCtrl.ClearObjects();
            cur_line.clear();

            var loaded = load_settings_file(this, file_name);
            lines_ = loaded.Item1;
            lines_.Add(cur_line_id_, new line());

            foreach (var n in loaded.Item2) {
                if (n.the_note != null) {
                    add_color_for_author(n.the_note.author_name, n.the_note.author_color);
                    add_note(lines_[n.line_id], n.the_note, n.note_id, n.reply_id, n.deleted, n.utc_last_edited);
                } else
                    add_note_header(n.line_id, n.note_id, n.deleted, n.utc_last_edited);
            }
        
            // update last_note_id and last_line_id
            dirty_ = false;
            --ignore_change_;

            refresh_notes();
            notesCtrl.Refresh();
        }

        private Tuple<Dictionary<string, line>, List<note_item>> adjust_guids_before_merge(Tuple<Dictionary<string, line>, List<note_item>> merge_from) {
            // problem : two different people can enter a note on the same line
            //           in this case, each will get its own GUID for the line they added teh note on
            //           we need to merge the two GUIDs, since they literally point to the same line
            Dictionary<string,string> merge_to_local_line_id = new Dictionary<string, string>();
            foreach (var l in merge_from.Item1) {
                var local = lines_.FirstOrDefault(x => x.Value.idx == l.Value.idx);
                if ( local.Value != null)
                    merge_to_local_line_id.Add( l.Key, local.Key);
                else 
                    merge_to_local_line_id.Add( l.Key, l.Key);
            }

            Dictionary<string, line> updated_lines = merge_from.Item1.ToDictionary(x => merge_to_local_line_id[x.Key], x => x.Value);
            foreach (var n in merge_from.Item2)
                n.line_id = merge_to_local_line_id[n.line_id];

            return new Tuple<Dictionary<string, line>, List<note_item>>(updated_lines, merge_from.Item2);
        }

        public void merge(string other_file) {
            // create a backup, just in case something could go wrong
            save(file_name_ + ".backup." + DateTime.Now.Ticks);

            // update is_merged!
            ++ignore_change_;
            
            var merge_from = adjust_guids_before_merge( load_settings_file(this, other_file));
            // first, merge lines
            foreach ( var new_line in merge_from.Item1)
                if ( !lines_.ContainsKey(new_line.Key))
                    lines_.Add(new_line.Key, new_line.Value);

            // now, merge notes
            //
            foreach (var new_note in merge_from.Item2) {                
                if (!new_note.is_note_header) {
                    var found = notes_sorted_by_line_index_.FindIndex(x => x.note_id == new_note.note_id);
                    if (found != -1) {
                        // in this case, look at last_edited -> if bigger than ours, take that (someone else updated the entry after us)
                        Debug.Assert(notes_sorted_by_line_index_[found].line_id == new_note.line_id);
                        Debug.Assert(notes_sorted_by_line_index_[found].reply_id == new_note.reply_id);
                        if (new_note.utc_last_edited > notes_sorted_by_line_index_[found].utc_last_edited)
                            notes_sorted_by_line_index_[found] = new_note;
                    } else {
                        // it's a new note, just add it
                        // the way we keep the sorted lines (sorted by line index), we should never encounter a line with a reply pointing to an inexisting line
                        Debug.Assert(lines_.ContainsKey(new_note.line_id));
                        add_color_for_author(new_note.the_note.author_name, new_note.the_note.author_color);
                        add_note(lines_[new_note.line_id], new_note.the_note, new_note.note_id, new_note.reply_id, new_note.deleted, new_note.utc_last_edited);
                    }
                } else {
                    // it's a note header
                    // note headers are the same everywhere - so if found, ours is good
                    var found = notes_sorted_by_line_index_.FirstOrDefault(x => x.line_id == new_note.line_id);
                    if ( found == null)
                        add_note_header(new_note.line_id, new_note.note_id, new_note.deleted, new_note.utc_last_edited);
                }
            }
            dirty_ = false;
            --ignore_change_;

            // the reason we readd everything - the merged notes would end up being appended to the end, no matter what line they were appended to
            // we want to show them sorted by line
            readd_everything();
            notesCtrl.Refresh();

            save();
        }

        // saves the settings - you can specify another file name (in case you're making a backup)
        public void save(string file_name = "") {
            if (file_name == "")
                file_name = file_name_;

            // needs to be loaded first
            if (file_name == "")
                return;
            if (!dirty_)
                return;

            settings_file sett = new settings_file(file_name) { log_each_set = false };

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
                sett.set(prefix + "added_at", "" + n.utc_last_edited.Ticks);
            }
            sett.save();

            if ( file_name == file_name_)
                dirty_ = false;
        }

        private void refresh_note(note_item i) {
            notesCtrl.RefreshObject(i);

            var item = notesCtrl.ModelToItem(i);

            OLVListSubItem idx = item.GetSubItem(0), line = item.GetSubItem(1), text = item.GetSubItem(2);
            idx.ForeColor = i.idx_fg;
            line.ForeColor = i.line_fg;
            text.ForeColor = i.text_fg;

            idx.BackColor = line.BackColor = text.BackColor = i.bg;

            line.Font = i.the_note != null ? note_font_ : header_font_;
            text.Font = i.the_note != null ? note_font_ : header_font_;
        }

        private void refresh_note_indexes() {
            // so that the UI indexes (first column) are recomputed - in case of an insert
            for ( int idx = 0; idx < notesCtrl.GetItemCount(); ++idx)
                notesCtrl.RefreshObject( notesCtrl.GetItem(idx).RowObject );
        }

        public void refresh_notes() {
            // so that the UI indexes (first column) are recomputed - in case of an insert
            for ( int idx = 0; idx < notesCtrl.GetItemCount(); ++idx)
                refresh_note( notesCtrl.GetItem(idx).RowObject as note_item);
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
                    bool is_edit = i.the_note != null && i.the_note.author_name == author_name_;
                    if (is_edit) {
                        dirty_ = true;
                        i.the_note.note_text = note_text;
                        // user modified the note
                        i.utc_last_edited = DateTime.UtcNow;
                        refresh_note(i);
                        return;
                    }
                    bool is_reply = i.the_note != null && i.the_note.author_name != author_name_;
                    if (is_reply)
                        reply_to_note_id = i.note_id;
                }

                note new_ = new note() { author_name = author_name_, author_initials = author_initials_, 
                    author_color = author_color_, note_text = note_text, made_by_current_user = true, is_new = true };
                add_note(new_, "", reply_to_note_id, false, DateTime.UtcNow);
                refresh_notes();
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
                update_cur_note_controls();
                notesCtrl.Focus();
            }
        }

        private void notesCtrl_SelectedIndexChanged(object sender, EventArgs e) {
            if (ignore_change_ > 0)
                return;

            update_cur_note_controls();
            refresh_notes();
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
                    addNoteToLineLabel.Text = i.the_note.author_name == author_name_ ? "Edit Note to Line" : "Reply to " + i.the_note.author_initials;
                else
                    addNoteToLineLabel.Text = "Add Note to Line";
                addNoteToLine.Text = "[" + (lines_[i.line_id].idx + 1) + "]";
                curNote.Text = i.the_note != null ? i.the_note.note_text : "";
                curNote.Enabled = true;
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
            util.postpone( () => notesCtrl.Focus(), 10);
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

        private Color author_color(string name) {
            Debug.Assert(author_colors_.ContainsKey(name));
            return author_colors_[name];
        }

        private void curNote_Enter(object sender, EventArgs e) {
            int sel = notesCtrl.SelectedIndex;
            if (sel < 0) {
                Debug.Assert(false);
                curNote.Enabled = false;
                return;
            }

            var i = notesCtrl.GetItem(sel).RowObject as note_item;
            if (i.the_note != null && i.the_note.author_name != author_name_) {
                ++ignore_change_;
                curNote.Text = "";
                --ignore_change_;
            }
        }


    }
}
