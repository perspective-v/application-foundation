using System.Linq.Expressions;

namespace PerspectiveV.Common.Paging.Providers;
/// <summary>
/// Represents the DecimalToIntSearchExpressionProvider type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of DecimalToIntSearchExpressionProvider.
/// var type = typeof(DecimalToIntSearchExpressionProvider);
/// ]]></code>
/// </example>
public class DecimalToIntSearchExpressionProvider : DefaultSearchExpressionProvider {
    /// <summary>
    /// Gets Get Value.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The operation result.</returns>
    public override ConstantExpression GetValue(string input) {
        if (!decimal.TryParse(input, out var dec))
            throw new ArgumentException("Individual search value");

        var places = BitConverter.GetBytes(decimal.GetBits(dec)[3])[2];
        if (places < 2) places = 2;
        var justDigits = (int)(dec * (decimal)Math.Pow(10, places));

        return Expression.Constant(justDigits);
    }

    /// <summary>
    /// Gets Get Comparison.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="op">The op value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>The operation result.</returns>
    public override Expression GetComparison(
        MemberExpression left, string op, ConstantExpression right) {
        switch (op.ToLower()) {
            case "gt": return Expression.GreaterThan(left, right);
            case "gte": return Expression.GreaterThanOrEqual(left, right);
            case "lt": return Expression.LessThan(left, right);
            case "lte": return Expression.LessThanOrEqual(left, right);
            // If nothing matches, fall back to base implementation
            default: return base.GetComparison(left, op, right);
        }
    }
}
