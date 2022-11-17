﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryArchiLog.Filter
{

    public class PaginationFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PaginationFilter()
        {
            this.Page = 1;
            this.PageSize = 7;
        }
        public PaginationFilter(int page, int pageSize)
        {
            this.Page = page < 1 ? 1 : page;
            this.PageSize = pageSize > 50 ? 50 : pageSize;
        }
    }
}
