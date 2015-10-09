using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lw_common {

    public enum action_type {
        search, search_prev, search_next, 

        // a search with defaults + what is selected
        default_search,
        
        // 1.2.6+ - now, this can do a lot of stuff, depending on the context
        escape,

        next_view, prev_view,
        home, end,
        pageup, pagedown,
        arrow_up, arrow_down,
        toggle_filters, toggle_source, toggle_fulllog, toggle_notes,

        toggle_bookmark, next_bookmark, prev_bookmark, clear_bookmarks,

        // 1.0.25+
        copy_to_clipboard, copy_full_line_to_clipboard,

        // 1.0.28+
        pane_next, pane_prev,

        // 1.0.28+
        toggle_history_dropdown,
        new_log_wizard, show_preferences,

        // 1.0.35+
        increase_font, decrease_font,
        toggle_show_msg_only,
        // ... equivalent of ctrl-up/down
        scroll_up, scroll_down,

        // 1.0.52+
        go_to_line,

        // 1.0.53
        refresh,

        // 1.0.56
        toggle_title,

        // 1.0.77+ - (show/hide the tabs themselves) ; show the tab name in the "Message" column itself
        toggle_view_tabs,

        // 1.0.70+ - toggle the header (show/hide)
        toggle_view_header,

        // 1.0.77 - show/hide details view - note: I've not implemented "details" view yet
        toggle_details,

        toggle_status,

        // 1.0.67
        open_in_explorer,

        // 1.0.77+ default behavior does not work correctly
        shift_arrow_up, shift_arrow_down,

        // 1.0.80+ - allow saving the current position + toggles + current view
        goto_position_1, 
        goto_position_2, 
        goto_position_3, 
        goto_position_4, 
        goto_position_5,
 
        toggle_enabled_dimmed,

        undo,

        export_notes,

        right_click_via_key,

        none,
    }

}
