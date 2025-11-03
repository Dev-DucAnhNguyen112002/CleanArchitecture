using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CleanArchitectureTest.Application.Common.Extentions;

public static class QueryableExtensions
{
    public static ProjectionExpression<TSource> Project<TSource>(this IQueryable<TSource> source)
    {
        return new ProjectionExpression<TSource>(source);
    }
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortBy, bool isDesc)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var fields = sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        IOrderedQueryable<T>? orderedQuery = null;

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            var property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null) continue;

            var param = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.MakeMemberAccess(param, property);
            var orderByExp = Expression.Lambda(propertyAccess, param);

            string methodName;
            if (i == 0)
                methodName = isDesc ? "OrderByDescending" : "OrderBy";
            else
                methodName = isDesc ? "ThenByDescending" : "ThenBy";

            var resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), property.PropertyType },
                (i == 0 ? query : orderedQuery!).Expression, Expression.Quote(orderByExp));

            orderedQuery = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExp);
        }

        return orderedQuery ?? query;
    }
}

public class ProjectionExpression<TSource>
{
    private static readonly Dictionary<string, Expression> ExpressionCache = new Dictionary<string, Expression>();

    private readonly IQueryable<TSource> _source;

    public ProjectionExpression(IQueryable<TSource> source)
    {
        _source = source;
    }

    public IQueryable<TDest> To<TDest>()
    {
        var queryExpression = GetCachedExpression<TDest>() ?? BuildExpression<TDest>();

        return _source.Select(queryExpression);
    }

    private static Expression<Func<TSource, TDest>>? GetCachedExpression<TDest>()
    {
        var key = GetCacheKey<TDest>();

        return ExpressionCache.ContainsKey(key) ? ExpressionCache[key] as Expression<Func<TSource, TDest>> : null;
    }

    private static Expression<Func<TSource, TDest>> BuildExpression<TDest>()
    {
        var sourceProperties = typeof(TSource).GetProperties();
        var destinationProperties = typeof(TDest).GetProperties().Where(dest => dest.CanWrite);
        var parameterExpression = Expression.Parameter(typeof(TSource), "src");

        var bindings = destinationProperties
            .Select(destinationProperty => BuildBinding(parameterExpression, destinationProperty, sourceProperties))
            .Where(binding => binding != null);

        var expression = Expression.Lambda<Func<TSource, TDest>>(Expression.MemberInit(Expression.New(typeof(TDest)), bindings), parameterExpression);

        var key = GetCacheKey<TDest>();

        ExpressionCache.Add(key, expression);

        return expression;
    }

    private static MemberAssignment BuildBinding(Expression parameterExpression, MemberInfo destinationProperty, IEnumerable<PropertyInfo> sourceProperties)
    {
        var sourceProperty = sourceProperties.FirstOrDefault(src => src.Name == destinationProperty.Name);

        if (sourceProperty != null)
        {
            return Expression.Bind(destinationProperty, Expression.Property(parameterExpression, sourceProperty));
        }

        var propertyNames = SplitCamelCase(destinationProperty.Name);

        if (propertyNames.Length == 2)
        {
            sourceProperty = sourceProperties.FirstOrDefault(src => src.Name == propertyNames[0]);

            if (sourceProperty != null)
            {
                var sourceChildProperty = sourceProperty.PropertyType.GetProperties().FirstOrDefault(src => src.Name == propertyNames[1]);

                if (sourceChildProperty != null)
                {
                    return Expression.Bind(destinationProperty, Expression.Property(Expression.Property(parameterExpression, sourceProperty), sourceChildProperty));
                }
            }
        }

        return null!;
    }

    private static string GetCacheKey<TDest>()
    {
        return string.Concat(typeof(TSource).FullName, typeof(TDest).FullName);
    }

    private static string[] SplitCamelCase(string input)
    {
        return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim().Split(' ');
    }
}
