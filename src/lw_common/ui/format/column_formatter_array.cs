using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lw_common.ui.format.column_formatters;

namespace lw_common.ui.format {
    /* 
    Syntax:

    For each formatter:

    [column-type]
    formatter_name1
    formatter_syntax1
    formatter_syntax2
    ...
    formatter_syntaxN
    empty_line 
    formatter_name2
    formatter_syntax1
    formatter_syntax2
    ...
    formatter_syntaxN
    empty_line 
    ...
    formatter_nameN
    ...

    column-type
    - special name: "all" => apply it to all columns


    */
    public class column_formatter_array {
        private class formatter {
            // ... such as "all", "time", "msg", "ctx1", etc.
            public string column_type = "";
            public string name = "";
            public string syntax = "";
            public column_formatter_base the_formatter = null;

            public bool ok {
                get { return name != "";  }
            }
        }

        private List<formatter> formatters_ = new List<formatter>();
        private string syntax_ = "";

        public string syntax {
            get { return syntax_; }
        }

        public void load(string syntax) {
            string errors = "";
            load(syntax, ref errors);
        }

        public void load(string syntax, ref string errors) {
            syntax_ = syntax;
            errors = "";
            var formatters = new List<formatter>();
            var last = new formatter();
            foreach (string line in syntax.Split(new[] {"\r\n"}, StringSplitOptions.None).Select(x => x.Trim())) {
                if (line.StartsWith("[")) {
                    // a new section
                    if (last.ok) 
                        create_last_formatter(formatters, ref last, ref errors);

                    if (line.EndsWith("]"))
                        last.column_type = line.Substring(1, line.Length - 2).Trim();
                    else
                        errors += "Invalid line: " + line + "\r\n";
                }
                else if (line.StartsWith("#"))
                    // comment
                    continue;
                else if (line != "") {
                    if (last.name == "")
                        // first line = the name of the formatter
                        last.name = line;
                    // has to be syntax like : a=b
                    else if (line.IndexOf("=") > 0)
                        last.syntax += line + "\r\n";
                    else
                        errors += "Invalid line: " + line + "\r\n";
                } else {
                    // empty line - end of a formatter
                    if (last.ok) 
                        create_last_formatter(formatters, ref last, ref errors);
                }
            }

            if ( last.ok)
                create_last_formatter(formatters, ref last, ref errors);

            formatters_ = formatters;
        }

        private void create_last_formatter(List<formatter> formatters, ref formatter last, ref string errors) {
            string error = "";
            last.the_formatter = create_formatter(last.name, last.syntax, ref error);
            if (error != "")
                errors += error + "\r\n";
            if ( last.the_formatter != null)
                formatters.Add(last);
            var new_last = new formatter { column_type = last.column_type };
            last = new_last;
        }

        private bool needs_apply_formatter(formatter format, column_formatter_base.format_cell cell) {
            if (format.column_type == "all")
                return true;

            var aliases = cell.parent.filter.log.aliases;
            var cell_type = aliases.to_info_type(format.column_type);
            if (cell_type != info_type.max)
                return cell.col_type == cell_type;

            // in this case, we don't know what column the formater is to be applied to
            return false;
        }

        internal void format_before(column_formatter_base.format_cell cell) {
            // each formatter is called once "before" the filters
            // then, it's called again "after" the filters
            //
            // this way, I can modify the text before (when dealing with numbers and such)
            foreach ( var format in formatters_)
                if ( needs_apply_formatter(format, cell))
                    format.the_formatter.format_before_do_replace(cell);

            foreach ( var format in formatters_)
                if ( needs_apply_formatter(format, cell))
                    format.the_formatter.format_before(cell);
            
        }
        internal void format_after(column_formatter_base.format_cell cell) {
            // each formatter is called once "before" the filters
            // then, it's called again "after" the filters
            //
            // this way, I can modify the text before (when dealing with numbers and such)
            foreach ( var format in formatters_)
                if (needs_apply_formatter(format, cell)) {
                    format.the_formatter.format_after(cell);
                    switch (format.the_formatter.align) {
                    case column_formatter_base.align_type.left:
                        cell.format_text.align = HorizontalAlignment.Left;
                        break;
                    case column_formatter_base.align_type.center:
                        cell.format_text.align = HorizontalAlignment.Center;
                        break;
                    case column_formatter_base.align_type.right:
                        cell.format_text.align = HorizontalAlignment.Right;
                        break;
                    case column_formatter_base.align_type.none:
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                    }
                }

            foreach ( var format in formatters_)
                if (needs_apply_formatter(format, cell))
                    if (format.the_formatter.get_image() != null) {
                        cell.format_text.image = format.the_formatter.get_image();
                        break;
                    }

            cell.format_text.update_parts();
        }

        // note: it's possible to create a valid formatter, and have an error. Like, when the syntax is partially right
        //       In that case, I will just use what is valid
        private static column_formatter_base create_formatter(string name, string syntax, ref string error) {
            error = "";
            column_formatter_base result = null;
            switch (name) {
            case "cell":
                result = new cell();
                break;
            case "format":
                result = new column_formatters.format();
                break;
            case "picture":
                result = new picture();
                break;
            case "stack_trace":
            case "stack-trace":
                result = new stack_trace();
                break;
            case "xml":
                result = new xml();
                break;
            default:
                error = "Invalid formatter name: " + name;
                break;
            }
            // load_syntax
            if (result != null)
                result.load_syntax(new settings_as_string(syntax), ref error);
            return result;
        }

        public void toggle_number_base() {
            foreach (var format in formatters_)
                format.the_formatter.toggle_number_base();
        }

        public void toggle_abbvreviation() {
            foreach (var format in formatters_)
                format.the_formatter.toggle_number_base();
            
        }
    }
}
