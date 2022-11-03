﻿using LibraryArchiLog.Models;
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
        public static IOrderedQueryable<TModel> Sort<TModel>(this IQueryable<TModel> query, SortParams p)
        {
            if (!string.IsNullOrWhiteSpace(p.Asc))
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

        //public static IOrderedQueryable<TModel> Filter<TModel>(this IQueryable<TModel> query, Params p )
        //{
        //    if (!string.IsNullOrWhiteSpace(p.Name))
        //    {
        //        string champ = p.Name;
                
        //        var parameter = Expression.Parameter(typeof(TModel), "x");
        //    }
        //    else return (IOrderedQueryable<TModel>)query;
        //}
    }
}
