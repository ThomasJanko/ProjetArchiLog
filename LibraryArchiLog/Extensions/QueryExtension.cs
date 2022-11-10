using LibraryArchiLog.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace LibraryArchiLog.Extensions
{
    public static class QueryExtension
    {
        public static IOrderedQueryable<TModel> Sort<TModel>(this IQueryable<TModel> query, SortParams p)
        {

            if ((!string.IsNullOrWhiteSpace(p.Asc)) && (!string.IsNullOrWhiteSpace(p.Desc)))
            {
                string champ_asc = p.Asc;
                string champ_desc = p.Desc;

                var parameter_asc = Expression.Parameter(typeof(TModel), "x");
                var property_asc = Expression.Property(parameter_asc, champ_asc);
                var parameter_desc = Expression.Parameter(typeof(TModel), "x");
                var property_desc = Expression.Property(parameter_desc, champ_desc/*"Name"*/);

                var o_asc = Expression.Convert(property_asc, typeof(object));
                var lambda_asc = Expression.Lambda<Func<TModel, object>>(o_asc, parameter_asc);
                var o_desc = Expression.Convert(property_desc, typeof(object));
                var lambda_desc = Expression.Lambda<Func<TModel, object>>(o_desc, parameter_desc);

                return query.OrderBy(lambda_asc).ThenByDescending(lambda_desc);

            }

            else if (!string.IsNullOrWhiteSpace(p.Asc))
            {
                string champ = p.Asc;

                //créer lambda
                var parameter = Expression.Parameter(typeof(TModel), "x");
                var property = Expression.Property(parameter, champ/*"Name"*/);

                var o = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<TModel, object>>(o, parameter);

                //utilisation lambda
                return query.OrderBy(lambda);
                //return query.OrderBy(x => x.Name);
            }

            else if (!string.IsNullOrWhiteSpace(p.Desc))
            {
                string champ = p.Desc;

                //créer lambda
                var parameter = Expression.Parameter(typeof(TModel), "x");
                var property = Expression.Property(parameter, champ/*"Name"*/);

                var o = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<TModel, object>>(o, parameter);

                //utilisation lambda
                return query.OrderByDescending(lambda);
                //return query.OrderBy(x => x.Name);
            }

            else
            {
                return (IOrderedQueryable<TModel>)query;
            }
        }
    }
}
