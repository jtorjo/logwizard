using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using lw_common;

namespace test_ui {
    public partial class test_olv : Form {
        public class item {
            public string idx = "";
            public string date = "";
            public string time = "";
            public string hidden = "h";
            public string msg = "";
        }

        private List<OLVColumn> old_cols_, new_cols_; 

        public test_olv() {
            InitializeComponent();

            list.AddObject( new item { idx = "0", date = "today", time = "00:00:01", msg = "first line"});
            list.AddObject( new item { idx = "1", date = "today", time = "00:01:01", msg = "second line"});
            list.AddObject( new item { idx = "2", date = "today", time = "00:10:01", msg = "third line"});
            list.AddObject( new item { idx = "3", date = "today", time = "02:00:01", msg = "fourth line"});
            list.AddObject( new item { idx = "4", date = "today", time = "03:00:01", msg = "fifth line"});
            list.AddObject( new item { idx = "5", date = "today", time = "04:00:01", msg = "sixth line"});

            dateCol.IsVisible = false;
            hiddenCol.IsVisible = false;
            list.RebuildColumns();

            old_cols_ = list.AllColumns.ToList();

            msgCol.DisplayIndex = 1;

            util.postpone(test, 100);

            list.ColumnRightClick += list_ColumnRightClick;
            // SelectColumnsOnRightClick !!!!!!!!! ShowCommandMenuOnRightClick 
        }

        void list_ColumnRightClick(object sender, ColumnClickEventArgs e) {
            Console.WriteLine("click on " + e.Column);
        }

        private void test() {
            new_cols_ = list.AllColumns.ToList();
            Debug.Assert(Enumerable.SequenceEqual(new_cols_, old_cols_));
        }
    }
}
