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
        private static void assert(string a, string b) {
            if (a != b) {
                Console.Out.WriteLine("Assertion failed:");
                Console.Out.WriteLine("[" + a + "]");
                Console.Out.WriteLine("[" + b + "]");
                Debug.Assert(false);
            }
        }

        private static void test_log4net(string a, string b) {
            assert( parse_log4net_syntax.parse(a), b);
        }
        private static void test_nlog(string a, string b) {
            assert( parse_nlog_syntax.parse(a), b);
//            Console.Out.WriteLine( parse_nlog_syntax.parse( a));
        }

        static void Main(string[] args)
        {
            test_nlog(  "${longdate} [${threadid}] ${machinename} ${joblayout} ${uppercase:inner=${level}} ${logger} - ${message} ${onexception:${newline}${exception:format=ToString:innerFormat=ToString:maxInnerExceptionLevel=2}}", 
                        "time[0,' ['] thread['','] '] ctx1{machinename}['',' '] ctx2{joblayout}['',' '] level['',' '] class{logger}['',' - '] msg['']");
            test_nlog(  "${date:format=dd.MM.yyyy HH\\:mm\\:ss,fff} | ${level:uppercase=true} | ${message}", 
                        "time[0,' | '] level['',' | '] msg['']");
            test_nlog(  "${date:format=dd.MM.yyyy HH\\:mm\\:ss,fff} | ${pad:padding=5:fixedlength=true:${level:uppercase=true}} | ${message}", 
                        "time[0,' | '] level['',5] msg[' | ']");
            test_nlog(  "${longdate} [${threadid}] ${machinename} ${joblayout} ${uppercase:inner=${level}} ${logger} - ${message} ", 
                        "time[0,' ['] thread['','] '] ctx1{machinename}['',' '] ctx2{joblayout}['',' '] level['',' '] class{logger}['',' - '] msg['']");

            test_log4net(   "%date{yyyy-MM-dd HH:mm:ss} - %message", 
                            "time[0,' - '] msg['']");
            test_log4net(   "%-70file(%4line): %date{HH:mm:ss,fff} %-5level - %message%newline", 
                            "file[0,'(';70] ctx1{line}['','): ';4] time['',' '] level['',' - ';5] msg['']");
            test_log4net(   "%date{HH:mm:ss,fff} %-5level - %message%newline", 
                            "time[0,' '] level['',' - ';5] msg['']");
            test_log4net(   "%timestamp [%thread] %level %logger %ndc - %message%newline", 
                            "time[0,' ['] thread['','] '] level['',' '] class{Logger}['',' '] ctx1{ndc}['',' - '] msg['']");
            test_log4net(   "%-6timestamp [%15.15thread] %-5level %30.30logger %ndc - %message%newline", 
                            "time[0,' [';6] thread['',15] level['] ',' ';5] class{Logger}['',30] ctx1{ndc}[' ',' - '] msg['']");
            test_log4net(   "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline", 
                            "time[0,' ['] thread['','] '] level['',' ';5] class{Logger}['',' ['] ctx1{property}{property}['','] - '] msg['']");
        }
    }
}
