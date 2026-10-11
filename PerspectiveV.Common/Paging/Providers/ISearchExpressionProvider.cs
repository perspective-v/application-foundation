using System.Linq.Expressions;

namespace PerspectiveV.Common.Paging.Providers;
public interface ISearchExpressionProvider {

    ConstantExpression GetValue(string input);
    Expression GetComparison(
       MemberExpression left,
       string op,
       ConstantExpression right);
}
