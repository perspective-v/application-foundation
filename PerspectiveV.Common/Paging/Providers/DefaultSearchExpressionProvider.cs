using System.Linq.Expressions;

namespace PerspectiveV.Common.Paging.Providers;
/// <summary>
/// Represents the DefaultSearchExpressionProvider type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of DefaultSearchExpressionProvider.
/// var type = typeof(DefaultSearchExpressionProvider);
/// ]]></code>
/// </example>
public class DefaultSearchExpressionProvider : ISearchExpressionProvider {
    /// <summary>
    /// Gets Get Comparison.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="op">The op value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>The operation result.</returns>
    public virtual Expression GetComparison(MemberExpression left, string op, ConstantExpression right) {
        if (!op.Equals("eq", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid operator '{op}'.");

        return Expression.Equal(left, right);
    }

    /// <summary>
    /// Gets Get Value.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The operation result.</returns>
    public virtual ConstantExpression GetValue(string input) {
        return Expression.Constant(input);
    }
}
