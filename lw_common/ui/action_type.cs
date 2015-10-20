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

        // 1.3.26d+ - toggle showing just what matches a filter function on/off
        toggle_filter_view,

        // 1.3.26+ - toggle showing full log on/off
        toggle_show_full_log,

        none,
    }

}
