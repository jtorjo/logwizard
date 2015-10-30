using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace lw_common.ui {
    class font_list {
        private Dictionary<string, Font> fonts_ = new Dictionary<string, Font>();

        public Font get_font(Font f, bool bold, bool italic) {
            string id = font_to_string(f, bold, italic);
            if (!fonts_.ContainsKey(id))
                fonts_.Add(id, create_new(f.Name, (int)f.Size, bold, italic));
            return fonts_[id];
        }

        public Font get_font(string font_name, int size, bool bold, bool italic) {
            string id = font_to_string(font_name, size, bold, italic);
            if (!fonts_.ContainsKey(id))
                fonts_.Add(id, create_new(font_name, size, bold, italic));
            return fonts_[id];
        }

        private string font_to_string(Font f) {
            return font_to_string(f.Name, (int) f.Size, f.Bold, f.Italic);
        }
        private string font_to_string(Font f, bool bold, bool italic) {
            return font_to_string(f.Name, (int) f.Size, bold, italic);
        }
        private string font_to_string(string font_name,int size, bool bold, bool italic) {
            return font_name + "|" + size + "|" + bold + "|" + italic;
        }

        private Font create_new(string font_name, int size, bool bold, bool italic) {
            FontStyle style = FontStyle.Regular;
            if (bold)
                style = style | FontStyle.Bold;
            if (italic)
                style = style | FontStyle.Italic;
            return new Font(font_name, size, style);
        }
    }
}
