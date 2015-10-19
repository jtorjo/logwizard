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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;

namespace lw_common {
    // allows exporting text to different formats
    public class export_text {
        public class cell {
            public readonly int row;
            public readonly int col;
            public readonly string text;
            public cell(int row, int col, string text) {
                this.row = row;
                this.col = col;
                this.text = text;
            }

            public string font = "";
            public bool bold = false;
            public int font_size = 0;

            public Color bg = util.transparent, fg = util.transparent;
        }

        private int columns_ = 0;
        private List<List<cell>> cells_ = new List<List<cell>>();

        public void add_cell(cell c) {
            while (cells_.Count <= c.row) {
                cells_.Add(new List<cell>());
                for ( int i = 0; i < columns_; ++i)
                    cells_.Last().Add(null);
            }

            if (columns_ <= c.col) {
                foreach (var row in cells_)
                    while (row.Count <= c.col)
                        row.Add(null);
                columns_ = c.col + 1;
            }

            cells_[c.row][c.col] = c;
        }

        private int column_max_len(int col) {
            Debug.Assert(col < columns_);
            if (cells_.Count < 1 || columns_ < 1)
                return 0;

            return cells_.Max(x => x[col] != null ? x[col].text.Length : 0);
        }

        public string to_text() {
            List<int> lengths = new List<int>();
            for ( int col = 0; col < columns_; ++col)
                lengths.Add( column_max_len(col));

            StringBuilder txt = new StringBuilder();
            foreach (var row in cells_) {
                for (int col = 0; col < columns_; ++col) {
                    int cell_len = 0;
                    if (row[col] != null) {
                        txt.Append( row[col].text);
                        cell_len = row[col].text.Length;
                    }

                    if ( col < columns_ - 1)
                        txt.Append(' ', lengths[col] + 1 - cell_len);
                }
                txt.Append("\r\n");
            }
            return txt.ToString();
        }

        private int html_font_size(int size) {
            if (size <= 7) return 2;
            if (size <= 9) return 3;
            if (size < 12) return 4;
            return 5;
        }

        public string to_html() {
            string prefix = "<html> <body> <table style=\"width:100%\">";
            string suffix = "</table> </body> </html>";

            
            StringBuilder txt = new StringBuilder();
            foreach (var row in cells_) {
                txt.Append(" <tr> ");
                for (int col = 0; col < columns_; ++col) {
                    string attributes = "", font_prefix = "";
                    if (row[col] != null) {
                        cell c = row[col];
                        if (c.bg != util.transparent)
                            attributes += " bgcolor=\"" + util.color_to_str(c.bg) + "\"";
                        if (c.fg != util.transparent) 
                            font_prefix += " color=\"" + util.color_to_str(c.fg) + "\"";
                        if (c.font_size > 0) 
                            font_prefix += " size=" + html_font_size(c.font_size);
                        if (c.font != "")
                            font_prefix += " face=\"" + c.font + "\"";
                    }

                    txt.Append(" <td" + (attributes != "" ? " " + attributes + " " : "") + ">");
                    if (row[col] != null) {
                        if (font_prefix != "")
                            txt.Append("<font " + font_prefix + ">");
                        txt.Append( WebUtility.HtmlEncode( row[col].text) );
                        if (font_prefix != "")
                            txt.Append("</font>");
                    }
                    txt.Append("</td> ");
                }
                txt.Append(" </tr> ");
            }

            return prefix + txt + suffix;
        }
    }
}
