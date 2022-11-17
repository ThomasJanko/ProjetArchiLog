using LibraryArchiLog.Filter;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryArchiLog.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }
        //public Uri GetPageUri(PaginationFilter filter, string route)
        //{
        //    var _enpointUri = new Uri(string.Concat(_baseUri, route));
        //    var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
        //    modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
        //    return new Uri(modifiedUri);
        //}

        public Uri GetPageUri(string range, string route, string asc, string desc, string type, string rating, string date)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));


            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "range", range);
            if (!string.IsNullOrEmpty(asc))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "asc", asc);
            }
            if (!string.IsNullOrEmpty(desc))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "desc", desc);
            }
            if (!string.IsNullOrEmpty(type))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "type", type);
            }
            if (!string.IsNullOrEmpty(rating))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "rating", rating);
            }
            if (!string.IsNullOrEmpty(date))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "Date", date);
            }


            return new Uri(modifiedUri);
        }
    }
}
