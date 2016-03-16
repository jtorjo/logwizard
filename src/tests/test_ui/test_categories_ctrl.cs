using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorPicker;
using lw_common;
using lw_common.ui;

namespace test_ui {
    public partial class test_categories_ctrl : Form {
        public test_categories_ctrl() {
            InitializeComponent();

            ColorTable.copy_color_to_clipboard = true;

            categories.on_change_category_type += On_change_category_type;
            categories.set_category_types( new List<string>() { "thread", "level", "date", "err1", "err2" }, "thread" );
            categories.on_category_colors_change += On_category_colors_change;
        }

        private void On_category_colors_change(List<category_colors> colors) {
            
        }

        private void On_change_category_type(string category_type) {
            switch (category_type) {
            case "thread":
                categories.set_categories( new List<category_colors>() {
                    new category_colors() { name = "back_thread" },
                    new category_colors() { name = "scraper" },
                    new category_colors() { name = "main" },
                    new category_colors() { name = "secondary" },
                });
                break;
            case "level":
                categories.set_categories( new List<category_colors>() {
                    new category_colors() { name = "info", bg_color = util.str_to_color("green") },
                    new category_colors() { name = "dbg", bg_color = util.str_to_color("blue") },
                    new category_colors() { name = "error", bg_color = util.str_to_color("red") },
                    new category_colors() { name = "warn", bg_color = util.str_to_color("yellow") },
                    new category_colors() { name = "trace", bg_color = util.str_to_color("violet"), raw_same_category_bg = util.str_to_color("pink"), raw_this_category_bg = util.str_to_color("coral") },
                    new category_colors() { name = "fatal", bg_color = util.str_to_color("orange") },
                });
                break;
            case "date":
                categories.set_categories( new List<category_colors>() {
                    new category_colors() { name = "yesterday", bg_color = util.str_to_color("red") },
                    new category_colors() { name = "today", bg_color = util.str_to_color("green") },
                    new category_colors() { name = "tomorrow", bg_color = util.str_to_color("blue") },
                });
                break;
            case "err1":
            case "err2":
                categories.set_error("Too many different values!");
                break;
            }
        }
    }
}
