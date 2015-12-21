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
 *
 * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace lw_common.ui {

    internal class full_log_match_item : match_item {
        private readonly log_view parent_ = null;

        public full_log_match_item(BitArray matches, font_info font, line line, int lineIdx, log_view parent) : base(matches, font, line, lineIdx, parent) {
            Debug.Assert(parent != null);
            parent_ = parent;
        }

        public override string view {
            get {
                return parent_.lv_parent.matched_logs(line_idx);
            }
        }

        public override Color fg(log_view parent) {
            var result = parent.lv_parent.full_log_row_colors(line_idx).Item1;
            if (result == util.transparent)
                result = app.inst.fg;
            return result;
        }

        public override Color bg(log_view parent) {
            var result = parent.lv_parent.full_log_row_colors(line_idx).Item2;
            if (result == util.transparent)
                result = app.inst.bg;
            return result;
        }
    }

}
