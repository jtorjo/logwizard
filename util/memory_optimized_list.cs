using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogWizard {
    class memory_optimized_list<T> : List<T> {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public double increase_percentage = .2;
        private int min_capacity_ = 100;

        private const int PAD = 100;

        // friendly name - useful when dumping change of capacity
        public string name = "";

        public memory_optimized_list(int capacity) : base(capacity) {
            ensure(min_capacity_);
        }

        public memory_optimized_list() : base() {
            
        }

        public int min_capacity {
            get { return min_capacity_; }
            set {
                if (value <= 0)
                    return;
                min_capacity_ = value;
                if (Capacity < min_capacity_)
                    Capacity = min_capacity_;
            }
        }

        public new void Add(T element) {
            ensure( Count + 1);
            base.Add(element);
        }

        public new void AddRange(IEnumerable<T> range) {
            ensure(Count + range.Count());
            base.AddRange(range);
        }

        private void ensure(int count) {
            if (count + PAD > Capacity) {
                Capacity = PAD + (int) (Capacity * (1 + increase_percentage));
                logger.Debug("[memory] new capacity [" + name +"] - " + Capacity);
                GC.Collect();
            }
        }
    }
}
