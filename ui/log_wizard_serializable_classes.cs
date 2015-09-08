using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWizard {
    [Serializable]
    public class ui_filter {
        // the filter itself
        public string text = "";
        // if true, it's enabled
        public bool enabled = true;
        // if !enabled, but dimmed, the filter acts the same, only that it shows the lines pertaining to it as gray (dimmed)
        public bool dimmed = false;

        public bool apply_to_existing_lines = false;
    }

    [Serializable]
    public class ui_view {
        // friendlly name
        public string name = "";
        // the filters
        public List< ui_filter > filters = new List<ui_filter>();
    }

    [Serializable]
    public class ui_context {
        public string name  = "";
        public string auto_match = "";

        // show/hide toggles
        public bool show_filter = true;
        public bool show_source = true;
        public bool show_fulllog = false;

        public List<ui_view> views = new List<ui_view>();

        public void copy_from(ui_context other) {
            name = other.name;
            auto_match = other.auto_match;
            views = other.views.ToList();
            show_filter = other.show_filter;
            show_source = other.show_source;
            show_fulllog = other.show_fulllog;
        }
    }
}
