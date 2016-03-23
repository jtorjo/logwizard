using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common
{
    internal static class parse_syntax
    {
        // parses syntax if it's in a different dialect (nlog / log4net)
        public static string parse(string syntax) {
            var converted = "";
            if (parse_nlog_syntax.is_syntax_string(syntax)) 
                converted = parse_nlog_syntax.parse(syntax);
            if (converted == "" && parse_log4net_syntax.is_syntax_string(syntax))
                converted = parse_log4net_syntax.parse(syntax);            
            return converted != "" ? converted : syntax;
        }
    }
}
