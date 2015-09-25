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
using System.Text;
using System.Windows.Forms;

namespace lw_common.ui {
    public partial class note_ctrl : UserControl {

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

            public bool deleted = false;

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

            public int idx = -1;
            public string view_name = "";
            public string msg = "";
        }

        private class note_item {
            public readonly note_ctrl self = null;

            // if null, it's teh header
            public readonly note the_note = null;

            // each note has a unique ID -this can be used when adding a note that is in reply to someone
            public readonly int note_id;

            // in case this is a note that is in reply to another note, this contains the ID of the other note we're replying to
            public readonly int reply_id;

            // pointer to a line - each line has a unique ID (different from its index)
            public readonly int line_id;

            // when was this note added (could be useful when sorting by time)
            public readonly DateTime utc_added = DateTime.UtcNow;

            // if true, this is the header (information about the line itself)
            public bool is_note_header {
                get { return the_note == null; }
            }

            public readonly bool is_cur_line = false;

            public int indent {
                get {
                    int ind = 0;
                    note_item cur = this;
                    while (cur.reply_id > 0) {
                        ++ind;
                        cur = self.notes_sorted_by_line_id_.FirstOrDefault(x => x.note_id == reply_id);
                        if (cur == null)
                            break;
                    }
                    return ind;
                }
            }


            public string text {
                get {
                    // gets the note that is printed on screen - on the list
                    // ignore #... at beginning of msg
                    return "";
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

            public note_item(note_ctrl self, note n, int unique_id, int reply_id, int line_id) {
                the_note = n;
                this.self = self;
                this.note_id = unique_id;
                this.line_id = line_id;
                this.reply_id = reply_id;
            }

            public note_item(note_ctrl self, int line_id, bool is_cur_line) {
                this.self = self;
                this.line_id = line_id;
                this.is_cur_line = is_cur_line;
                the_note = null;
            }

        }

        private Dictionary<int, line> lines_ = new Dictionary<int,line>();
        // this is always the last note - in case we're on a NEW line, this will contain the new line (so that the user visually sees what he's about to make a note on)
        //                                if we're on an existing line, this wil always be null
        private note_item last_note_ = null;

        // this is the note corresponding to the selected line in the current view/full log
        private note_item selected_log_line_ = null;

        // the are kept as if they were sorted by line, then time - first the line (header), then all the notes related to that line
        private List<note_item> notes_sorted_by_line_id_ = new List<note_item>();

        private int next_note_id_ = 0, next_line_id_ = 0;

        private line cur_line_ = null;

        private bool dirty_ = false;

        public note_ctrl() {
            InitializeComponent();
        }

        private void note_ctrl_Load(object sender, EventArgs e) {
        }



        public void set_author(string author_name, string author_initials, Color notes_color) {
            author_name_ = author_name;
            author_initials_ = author_initials;
            author_color_ = notes_color;
            // update anything that was added by me before the changes were made
            foreach (note_item n in notes_sorted_by_line_id_)
                if (n.the_note != null && n.the_note.made_by_current_user) {
                    n.the_note.author_name = author_name;
                    n.the_note.author_initials = author_initials;
                    n.the_note.author_color = notes_color;
                }
            notesCtrl.Refresh();
        }

        // helper - add to the currently selected line
        public int add_note(note n, int note_id, int reply_to_note_id) {
            Debug.Assert(cur_line_ != null);
            if (cur_line_ == null)
                return -1;

            return add_note(cur_line_, n, note_id, reply_to_note_id);
        }

        // returns the ID assigned to this note that we're adding;
        // if -1, something went wrong
        //
        // note_id - the unique ID of the note, or -1 if a new unique ID is to be created
        //           the reason we have this is that we can persist notes (together with their IDs)
        public int add_note(line l, note n, int note_id, int reply_to_note_id) {
            // a note: can be added on a line, or as reply to someone
            // new notes: always at the end
            if (selected_log_line_ == null) {
                // the user has not selected any line at this time
                Debug.Assert(false);
                return -1;
            }

            int line_id = -1;
            var found_line = lines_.FirstOrDefault(x => x.Value == l);
            if (found_line.Value == null) {
                // could not find the line
                if (cur_line_ == l) {
                    // it's the current line
                    line_id = ++next_line_id_;
                    lines_.Add(line_id, l);
                } 
                else {
                    Debug.Assert(false);
                    return -1;
                }
            }
            line_id = found_line.Key;

            note_id = note_id >= 0 ? note_id : ++next_note_id_;
            // ... this note should not exist already
            Debug.Assert( notes_sorted_by_line_id_.Count(x => x.note_id == note_id) == 0);
            if ( reply_to_note_id >= 0)
                // note we're replying to should exist already
                Debug.Assert( notes_sorted_by_line_id_.Count(x => x.note_id == reply_to_note_id) == 1);

            var new_ = new note_item(this, n, note_id, reply_to_note_id, selected_log_line_.line_id);
            if (next_note_id_ <= note_id)
                next_note_id_ = note_id + 1;

            bool inserted = false;
            if (reply_to_note_id > 0) {
                // add this note as the last reply to this reply note
                var note = notes_sorted_by_line_id_.FirstOrDefault(x => x.line_id == line_id);
                if (note != null) {
                    Debug.Assert(note.is_note_header);
                    // everything following this, is related to this line (until we get to next line)
                    int idx_note = notes_sorted_by_line_id_.IndexOf(note);
                    for (; idx_note < notes_sorted_by_line_id_.Count && !inserted; idx_note++)
                        if (notes_sorted_by_line_id_[idx_note].line_id == line_id) {
                            if (reply_to_note_id > 0) {
                                // in this case, look for this note (that we're replying to)
                                if (notes_sorted_by_line_id_[idx_note].note_id == reply_to_note_id) {
                                    notes_sorted_by_line_id_.Insert(idx_note + 1, new_);
                                    inserted = true;
                                }
                            } else {
                                // look for the last note about this line, and insert it after that
                                if ( idx_note < notes_sorted_by_line_id_.Count - 1)
                                    if (notes_sorted_by_line_id_[idx_note + 1].line_id != line_id) {
                                        // found the last note that relates to this line
                                        notes_sorted_by_line_id_.Insert(idx_note + 1, new_);
                                        inserted = true;                                        
                                    }
                            }
                        } else
                            // went to next line
                            break;
                } else {
                    // this is the first entry that relates to this line
                    // ... find the note beore which we should insert ourselves
                    note = notes_sorted_by_line_id_.FirstOrDefault(x => x.line_id > line_id);
                    if (note != null) {
                        int idx = notes_sorted_by_line_id_.IndexOf(note);
                        notes_sorted_by_line_id_.Insert(idx, new_);
                        inserted = true;
                    } else {
                        notes_sorted_by_line_id_.Add(new_);
                        inserted = true;
                    }
                }
            }
            Debug.Assert(inserted);

            // update the UI
            if (inserted)
                update_ui();

            dirty_ = true;
            return new_.note_id;
        }

        private void update_ui() {
            
            // care about sorting
            // also, new lines are always added last!!!!!!!!!!!!!!
            // also, care about selected_log_line_ / last_line_ ???

                if (showDeletedLines.Checked)
                    ;

            // if i delete a note from line ,and no other notes on that line:
            // if it was from current user, delete the line note as well ; otherwise, just hide it
        }


        public void del_note(int note_id) {
            // note: delete someone else's note -> it's grayed (hidden)
            //       delete your note: it's fully deleted + copied to clipboard -> ALL THE notes you delete - keep them internally and the user should be able to access them
            //       (just in case he did a mistake)

            dirty_ = true;
        }

        // this sets the line the user is viewing - by default, this is what the user is commenting on
        public void set_cur_line(line l) {
            dirty_ = true;
            // if no notes, perhaps show the current line in teh view - just for the user to see what he would be commenting on? tothink
            // perhaps I should always show the current line as the last line possible (so that the user would know he'd be editing it?)

            //  ......... if last line is "is_cur_line", update it; otherwise, insert

            // if already there + selection relates to it, don't change the selection 
            cur_line_ = l;
        }

        // when saving a line - save the line index + the view it was in





        // notifies the views of what the user has selected (what line / view)
        public void sync_to_views() {
            // FIXME            
        }

        // gets notified when the view and/or line has changed
        public void sync_from_views() {
            // FIXME            
        }

        public void undo() {
            // FIXME

            // Ctrl-Z -> write a msg "All your deleted notes have been copied to clipboard - first your own, then the ones from the other authors"
        }

        public void load(string file_name) {
        
            // update last_note_id and last_line_id
            dirty_ = false;
        }

        public void save() {
            if (!dirty_)
                return;
            // needs to be loaded first
            dirty_ = false;
        }

    }
}
