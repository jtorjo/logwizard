using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common.parse_syntax
{
    public static class parse_nlog_syntax
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool is_nlog_syntax(string syntax) {
            int idx1 = syntax.IndexOf("${");
            int idx2 = idx1 >= 0 ? syntax.IndexOf("${", idx1 + 1) : -1;
            return idx1 >= 0 && idx2 >= 0;
        }

        // returning an empty string means we could not parse it
        public static string parse(string syntax) {
            // https://github.com/NLog/NLog/wiki/Pad-Layout-Renderer
            return "";
        }
    }
}
