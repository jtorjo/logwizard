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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui {
    class gradient_brush_list {
        private Dictionary< Tuple<Rectangle,Color,Color> , LinearGradientBrush > brushes_ = new Dictionary<Tuple<Rectangle,Color, Color>, LinearGradientBrush>();

        public Brush brush(Rectangle r, Color from, Color to) {
            var c = new Tuple<Rectangle,Color,Color>(r,from,to);
            if (brushes_.ContainsKey(c))
                return brushes_[c];

            brushes_.Add(c, new LinearGradientBrush(r, from, to, 0.0));
            return brushes_[c];
        }
    }
}
