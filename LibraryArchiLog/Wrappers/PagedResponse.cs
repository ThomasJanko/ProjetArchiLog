using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryArchiLog.Wrappers
{
    

    public class PagedResponse<T> : Response<T>
    {
        public String Range { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri First { get; set; }
        public Uri Next { get; set; }
        public Uri Prev { get; set; }
        public Uri Last { get; set; }
        public PagedResponse(T data, string range, int page, int pageSize)
        {
            this.Range = range;
            this.Page = page;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
