using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Layout;

namespace lw_common.ui.format.column_formatters {
    // formats the whole cell
    class cell : column_formatter_base {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // extra formatting, if any
        private text_part formatting_ = null;

        private Image selection_image_ = null;
        private Image bookmark_image_ = null;
        private Image selection_and_bookmark_image_ = null;

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);
            var format = sett.get("format");
            if ( format != "")
                formatting_ = text_part.from_friendly_string(format);

            var sel = sett.get("selection");
            var bookmark = sett.get("bookmark");
            if (sel != "")
                sel = util.absolute_logwizard_filename(sel);
            if (bookmark != "")
                bookmark = util.absolute_logwizard_filename(bookmark);

            if ( sel != "")
                try {
                    selection_image_ = Image.FromFile(sel);
                } catch(Exception e) {
                    logger.Error("bad picture " + e.Message);
                    error = "Bad image file: " + sel;
                }
            if ( bookmark != "")
                try {
                    bookmark_image_ = Image.FromFile(bookmark);
                } catch(Exception e) {
                    logger.Error("bad picture " + e.Message);
                    error = "Bad image file: " + bookmark;
                }

            if (selection_image_ != null && bookmark_image_ != null)
                selection_and_bookmark_image_ = util.merge_images_horizontally(selection_image_, bookmark_image_);
        }

        internal override void format_before(format_cell cell) {
            var text = cell.format_text.text;
            if (formatting_ != null && formatting_.bg != util.transparent)
                cell.format_text.bg = formatting_.bg;
        }

        internal override void format_after(format_cell cell) {
            var text = cell.format_text.text;
            if ( formatting_ != null)
                cell.format_text.add_part(new text_part(0, text.Length, formatting_) );

            bool is_sel = cell.row_index == cell.sel_index;
            if (is_sel && cell.is_bookmark && selection_and_bookmark_image_ != null)
                cell.format_text.image = selection_and_bookmark_image_;
            else if ( is_sel && selection_image_ != null)
                cell.format_text.image = selection_image_;
            else if (cell.is_bookmark && bookmark_image_ != null)
                cell.format_text.image = bookmark_image_;
        }
    }
}
