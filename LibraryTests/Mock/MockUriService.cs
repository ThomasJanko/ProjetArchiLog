using System;
using LibraryArchiLog.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace LibraryTests.Mock
{
    public class MockUriService : IUriService
    {
        private readonly string _baseUri;
        public MockUriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPageUri(string range, string route)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "range", range);
            return new Uri(modifiedUri);
        }

        public Uri GetPageUri(string range, string route, string asc, string desc, string type, string rating, string date)
        {
            throw new NotImplementedException();
        }
    }
}
