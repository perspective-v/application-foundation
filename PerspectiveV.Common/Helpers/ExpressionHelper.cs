using System.Linq.Expressions;
using System.Reflection;

namespace PerspectiveV.Common.Helpers;
/// <summary>
/// Provides static members in Expression Helper.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by ExpressionHelper.
/// var typeName = nameof(ExpressionHelper);
/// ]]></code>
/// </example>
public static class ExpressionHelper {
    private static readonly MethodInfo LambdaMethod = typeof(Expression)
        .GetMethods()
        .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2);

    private static MethodInfo[] QueryableMethods = typeof(Queryable)
        .GetMethods()
        .ToArray();

    /// <summary>
    /// Gets Get Lambda Func Builder.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="dest">The dest value.</param>
    /// <returns>The operation result.</returns>
    private static MethodInfo GetLambdaFuncBuilder(Type source, Type dest) {
        var predicateType = typeof(Func<,>).MakeGenericType(source, dest);
        return LambdaMethod.MakeGenericMethod(predicateType);
    }

    /// <summary>
    /// Gets Get Property Info.
    /// </summary>
    /// <param name="name">The name value.</param>
    /// <returns>The operation result.</returns>
    public static PropertyInfo GetPropertyInfo<T>(string name)
        => typeof(T).GetProperties()
        .Single(p => p.Name == name);

    /// <summary>
    /// Executes Parameter.
    /// </summary>
    /// <returns>The operation result.</returns>
    public static ParameterExpression Parameter<T>()
        => Expression.Parameter(typeof(T));

    /// <summary>
    /// Gets Get Property Expression.
    /// </summary>
    /// <param name="obj">The obj value.</param>
    /// <param name="property">The property value.</param>
    /// <returns>The operation result.</returns>
    public static MemberExpression GetPropertyExpression(ParameterExpression obj, PropertyInfo property)
        => Expression.Property(obj, property);

    /// <summary>
    /// Gets Get Lambda.
    /// </summary>
    /// <param name="obj">The obj value.</param>
    /// <param name="arg">The arg value.</param>
    /// <returns>The operation result.</returns>
    public static LambdaExpression GetLambda<TSource, TDest>(ParameterExpression obj, Expression arg)
        => GetLambda(typeof(TSource), typeof(TDest), obj, arg);

    /// <summary>
    /// Gets Get Lambda.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="dest">The dest value.</param>
    /// <param name="obj">The obj value.</param>
    /// <param name="arg">The arg value.</param>
    /// <returns>The operation result.</returns>
    public static LambdaExpression GetLambda(Type source, Type dest, ParameterExpression obj, Expression arg) {
        var lambdaBuilder = GetLambdaFuncBuilder(source, dest);
        var result = lambdaBuilder.Invoke(null, new object[] { arg, new[] { obj } }) as LambdaExpression;
        return result ?? throw new InvalidOperationException("lambdaBuilder.Invoke returned null.");
    }

    /// <summary>
    /// Executes Call Where.
    /// </summary>
    /// <param name="query">The query value.</param>
    /// <param name="predicate">The predicate value.</param>
    /// <returns>The operation result.</returns>
    public static IQueryable<T> CallWhere<T>(IQueryable<T> query, LambdaExpression predicate) {
        var whereMethodBuilder = QueryableMethods
            .First(x => x.Name == "Where" && x.GetParameters().Length == 2)
            .MakeGenericMethod(new[] { typeof(T) });

        var result = whereMethodBuilder?.Invoke(null, new object[] { query, predicate }) as IQueryable<T>;
        return result ?? throw new InvalidOperationException("whereMethodBuilder.Invoke returned null.");
    }

    /// <summary>
    /// Executes Call Order By Or Then By.
    /// </summary>
    /// <param name="modifiedQuery">The modifiedQuery value.</param>
    /// <param name="useThenBy">The useThenBy value.</param>
    /// <param name="descending">The descending value.</param>
    /// <param name="propertyType">The propertyType value.</param>
    /// <param name="keySelector">The keySelector value.</param>
    /// <returns>The operation result.</returns>
    public static IQueryable<TEntity> CallOrderByOrThenBy<TEntity>(
        IQueryable<TEntity> modifiedQuery,
        bool useThenBy,
        bool descending,
        Type propertyType,
        LambdaExpression keySelector) {
        var methodName = "OrderBy";
        if (useThenBy) methodName = "ThenBy";
        if (descending) methodName += "Descending";

        var method = QueryableMethods
            .First(x => x.Name == methodName && x.GetParameters().Length == 2)
            .MakeGenericMethod(new[] { typeof(TEntity), propertyType });

        var result = method.Invoke(null, new object[] { modifiedQuery, keySelector }) as IQueryable<TEntity>;
        return result ?? throw new InvalidOperationException("Failed to invoke method, resulting in null.");
    }
}
