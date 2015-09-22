using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogWizard.ui {
    public partial class notes_ctrl : UserControl {
        public notes_ctrl() {
            InitializeComponent();
        }

        // when saving a line - save the line index + the view it was in


        // note: delete someone else's note -> it's grayed (hidden)
        //       delete your note: it's fully deleted + copied to clipboard -> ALL THE notes you delete - keep them internally and the user should be able to access them
        //       (just in case he did a mistake)
        //       
        // Ctrl-Z -> write a msg "All your deleted notes have been copied to clipboard - first your own, then the ones from the other authors"

        // notifies the views of what the user has selected (what line / view)
        public void sync_to_views() {
            
        }

        // gets notified when the view and/or line has changed
        public void sync_from_views() {
            
        }

        public void load(string file_name) {
            
        }

        public void save() {
            // needs to be loaded first
        }
    }
}
