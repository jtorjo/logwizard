/* 
 * Copyright (C) 2014-2016 John Torjo
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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogWizard;

namespace lw_common.ui {

    public enum log_view_sel_change_type {
        search,
        bookmark,
        click,
        // user pressed backspace, thus we go back to the cell containing [search-1]
        backspace
    }

    public interface log_view_parent {
        void handle_subcontrol_keys(Control c);
        void on_view_name_changed(log_view view, string name);
        void on_sel_line(log_view lv, int line_idx);
        void on_edit_aliases();

        string matched_logs(int line_idx);
        Rectangle client_rect_no_filter { get; }

        // read-only
        ui_info current_ui { get; }

        // returns the log that contains all lines
        log_view full_log { get; }

        void simple_action(log_view_right_click.simple_action simple);

        void add_or_edit_filter(string filter_str, string filter_id, bool apply_to_existing_lines);

        // called after we've searched to something (thus, changed the current line)
        void sel_changed(log_view_sel_change_type change);

        void select_filter_rows(List<int> filter_row_indexes);

        void edit_filter_row(int filter_row_idx);

        List<Tuple<string, int>> other_views_containing_this_line(int row_idx);
        void go_to_view(int view_idx);

        Tuple<Color, Color> full_log_row_colors(int line_idx);

        // called when extra filter has changed, or show-all-lines has changed
        void after_set_filter_update();

        // if >= 0, it's the selected row (the one is FOCUSED on - in the filter control)
        int selected_filter_row_index { get; }

        bool can_edit_context { get; }

        void edit_log_settings();

        void after_column_positions_change();

        void needs_details_pane();

        // these are the columns that are visible in the description pane (if it's shown)
        List<info_type> description_columns();
    }
}