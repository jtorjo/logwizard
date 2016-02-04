using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.ui.format.column_formatters {
    /* 
        pic=prefix->name
        pic2=prefix->name
        ...


    name can be a name that is relative to homepath (%appdata%\..\local) or absolute
    */
    class picture : column_formatter_base {

        private Dictionary<string, Image> name_to_picture_ = new Dictionary<string, Image>();

        private Image picture_ = null;

        internal override void load_syntax(settings_as_string sett, ref string error) {
            base.load_syntax(sett, ref error);

            for (int idx = 0;; ++idx) {
                string sett_name = "pic" + (idx > 0 ? "" +(idx+1) : "");
                var pic = sett.get(sett_name);
                if (pic != "") {
                    var sep = pic.IndexOf("->");
                    if (sep >= 0) {
                        string prefix = pic.Substring(0, sep).Trim().ToLower();
                        string file = pic.Substring(sep + 2).Trim();
                        if (prefix == "" || file == "")
                            continue;
                        try {
                            if (!Path.IsPathRooted(file))
                                file = util.personal_dir() + "LogWizard\\" + file;

                            if (File.Exists(file)) {
                                Image bmp = Image.FromFile(file);
                                if (bmp != null)
                                    name_to_picture_.Add(prefix, bmp);
                            }
                        } catch {
                            error = "Invalid file: " + file;
                        }
                    } else
                        error = "Invalid line: " + pic;
                } else
                    break;
            }
        }

        internal override void format_before_do_replace(format_cell cell) {
            string text = cell.format_text.text.ToLower();
            var exists = name_to_picture_.Keys.FirstOrDefault(x => text.StartsWith(x));
            picture_ = exists != null ? name_to_picture_[exists] : null;
            if ( picture_ != null)
                cell.format_text.replace_text(0, cell.format_text.text.Length, "");
        }

        internal override void format_before(format_cell cell) {
        }

        internal override void format_after(format_cell cell) {
        }

        internal override Image get_image() {
            return picture_;
        }
    }
}
