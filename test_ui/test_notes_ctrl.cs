using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lw_common.ui;

namespace test_ui {
    public partial class test_notes_ctrl : Form {

        public class dummy_note {
            public int idx = 0;
            public string view = "";
            public string msg = "";
        }

        public test_notes_ctrl() {
            InitializeComponent();
            notes.set_author("John Torjo", "jt", Color.Blue);
            //notes.set_author("Denny", "dd", Color.Blue);
            //notes.set_author("Vlad", "vv", Color.Blue);

            notes.on_note_selected = on_note_selected;

            dummyView.AddObject( new dummy_note() { idx = 5, view = "err", msg = "this is error 1" });
            dummyView.AddObject( new dummy_note() { idx = 6, view = "err", msg = "alig is in the house" });
            dummyView.AddObject( new dummy_note() { idx = 7, view = "err", msg = "this is error 2" });
            dummyView.AddObject( new dummy_note() { idx = 8, view = "time", msg = "first time" });
            dummyView.AddObject( new dummy_note() { idx = 15, view = "time", msg = "second time is easier" });
            dummyView.AddObject( new dummy_note() { idx = 25, view = "time", msg = "third is a charm" });
            dummyView.AddObject( new dummy_note() { idx = 35, view = "time", msg = "fourth is good" });
            dummyView.AddObject( new dummy_note() { idx = 45, view = "time", msg = "fifth is awesome" });
            dummyView.AddObject( new dummy_note() { idx = 55, view = "time", msg = "practice makes perfect" });
            dummyView.AddObject( new dummy_note() { idx = 65, view = "time", msg = "[hk] key handler created - 410D2" });
            dummyView.AddObject( new dummy_note() { idx = 75, view = "time", msg = "RelRects : debug mode" });
            dummyView.AddObject( new dummy_note() { idx = 105, view = "pots", msg = "Notification: Listening on hotkeys..." });
            dummyView.AddObject( new dummy_note() { idx = 115, view = "pots", msg = "load_save - on_change not implemented - enable_vision" });
            dummyView.AddObject( new dummy_note() { idx = 116, view = "pots", msg = "load_save - on_change not implemented - sta_type1" });
            dummyView.AddObject( new dummy_note() { idx = 117, view = "pots", msg = "load_save - on_change not implemented - multi_stack_default_as_str" });
            dummyView.AddObject( new dummy_note() { idx = 125, view = "err", msg = "[find] compute_pots, scrapes= 14 - 295ms." });
            dummyView.AddObject( new dummy_note() { idx = 135, view = "err", msg = "No glyphs found when scraping pot on ps.eu Semiramis/9113E" });
            dummyView.AddObject( new dummy_note() { idx = 145, view = "err", msg = "[pot] Pot string could not be parsed [] : Input string was not in a correct format." });
            dummyView.AddObject( new dummy_note() { idx = 155, view = "err", msg = "[pot] Pot string could not be parsed [__] : Input string was not in a correct format." });
            dummyView.AddObject( new dummy_note() { idx = 156, view = "err", msg = "this is the last error" });

            notes.load("test_notes_ui.txt");
        }

        private void on_note_selected(int line_idx, string msg) {
            for ( int idx = 0; idx < dummyView.GetItemCount(); ++idx)
                if ((dummyView.GetItem(idx).RowObject as dummy_note).idx == line_idx+1) {
                    dummyView.SelectedIndex = idx;
                    return;
                }

            Debug.Assert(false);
        }

        private void dummyView_SelectedIndexChanged(object sender, EventArgs e) {
            int sel = dummyView.SelectedIndex;
            if (sel >= 0) {
                var note = dummyView.GetItem(sel).RowObject as dummy_note;
                notes.set_current_line( new note_ctrl.line { idx = note.idx -1, msg = note.msg, view_name = note.view });
                notes.refresh_notes();
            }
        }

        private void undo_Click(object sender, EventArgs e) {
            notes.undo();
        }
    }
}
