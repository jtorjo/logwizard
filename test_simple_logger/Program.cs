using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;

namespace test_simple_logger {
    class Program {
        static void Main(string[] args) {
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
    }
}
