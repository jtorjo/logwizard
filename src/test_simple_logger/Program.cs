using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using log4net;

namespace test_simple_logger {
    class Program {
        [DllImport("kernel32.dll")]
        static extern void OutputDebugString(string lpOutputString);

        static void Main(string[] args) {
            test_output_string();
        }

        private static void test_xml_logger() {
            const int sleep_ms = 1000;
            log4net.Config.XmlConfigurator.Configure( new FileInfo("test_simple_logger.exe.config"));
            var logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            int idx = 0;
            while (true) {
                logger.Info("another sample msg " + idx);
                ++idx;
                Thread.Sleep(sleep_ms);
            }            
        }

        private static void test_output_string() {
            const int sleep_ms = 1000;
            int idx = 0;
            while (true) {
                OutputDebugString("simple log " + idx);
                Console.WriteLine("simple log " + idx);
                ++idx;
                Thread.Sleep(sleep_ms);
            }
        }
    }
}
