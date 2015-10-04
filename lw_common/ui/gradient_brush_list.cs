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
