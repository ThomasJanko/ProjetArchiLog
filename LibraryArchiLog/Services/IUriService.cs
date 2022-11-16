using LibraryArchiLog.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryArchiLog.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(string? range, string? route, string? asc, string? desc, string? type, string? rating, string? date);

        //public interface IOperationTransient : IUriService { }
        //public interface IOperationScoped : IUriService { }
        //public interface IOperationSingleton : IUriService { }
    }

}
