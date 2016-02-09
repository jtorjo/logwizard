using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui {
    public class category_colors {
        // the name of the category
        public string name = "";

        // the background for this category - if transparent, LogWizard will choose something
        //
        // this is used per-se only for computing the background colors (same_category_bg and this_category_bg)
        private Color bg_color_ = util.transparent;

        // the background for the rows that are of the same category
        // if tranasparent, I compute it from bg_color
        private Color raw_same_category_bg_ = util.transparent;
            
        // the background for the rows that are of the same category as the selected row
        private Color raw_this_category_bg_ = util.transparent;

        public Color bg_color {
            get { return bg_color_; }
            set {
                bg_color_ = value;
                if (bg_color != util.transparent) 
                    // at this point, they're computed from the bg_color
                    raw_same_category_bg_ = raw_this_category_bg_ = util.transparent;                    
            }
        }

        private static Color same_category_color(Color bg) {
            return util.color_luminance(bg, 0.96) ;
        }
        private static Color this_category_color(Color bg) {
            return util.color_luminance(bg, 0.9) ;
        }

        public Color same_category_bg {
            get { return raw_same_category_bg != util.transparent ? raw_same_category_bg : same_category_color(bg_color); }
        }

        public Color this_category_bg {
            get { return raw_this_category_bg != util.transparent ? raw_this_category_bg : this_category_color(bg_color); }
        }

        public Color raw_same_category_bg {
            get { return raw_same_category_bg_; }
            set { raw_same_category_bg_ = value; }
        }

        public Color raw_this_category_bg {
            get { return raw_this_category_bg_; }
            set { raw_this_category_bg_ = value; }
        }
    }
}
