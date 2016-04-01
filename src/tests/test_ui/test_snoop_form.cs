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
    public partial class test_snoop_form : Form
    {
        public test_snoop_form()
        {
            InitializeComponent();
            var snoop_a = new snoop_around_form(this);
            snoop_a.logical_parent_rect = new Rectangle(a.Location, a.Size) ;
            new Thread(() => add_values_thread(snoop_a)) {IsBackground = true}.Start();
        }

        private void add_values_thread(snoop_around_form snoop) {
            Thread.Sleep(3000);
            Dictionary<string,int> v0 = new Dictionary<string, int>() {
                { "one", 10 },
                { "two", 10 },
                { "three", 30 },
            };
            snoop.set_values(v0, false, false);

            Thread.Sleep(5000);
            Dictionary<string,int> v1 = new Dictionary<string, int>() {
                { "one", 5 },
                { "two", 2 },
                { "a", 1 },
                { "b", 2 },
                { "z", 15 },
            };
            snoop.set_values(v1, false, false);

            Thread.Sleep(5000);
            Dictionary<string,int> v2 = new Dictionary<string, int>() {
                { "a", 1 },
                { "b", 2 },
                { "c", 3 },
                { "d", 4 },
                { "z", 15 },
            };
            snoop.set_values(v2, true, true);
        }
    }
}
