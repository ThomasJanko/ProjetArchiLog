using LibraryArchiLog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
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

        //version 1
        //private static MemberExpression GetMemberExpression<TModel>(ParameterExpression parameter, string propName) where TModel : BaseModel
        //{
        //    if (string.IsNullOrEmpty(propName)) return null;
        //    else
        //    {
        //        return Expression.Property(parameter, propName);
        //    }
        //}

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

        //---------------------------------------------------------------------------filter---------------------------------------------------------------------------------------------------------
        public static IQueryable<TModel> FilterThis<TModel>(this IQueryable<TModel> contents, string type, string rating, string date) where TModel : BaseModel
        {
            Regex typeAndType = new(@"\b\,\b");
            Regex ratingAndRating = new(@"\b\d\,\d\b");
            Regex ratingToRating = new(@"\[\b\d\,\d\b\]");
            Regex ratingToEnd = new(@"\[\d\b\,\]");
            Regex startToRating = new(@"\[\,\d\b\]");
            Regex dateAndDate = new(@"\b\d{0,4}\-\d{0,2}\-\d{0,2}\,\d{0,4}\-\d{0,2}\-\d{0,2}\b");
            Regex dateToDate = new(@"\[\b\d{0,4}\-\d{0,2}\-\d{0,2}\,\d{0,4}\-\d{0,2}\-\d{0,2}\b\]");
            Regex dateToEnd = new(@"\[\b\d{0,4}\-\d{0,2}\-\d{0,2}\b\,\]");
            Regex startToDate = new(@"\[\,\b\d{0,4}\-\d{0,2}\-\d{0,2}\b\]");

            if (!string.IsNullOrEmpty(type))
            {

                var propInfo = typeof(TModel).GetProperty("Type", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var fieldName = propInfo.Name;
                if (typeAndType.IsMatch(type))
                {
                    string[] types = type.Split(',');
                    string one = types[0];
                    string two = types[1];
                    var predicateOne = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, one);
                    var predicateTwo = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, two);
                    var contentsOne = contents.Where(predicateOne);
                    var contentsTwo = contents.Where(predicateTwo);
                    contents = contentsOne.Concat(contentsTwo);
                }
                else
                {
                    var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, type);
                    contents = contents.Where(predicate);
                }
            }

            if (!string.IsNullOrEmpty(rating))
            {
                var propInfo = typeof(TModel).GetProperty("Rating", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var fieldName = propInfo.Name;
                if (ratingToRating.IsMatch(rating))
                {
                    var start = rating[1].ToString();
                    var end = rating[rating.Length - 2].ToString();
                    var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToInt32(start));
                    var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToInt32(end));
                    var contentsStart = contents.Where(predicateStart);
                    var contentsEnd = contents.Where(predicateEnd);
                    contents = contentsStart.Intersect(contentsEnd);
                }
                else if (ratingAndRating.IsMatch(rating))
                {
                    string[] ratings = rating.Split(',');
                    string one = ratings[0];
                    string two = ratings[1];
                    var predicateOne = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToInt32(one));
                    var predicateTwo = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToInt32(two));
                    var contentsOne = contents.Where(predicateOne);
                    var contentsTwo = contents.Where(predicateTwo);
                    contents = contentsOne.Concat(contentsTwo);

                }
                else if (ratingToEnd.IsMatch(rating))
                {
                    var start = rating[1].ToString();
                    var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToInt32(start));
                    contents = contents.Where(predicateStart);
                }
                else if (startToRating.IsMatch(rating))
                {
                    var end = rating[rating.Length - 2].ToString();
                    var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToInt32(end));
                    contents = contents.Where(predicateEnd);
                }
                else
                {
                    var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToInt32(rating));
                    contents = contents.Where(predicate);
                }
            }
            if (!string.IsNullOrEmpty(date))
            {
                var propInfo = typeof(TModel).GetProperty("Date", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var fieldName = propInfo.Name;
                if (dateToDate.IsMatch(date))
                {
                    var start = date.Substring(1, 10);
                    var end = date.Substring(12, 10);
                    string dateTimeStart = start + "T00:00:00";
                    DateTime dtStart = DateTime.ParseExact(dateTimeStart, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    string dateTimeEnd = end + "T00:00:00";
                    DateTime dtEnd = DateTime.ParseExact(dateTimeEnd, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, dtStart);
                    var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, dtEnd);
                    var contentsOne = contents.Where(predicateStart);
                    var contentsTwo = contents.Where(predicateEnd);
                    contents = contentsOne.Intersect(contentsTwo);
                }
                else if (dateAndDate.IsMatch(date))
                {
                    string[] dates = date.Split(',');
                    string one = dates[0].Substring(0, 10);
                    string two = dates[1].Substring(0, 10);
                    string dateTimeOne = one + "T00:00:00";
                    DateTime dtOne = DateTime.ParseExact(dateTimeOne, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    string dateTimeTwo = two + "T00:00:00";
                    DateTime dtTwo = DateTime.ParseExact(dateTimeTwo, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    var predicateOne = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, dtOne);
                    var predicateTwo = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, dtTwo);
                    var contentsOne = contents.Where(predicateOne);
                    var contentsTwo = contents.Where(predicateTwo);
                    contents = contentsOne.Concat(contentsTwo);
                }
                else if (dateToEnd.IsMatch(date))
                {
                    var start = date.Substring(1, 10);
                    string dateTimeStart = start + "T00:00:00";
                    DateTime dtStart = DateTime.ParseExact(dateTimeStart, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, dtStart);
                    contents = contents.Where(predicateStart);
                }
                else if (startToDate.IsMatch(date))
                {
                    var end = date.Substring(2, 10);
                    string dateTimeEnd = end + "T00:00:00";
                    DateTime dtEnd = DateTime.ParseExact(dateTimeEnd, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, dtEnd);
                    contents = contents.Where(predicateEnd);
                }
                else
                {
                    string dateTime = date + "T00:00:00";
                    DateTime dt = DateTime.ParseExact(dateTime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, dt);
                    contents = contents.Where(predicate);
                }
            }
            return contents;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

   
        //Récupérer une expression
        private static MemberExpression GetMemberExpression<TModel>(ParameterExpression parameter, string propName) where TModel : BaseModel
        {
            if (string.IsNullOrEmpty(propName)) return null;
            else
            {
                return Expression.Property(parameter, propName);
            }
        }

    }
}