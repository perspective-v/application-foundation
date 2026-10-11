using PerspectiveV.Common.Paging;
using System.Linq.Expressions;

namespace PerspectiveV.Common.Extensions;
/// <summary>
/// Provides extension methods for Query Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by QueryExtensions.
/// var typeName = nameof(QueryExtensions);
/// ]]></code>
/// </example>
public static class QueryExtensions {
    /// <summary>
    /// Executes Apply Pagination.
    /// </summary>
    /// <param name="query">The query value.</param>
    /// <param name="options">The options value.</param>
    /// <returns>The operation result.</returns>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PagingOptions options) {
        var offset = options?.Offset.GetValueOrDefault() ?? 0;
        var limit = options?.Limit.GetValueOrDefault() ?? int.MaxValue;
        query = query
            .Skip(offset)
            .Take(limit);
        return query;
    }

    /// <summary>
    /// Executes Query Fields.
    /// </summary>
    /// <param name="fields">The fields value.</param>
    /// <returns>The operation result.</returns>
    public static Func<T, T> QueryFields<T>(string fields = "") {
        string[] EntityFields;
        if (fields == "")
            // get Properties of the TModel
            EntityFields = typeof(T).GetProperties().Select(propertyInfo => propertyInfo.Name).ToArray();
        else
            EntityFields = fields.Split(',');

        // input parameter "o"
        var xParameter = Expression.Parameter(typeof(T), "o");

        // new statement "new Data()"
        var xNew = Expression.New(typeof(T));

        // create initializers
        var bindings = EntityFields.Select(o => o.Trim())
            .Select(o => {

                // property "Field1"
                var mi = typeof(T).GetProperty(o);

                // original value "o.Field1"
                var xOriginal = Expression.Property(xParameter, mi!);

                // set value "Field1 = o.Field1"
                return Expression.Bind(mi!, xOriginal);
            }
        );

        // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        var xInit = Expression.MemberInit(xNew, bindings);

        // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
        var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

        // compile to Func<Data, Data>
        return lambda.Compile();
    }

    /// <summary>
    /// Executes Select Fields.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="fields">The fields value.</param>
    /// <returns>The operation result.</returns>
    public static IQueryable<TResult> SelectFields<TResult>(this IQueryable source, string[] fields) {
        var sourceType = source.ElementType;
        var resultType = typeof(TResult);
        var parameter = Expression.Parameter(sourceType, "e");

        //var bindings = fields.Select(column => Expression.Bind(
        //    resultType.GetProperty(column), Expression.PropertyOrField(parameter, column)));

        var bindings = fields
            .Select(column => {
                var property = resultType.GetProperty(column);
                return property != null
                    ? Expression.Bind(property, Expression.PropertyOrField(parameter, column))
                    : null;
            })
            .Where(binding => binding is not null)
            .Cast<MemberBinding>()
            .ToList();


        var body = Expression.MemberInit(Expression.New(resultType), bindings);
        var selector = Expression.Lambda(body, parameter);
        return source.Provider.CreateQuery<TResult>(
            Expression.Call(typeof(Queryable), "Select", [sourceType, resultType], source.Expression, Expression.Quote(selector)));
    }
}
