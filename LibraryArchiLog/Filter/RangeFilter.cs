using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryArchiLog.Filter
{
    public class RangeFilter
    {
        public int Start { get; set; }
        public int End { get; set; }

        public RangeFilter(int start, int end, int limit)
        {
            this.End = start == 0 ? end + 1 : end;
            this.Start = start == 0 ? start + 1 : start;
            this.End = end > limit ? limit : end;

        }
    }
}
