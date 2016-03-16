using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lw_common.parse_syntax;

namespace test_parse_nlog_log4net_syntax
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert( parse_log4net_syntax.parse("%date{yyyy-MM-dd HH:mm:ss} - %message") == "time[0,' - '] msg[''] ");
            Debug.Assert( parse_log4net_syntax.parse("%-70file(%4line): %date{HH:mm:ss,fff} %-5level - %message%newline") == "file[0,'(';70] ctx1{line}['','): ';4] time['',' '] level['',' - ';5] msg[''] ");
            Debug.Assert( parse_log4net_syntax.parse("%date{HH:mm:ss,fff} %-5level - %message%newline") == "time[0,' '] level['',' - ';5] msg[''] ");
            Debug.Assert( parse_log4net_syntax.parse("%timestamp [%thread] %level %logger %ndc - %message%newline") == "time[0,' ['] thread['','] '] level['',' '] class{Logger}['',' '] ctx1{ndc}['',' - '] msg[''] ");
            Debug.Assert( parse_log4net_syntax.parse("%-6timestamp [%15.15thread] %-5level %30.30logger %ndc - %message%newline") == "time[0,' [';6] thread['',15] level['] ',' ';5] class{Logger}['',30] ctx1{ndc}[' ',' - '] msg[''] ");
            Debug.Assert( parse_log4net_syntax.parse("%date [%thread] %-5level %logger [%property{NDC}] - %message%newline") == "time[0,' ['] thread['','] '] level['',' ';5] class{Logger}['',' ['] ctx1{property}{property}['','] - '] msg[''] ");
        }
    }
}
