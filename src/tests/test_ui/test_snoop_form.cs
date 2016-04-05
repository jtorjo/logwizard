using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using lw_common.ui;

namespace test_ui
{
    public partial class test_snoop_form : Form {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int add_snoop_idx = 0;
        Dictionary<string,int> v0 = new Dictionary<string, int>() {
            { "one", 10 },
            { "two", 10 },
            { "three", 30 },
        };
        Dictionary<string,int> v1 = new Dictionary<string, int>() {
            { "one", 5 },
            { "two", 2 },
            { "a", 1 },
            { "b", 2 },
            { "z", 15 },
        };
        Dictionary<string,int> v2 = new Dictionary<string, int>() {
            { "a", 1 },
            { "b", 2 },
            { "c", 3 },
            { "d", 4 },
            { "z", 15 },
        };

        public test_snoop_form()
        {
            InitializeComponent();
            var snoop_a = new snoop_around_form();

            snoop_a.set_parent_rect(this, new Rectangle(a.Location, a.Size) );
            snoop_a.on_snoop = add_values;
        }

        private void add_values(snoop_around_form snoop, ref bool stop) {
            logger.Debug("entered test_snoop_form.add_values");
            while (!stop) {
                Thread.Sleep(3000);
                if (stop)
                    break;
                Dictionary<string, int> v;
                switch (add_snoop_idx++) {
                case 0:
                    v = v0;
                    break;
                case 1:
                    v = v1;
                    break;
                default:
                    v = v2;
                    break;
                }
                bool all_done = add_snoop_idx >= 3;
                snoop.set_values(v, all_done, all_done);
                if (all_done)
                    break;
            }
            logger.Debug("exited test_snoop_form.add_values");
        }
    }
}
