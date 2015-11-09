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
 *  * **** Get Latest version at https://github.com/jtorjo/logwizard **** 
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using lw_common;

namespace test_ui {
    class test_export {

        public static void test() {
            export_text e = new export_text();

            e.add_cell(new export_text.cell(0,0, "first cell"));
            e.add_cell(new export_text.cell(1,1, "cell[1,1]") { bg = Color.Blue });

            e.add_cell(new export_text.cell(2,1, "time") {bg = Color.CornflowerBlue, fg = Color.Red});
            e.add_cell(new export_text.cell(3,1, "time"));
            e.add_cell(new export_text.cell(4,1, "time"));
            e.add_cell(new export_text.cell(5,1, "err"));
            e.add_cell(new export_text.cell(6,1, "err"));

            e.add_cell(new export_text.cell(2,2, "pot"));
            e.add_cell(new export_text.cell(3,2, "pot"));
            e.add_cell(new export_text.cell(4,2, "pot"));
            e.add_cell(new export_text.cell(5,2, "betty"));
            e.add_cell(new export_text.cell(6,2, "lasting"));

            e.add_cell(new export_text.cell(2,3, "load_save - on_change not implemented - enable_vision") { font_size = 9, font = "Arial", fg = Color.Blue} );
            e.add_cell(new export_text.cell(3,3, "[find] compute_pots, scrapes= 14 - 295ms.") { font_size = 10, font = "Tahoma", fg = Color.Blue});
            e.add_cell(new export_text.cell(4,3, "[pot] Pot string could not be parsed [] : Input string was not in a correct format.") { font_size = 10, font = "Courier New", fg = Color.Blue});
            e.add_cell(new export_text.cell(5,3, "this is the last error") { font_size = 12, font = "Courier New", fg = Color.CornflowerBlue});
            e.add_cell(new export_text.cell(6,3, "[hk] key handler created - 410D2") { font_size = 13, font = "Courier New", fg = Color.Red});

            string txt = e.to_text();
            File.WriteAllText("out.txt", txt);
            string html = e.to_html();
            File.WriteAllText("out.html", html);

            clipboard_util.copy(html, txt);
        }

    }
}
