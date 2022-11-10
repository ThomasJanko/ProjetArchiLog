using LibraryArchiLog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace LibraryArchiLog.Extensions
{
    public static class QueryExtension
    {

        public enum OperationExpression
        {
            Equals,
            NotEquals,
            Minor,
            MinorEquals,
            Mayor,
            MayorEquals,
            Like,
            Contains,
            Any
        }

        public static Expression<Func<TModel, bool>> GetCriteriaWhere<TModel>(string fieldName, OperationExpression selectedOperator, object fieldValue) where TModel : BaseModel
        {

            var propInfo = typeof(TModel).GetProperty(fieldName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            var parameter = Expression.Parameter(typeof(TModel), "x");
            var expressionParameter = GetMemberExpression<TModel>(parameter, fieldName);
            if (propInfo != null && fieldValue != null)
            {

                BinaryExpression body = null;

                switch (selectedOperator)
                {
                    case OperationExpression.Equals:
                        body = Expression.Equal(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.NotEquals:
                        body = Expression.NotEqual(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.Minor:
                        body = Expression.LessThan(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.MinorEquals:
                        body = Expression.LessThanOrEqual(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.Mayor:
                        body = Expression.GreaterThan(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.MayorEquals:
                        body = Expression.GreaterThanOrEqual(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.Like:
                        MethodInfo contains = typeof(string).GetMethod("Contains");
                        var bodyLike = Expression.Call(expressionParameter, contains, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(bodyLike, parameter);
                    case OperationExpression.Contains:
                        return Contains<TModel>(fieldName, fieldValue, parameter, expressionParameter);
                    default:
                        throw new Exception("Not implement Operation");
                }
            }
            else
            {
                Expression<Func<TModel, bool>> filter = x => true;
                return filter;
            }
        }

        private static MemberExpression GetMemberExpression<TModel>(ParameterExpression parameter, string propName) where TModel : BaseModel
        {
            if (string.IsNullOrEmpty(propName)) return null;
            else
            {
                return Expression.Property(parameter, propName);
            }
        }

        public static Expression<Func<TModel, object>> GetExpression<TModel>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TModel), "x");
            Expression conversion = Expression.Convert(Expression.Property(param, propertyName), typeof(object));
            return Expression.Lambda<Func<TModel, object>>(conversion, param);
        }

        private static Expression<Func<TModel, bool>> Contains<TModel>(string fieldName, object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression) where TModel : BaseModel
        {
            var propertyExp = Expression.Property(parameterExpression, fieldName);
            if (propertyExp.Type == typeof(string))
            {
                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var someValue = Expression.Constant(fieldValue, typeof(string));
                var containsMethodExp = Expression.Call(propertyExp, method, someValue);
                return Expression.Lambda<Func<TModel, bool>>(containsMethodExp, parameterExpression);
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(propertyExp.Type);
                var result = converter.ConvertFrom(fieldValue);
                var someValue = Expression.Constant(result);
                var containsMethodExp = Expression.Equal(propertyExp, someValue);
                return Expression.Lambda<Func<TModel, bool>>(containsMethodExp, parameterExpression);
            }
        }

        public static IOrderedQueryable<TModel> Sort<TModel>(this IQueryable<TModel> query, SortParams p)
        {

            if ((!string.IsNullOrWhiteSpace(p.Asc)) && (!string.IsNullOrWhiteSpace(p.Desc)))
            {
                string champ_asc = p.Asc;
                string champ_desc = p.Desc;

                //créer lambda
                var parameter_asc = Expression.Parameter(typeof(TModel), "x");
                var property_asc = Expression.Property(parameter_asc, champ_asc);
                var parameter_desc = Expression.Parameter(typeof(TModel), "x");
                var property_desc = Expression.Property(parameter_desc, champ_desc);

                var o_asc = Expression.Convert(property_asc, typeof(object));
                var lambda_asc = Expression.Lambda<Func<TModel, object>>(o_asc, parameter_asc);
                var o_desc = Expression.Convert(property_desc, typeof(object));
                var lambda_desc = Expression.Lambda<Func<TModel, object>>(o_desc, parameter_desc);

                //utilisation lambda
                return query.OrderBy(lambda_asc).ThenByDescending(lambda_desc);

            }

            else if (!string.IsNullOrWhiteSpace(p.Asc))
            {
                string champ = p.Asc;
         
                var parameter = Expression.Parameter(typeof(TModel), "x");
                var property = Expression.Property(parameter, champ);

                var o = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<TModel, object>>(o, parameter);

                return query.OrderBy(lambda);
            }

            else if (!string.IsNullOrWhiteSpace(p.Desc))
            {
                string champ = p.Desc;

                var parameter = Expression.Parameter(typeof(TModel), "x");
                var property = Expression.Property(parameter, champ);

                var o = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<TModel, object>>(o, parameter);

                return query.OrderByDescending(lambda);
            }

            else
            {
                return (IOrderedQueryable<TModel>)query;
            }
        }

        public static IQueryable<TModel> SearchThis<TModel>(this IQueryable<TModel> contents, string name, string category) where TModel : BaseModel
        {
            if (!string.IsNullOrEmpty(name))
            {
                var propInfo = typeof(TModel).GetProperty("Name", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var predicate = GetCriteriaWhere<TModel>(propInfo.Name, OperationExpression.Contains, name);
                contents = contents.Where(predicate);
            }
            if (!string.IsNullOrEmpty(category))
            {
                var propInfo = typeof(TModel).GetProperty("Category", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var predicate = GetCriteriaWhere<TModel>(propInfo.Name, OperationExpression.Contains, category);
                contents = contents.Where(predicate);
            }
            return contents;
        }
    }
}