using System.Linq.Expressions;
namespace PerspectiveV.Common.Extensions;

/// <summary>
/// Expression utilities for dynamically composing LINQ predicates.
///
/// Usage:
///
/// <code>
/// Expression&lt;Func&lt;Car, bool&gt;&gt; isRed = c =&gt; c.Color == "Red";
/// Expression&lt;Func&lt;Car, bool&gt;&gt; isCheap = c =&gt; c.Price &lt; 10;
///
/// var combined = isRed.And(isCheap);
/// var query = db.Cars.Where(combined);
/// </code>
///
/// Dynamic builder usage:
///
/// <code>
/// var builder = new ExpressionBuilder&lt;Car&gt;();
/// builder.And(c =&gt; c.Color == "Red");
/// builder.And(c =&gt; c.Price &lt; 10);
/// builder.Or(c =&gt; c.Brand == "Toyota");
///
/// var filter = builder.Build();
/// var result = db.Cars.Where(filter);
/// </code>
/// </summary>
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.AndAlso);
    }

    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.OrElse);
    }

    public static Expression<Func<T, bool>> Not<T>(
        this Expression<Func<T, bool>> expression)
    {
        var parameter = expression.Parameters[0];
        var body = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression<T> Compose<T>(
        this Expression<T> first,
        Expression<T> second,
        Func<Expression, Expression, Expression> merge)
    {
        var map = first.Parameters
            .Select((f, i) => new { f, s = second.Parameters[i] })
            .ToDictionary(x => x.s, x => x.f);

        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        return Expression.Lambda<T>(
            merge(first.Body, secondBody),
            first.Parameters);
    }
}

/// <summary>
/// Replaces parameters in expression trees so two lambdas can be merged safely.
/// </summary>
public class ParameterRebinder : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, ParameterExpression> map;

    public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    public static Expression ReplaceParameters(
        Dictionary<ParameterExpression, ParameterExpression> map,
        Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (map.TryGetValue(node, out var replacement))
        {
            return base.VisitParameter(replacement);
        }

        return base.VisitParameter(node);
    }
}

/// <summary>
/// Builds dynamic LINQ expression filters using AND/OR grouping.
///
/// Usage:
///
/// <code>
/// var builder = new ExpressionBuilder&lt;Car&gt;();
///
/// builder.And(c =&gt; c.Color == "Red");
/// builder.And(c =&gt; c.Price &lt; 10);
/// builder.Or(c =&gt; c.Brand == "Toyota");
///
/// var filter = builder.Build();
/// var result = db.Cars.Where(filter);
/// </code>
/// </summary>
public class ExpressionBuilder<T>
{
    private readonly List<Expression<Func<T, bool>>> andCriteria = new();
    private readonly List<Expression<Func<T, bool>>> orCriteria = new();

    public void And(Expression<Func<T, bool>> predicate)
    {
        andCriteria.Add(predicate);
    }

    public void Or(Expression<Func<T, bool>> predicate)
    {
        orCriteria.Add(predicate);
    }

    public Expression<Func<T, bool>> Build()
    {
        Expression<Func<T, bool>>? andExpr = null;
        Expression<Func<T, bool>>? orExpr = null;

        foreach (var expr in andCriteria)
        {
            andExpr = andExpr == null ? expr : andExpr.And(expr);
        }

        foreach (var expr in orCriteria)
        {
            orExpr = orExpr == null ? expr : orExpr.Or(expr);
        }

        if (andExpr != null && orExpr != null)
            return andExpr.And(orExpr);

        return andExpr ?? orExpr ?? (x => true);
    }
}