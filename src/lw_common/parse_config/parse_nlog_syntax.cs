using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common
{
    public static class parse_nlog_syntax 
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool is_syntax_string(string syntax) {
            int idx1 = syntax.IndexOf("${");
            int idx2 = idx1 >= 0 ? syntax.IndexOf("${", idx1 + 1) : -1;
            return idx1 >= 0 && idx2 >= 0;
        }

        private static bool ignore_renderer(string name) {
            switch (name) {
            case "": // we can have an empty name when the parsing did not go well

            case "cached":
            case "filesystem-normalize":
            case "json-encode":
            case "onexception":  // this is waaay to complicated for now
            case "replace":
            case "replace-newlines":
            case "rot13":
            case "url-encode":
            case "whenEmpty":
            case "xml-encode":

            case "newline":
                return true;
            }
            return false;
        }

        private static bool need_to_get_inner_renderer(string name) {
            switch (name) {
            case "lowercase": 
            case "trim-whitespace": 
            case "uppercase": 
            case "when": 
                return true;
            }
            return false;
            
        }

        private static string column_to_lw_column(string name) {
            switch (name) {
            case "callsite":
            case "callsite-linenumber":
                return "class";

            case "date":
            case "longdate":
            case "time":
            case "shortdate":
                return "time";

            case "level":
                return "level";

            case "logger":
                return "class{logger}";

            case "message":
                return "msg";

            case "threadid":
            case "threadname":
                return "thread";
            }

            return "";
        }

        // returning an empty string means we could not parse it
        //
        // nlog syntax is case-insensitive
        public static string parse(string syntax) {
            // https://github.com/NLog/NLog/wiki/Pad-Layout-Renderer
            // https://github.com/NLog/NLog/wiki/Layout-renderers
            syntax_to_lw_syntax to_lw = new syntax_to_lw_syntax(syntax, "nlog");
            syntax = syntax.ToLower();
            try {
                var renderers = split_into_renderers(syntax);
                foreach (var renderer_and_suffix in renderers) {
                    int end_idx = renderer_and_suffix.LastIndexOf("}");
                    if (end_idx < 0) {
                        to_lw.on_error("pattern not ending in }");
                        break;
                    }
                    var renderer = renderer_and_suffix.Substring(0, end_idx).Trim();
                    if (renderer.StartsWith("${"))
                        renderer = renderer.Substring(2);
                    var suffix = renderer_and_suffix.Substring(end_idx + 1);
                    var renderer_and_props = parse_renderer(renderer);

                    if (need_to_get_inner_renderer(renderer_and_props.Item1))
                        renderer_and_props = get_inner_renderer(renderer_and_props.Item2);
                    if (ignore_renderer(renderer_and_props.Item1))
                        continue;
                    var inner = renderer_and_props.Item2.FirstOrDefault(x => x.Item1 == "inner");
                    if (inner != null && inner.Item2 == "layout")
                        // applies to full layout, we don't care
                        continue;
                    string renderer_name = renderer_and_props.Item1;
                    if (renderer_name == "newline")
                        continue;
                    // here - we have the renderer and its properties
                    int min_len = -1;
                    bool fixed_now = false;
                    if (renderer_name == "pad") {
                        // care about padding
                        inner = renderer_and_props.Item2.FirstOrDefault(x => x.Item1 == "inner");
                        if (inner == null) {
                            inner = renderer_and_props.Item2.FirstOrDefault(x => x.Item2.StartsWith("${"));
                            if (inner == null) {
                                to_lw.on_error("Invalid padding");
                                break;
                            }
                        }
                        if (inner.Item1 == "layout")
                            // applies to all line - we don't care
                            continue;
                        renderer_name = parse_renderer(inner.Item2).Item1;
                        var padding = renderer_and_props.Item2.FirstOrDefault(x => x.Item1 == "padding");
                        if (padding != null)
                            min_len = Math.Abs(int.Parse(padding.Item2));
                        var fixed_len = renderer_and_props.Item2.FirstOrDefault(x => x.Item1 == "fixedlength");
                        fixed_now = fixed_len != null && fixed_len.Item2 == "true" && min_len > 0;
                    }
                    var lw_column = column_to_lw_column(renderer_name);
                    to_lw.add_column(min_len, fixed_now, renderer_name, suffix, lw_column);
                }
            } catch (Exception e) {
                to_lw.on_error(e.Message);
            }
            return to_lw.lw_syntax;
        }

        private static Tuple<string, List<Tuple<string, string>>> get_inner_renderer(List<Tuple<string, string>> properties) {
            // by default, any property that contains an inner full renderer
            var default_inner = properties.FirstOrDefault(x => x.Item2.StartsWith("${"));
            if (default_inner != null)
                return parse_renderer(default_inner.Item2);
            // now, search for 'inner'
            var inner = properties.FirstOrDefault(x => x.Item1 == "inner");
            if (inner != null)
                return parse_renderer(inner.Item2);
            // there's no inner renderer
            return new Tuple<string, List<Tuple<string, string>>>("", new List<Tuple<string, string>>());
        }

        // string - the renderer name
        // list = pair of name=value from the renderer, if any
        private static Tuple<string, List<Tuple<string, string>>> parse_renderer(string renderer) {
            renderer = renderer.Trim();
            if (renderer.StartsWith("${") && renderer.EndsWith("}"))
                renderer = renderer.Substring(2, renderer.Length - 3);
            int end_of_name = renderer.IndexOf(":");
            var properties = new List<Tuple<string, string>>();
            if ( end_of_name < 0)
                return new Tuple<string, List<Tuple<string, string>>>(renderer, properties);

            string name = renderer.Substring(0, end_of_name);
            renderer = renderer.Substring(end_of_name + 1);
            while (renderer != "") {
                int indent = 0;
                int i = 0;
                for ( ; i < renderer.Length; ++i)
                    if (renderer[i] == '\\')
                        ++i;
                    else if (renderer[i] == '{')
                        ++indent;
                    else if (renderer[i] == '}') 
                        --indent;
                    else if (renderer[i] == ':') {
                        if (indent == 0)
                            break;
                    }
                if (i >= renderer.Length)
                    // last pair
                    break;
                properties.Add( parse_single_property( renderer.Substring(0,i) ));
                renderer = renderer.Substring(i + 1);
            }

            if ( renderer != "")
                properties.Add( parse_single_property(renderer));

            return new Tuple<string, List<Tuple<string, string>>>(name, properties);
        }

        private static Tuple<string, string> parse_single_property(string prop) {
            if ( prop.StartsWith("${"))
                // it's an inner renderer
                return new Tuple<string, string>("", prop);
            int idx = prop.IndexOf("=");
            if ( idx >= 0)
                return new Tuple<string, string>(prop.Substring(0,idx).Trim(), prop.Substring(idx+1).Trim());
            else 
                // no value?
                return new Tuple<string, string>(prop, "");
        }

        // watch for inner renderers, such as ${padding:padding=5,fixedlength=true:${level:uppercase=true}} 
        // (in the above case, $level)
        private static List<string> split_into_renderers(string syntax) {
            List<string> renderers = new List<string>();
            
            while (syntax != "") {
                int indent = 0;
                int i = 0;
                for ( ; i < syntax.Length; ++i)
                    if (syntax[i] == '\\')
                        ++i;
                    else if (syntax[i] == '{')
                        ++indent;
                    else if (syntax[i] == '}') {
                        if (--indent == 0)
                            break;
                    }

                if (i >= syntax.Length)
                    // in this case, the whole string is the last renderer
                    break;

                int next = syntax.IndexOf("${", i);
                if (next < 0)
                    // in this case, the whole string is the last renderer
                    break;

                renderers.Add( syntax.Substring(0,next));
                syntax = syntax.Substring(next);
            }

            if ( syntax != "")
                renderers.Add(syntax);
            return renderers;
        }
    }
}
