using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryArchiLog.Extensions
{
    public static class QueryExtension
    {
        public static IOrderedQueryable<TModel> Sort<TModel>(this IQueryable<TModel> query, Params p)
        {
            if (!string.IsNullOrWhiteSpace(p.Key))
            {
                string champ = p.Key;


                //return query.OrderBy(x => x.name);
            }
            else return (IOrderedQueryable<TModel>)query;
        }

        public static IOrderedQueryable<TModel> Filter<TModel>(this IQueryable<TModel> query, Params p )
        {
            if (!string.IsNullOrWhiteSpace(p.Key))
            {
                string champ = p.Key;


                var parameter = Expression.Parameter(typeof(TModel), "x");
            }
            else return (IOrderedQueryable<TModel>)query;
        }
    }
}
