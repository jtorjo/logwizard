/* 
 * Copyright (C) 2014-2015 John Torjo
 *
 * This file is part of LogWizard
 *
 * LogWizard is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * LogWizard is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact john.code@torjo.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace lw_common {

    public class memory_optimized_list_base {
        public static int log_idx = 0;
        public static object lock_ = new object();
        // at how many reallocations, show we Gc.collect?
        public const int gc_collect_step = 20;
    }

    public class memory_optimized_list<T> : List<T> {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public double increase_percentage = .2;
        private int min_capacity_ = 100;

        private const int PAD = 100;

        // friendly name - useful when dumping change of capacity
        public string name = "";

        private bool first_resize_ = true;

        public memory_optimized_list(int capacity) : base(capacity) {
            name = ToString();
        }

        public memory_optimized_list() : base() {
            name = ToString();            
        }

        public int min_capacity {
            get { return min_capacity_; }
            set {
                if (value <= 0)
                    return;
                min_capacity_ = value;
                if (Capacity < min_capacity_) {
                    Capacity = min_capacity_;
                    log_capacity();
                }
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

        public void prepare_add(int count) {
            ensure( Count + count);
        }

        private void ensure(int count) {
            count = Math.Max(count, min_capacity_);
            // ... for very small sizes, directly pad them
            int pad = count < PAD ? PAD : 0;
            if (count + pad > Capacity) {
                Capacity = pad + Math.Max( (int) (Capacity * (1 + increase_percentage)), count);
                log_capacity();
            }
        }

        private void log_capacity() {
            if (first_resize_) {
                first_resize_ = false;
                return;
            }

            int idx;
            lock (memory_optimized_list_base.lock_) idx = ++memory_optimized_list_base.log_idx;
            logger.Debug("[memory] (" + idx +  ") new capacity [" + name +"] - " + Capacity);
            if (idx % memory_optimized_list_base.gc_collect_step == 0) {
                logger.Debug("[memory] GC.collect - from memory_optimized_list" );
                GC.Collect();
            }
        }
    }
}
