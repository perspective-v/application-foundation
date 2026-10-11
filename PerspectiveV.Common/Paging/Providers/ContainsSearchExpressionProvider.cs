using System.Linq.Expressions;

namespace PerspectiveV.Common.Paging.Providers;

/// <summary>
/// Represents the ContainsSearchExpressionProvider type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ContainsSearchExpressionProvider.
/// var type = typeof(ContainsSearchExpressionProvider);
/// ]]></code>
/// </example>
public class ContainsSearchExpressionProvider : ISearchExpressionProvider
{
    /// <summary>
    /// Gets Get Comparison.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="op">The op value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>The operation result.</returns>
    public virtual Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
    {
        if (!op.Equals("contains", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid operator '{op}'.");

        // Guard: ensure we're only dealing with strings
        if (left.Type != typeof(string))
            throw new InvalidOperationException("Contains operator can only be applied to string properties.");

        // Convert both sides to lowercase for case-insensitive search
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        var leftToLower = Expression.Call(left, toLowerMethod);
        var rightToLower = Expression.Call(right, toLowerMethod);

        return Expression.Call(leftToLower, containsMethod, rightToLower);
    }

    /// <summary>
    /// Gets Get Value.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The operation result.</returns>
    public virtual ConstantExpression GetValue(string input)
    {
        return Expression.Constant(input ?? string.Empty);
    }
}
