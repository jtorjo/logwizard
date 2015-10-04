using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui {
    class solid_brush_list {
        private Dictionary<Color, Brush> brushes_ = new Dictionary<Color, Brush>();

        public Brush brush(Color c) {
            if (brushes_.ContainsKey(c))
                return brushes_[c];

            brushes_.Add(c, new SolidBrush(c));
            return brushes_[c]; 
        }
    }
}
