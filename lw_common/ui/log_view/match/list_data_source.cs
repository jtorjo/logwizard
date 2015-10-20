using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using LogWizard;

namespace lw_common.ui {
    internal class list_data_source : AbstractVirtualListDataSource {
        private VirtualObjectListView lv_ = null;

        private readonly filter.match_list items_;

        private log_view parent_;

        public list_data_source(VirtualObjectListView lv, log_view parent ) : base(lv) {
            lv_ = lv;
            parent_ = parent;
            items_ = parent.filter.matches ;
        }

        public string name {
            set { items_.name = "list_data " + value; }
        }

        public override int GetObjectIndex(object model) {
            return items_.index_of(model as match_item);
        }

        public override object GetNthObject(int n) {
            return n < items_.count ? items_.match_at(n) : null;
        }

        public override int GetObjectCount() {
            return items_.count;
        }

        public void refresh() {
            if ( items_.count == 0)
                lv_.ClearObjects();

            lv_.UpdateVirtualListSize();                
        }
    }
}
