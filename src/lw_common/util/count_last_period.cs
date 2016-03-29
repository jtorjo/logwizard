using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lw_common
{
    /* computes how many additions were in the last period 
     * you can count things like "how many errors in the last 40 seconds" and such
     * note that resetting the period_ms will only take effect from that point on
     */
    public class count_last_period
    {
        private List<DateTime> last = new List<DateTime>();

        public int period_ms = 1000;

        public count_last_period(int period_ms = 1000) {
            this.period_ms = period_ms;
        }

        public void add_now() {
            last.Insert(0, DateTime.Now);
        }

        // earliest still recorded "event"
        public DateTime earliest { get {
            if ( last.Count > 0)
                return last[last.Count - 1];
            else
                return DateTime.Now;
        }}

        public int count() {
            if (last.Count < 1)
                return 0; // optimization

            DateTime least = DateTime.Now.AddMilliseconds( -period_ms);
            if ( last[last.Count - 1] >= least)
                // optimization for when all items are in this period
                return last.Count;

            int result = 0;
            foreach ( DateTime dt in last)
                if ( dt >= least)
                    ++result;
                else
                    break;

            last.RemoveRange( result, last.Count - result);
            return result;
        }
    }
}
