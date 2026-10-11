using PerspectiveV.Common.Helpers;
using PerspectiveV.Common.Paging.Attributes;
using PerspectiveV.Common.Paging.Providers;
using System.Reflection;

namespace PerspectiveV.Common.Paging.Processors;
/// <summary>
/// Represents the SearchOptionsProcessor type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SearchOptionsProcessor.
/// var type = typeof(SearchOptionsProcessor<,>);
/// ]]></code>
/// </example>
public class SearchOptionsProcessor<TEntity, TModel> {
    public readonly string[] _searchQuery;
    /// <summary>
    /// Initializes a new instance of the SearchOptionsProcessor class.
    /// </summary>
    /// <param name="searchQuery">The searchQuery value.</param>
    public SearchOptionsProcessor(string[] searchQuery) {
        _searchQuery = searchQuery;
    }
    /// <summary>
    /// Gets Get All Terms.
    /// </summary>
    /// <returns>The operation result.</returns>
    public IEnumerable<SearchTerm> GetAllTerms() {
        if (_searchQuery == null) yield break;
        foreach (var expression in _searchQuery) {
            if (string.IsNullOrEmpty(expression)) continue;

            // eaach expression looks like:
            // "fieldname op value"
            var tokens = expression.Split(' ');

            if (tokens.Length == 0) {
                yield return new SearchTerm {
                    ValidSyntax = false,
                    Name = expression
                };

                continue;
            }

            // tokens are less than 3
            if (tokens.Length < 3) {
                yield return new SearchTerm {
                    ValidSyntax = false,
                    Name = tokens[0]
                };

                continue;
            }

            // if valid 
            yield return new SearchTerm {
                ValidSyntax = true,
                Name = tokens[0],
                Operator = tokens[1],
                Value = string.Join(" ", tokens.Skip(2))
            };
        }
    }

    /// <summary>
    /// Gets Get Valid Terms.
    /// </summary>
    /// <returns>The operation result.</returns>
    public IEnumerable<SearchTerm> GetValidTerms() {
        var queryTerms = GetAllTerms()
            .Where(x => x.ValidSyntax)
            .ToArray();

        if (queryTerms.Length == 0) yield break;

        var declaredTerms = GetTermsFromModel();

        foreach (var term in queryTerms) {
            var declaredTerm = declaredTerms
                .SingleOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));

            if (declaredTerm == null) continue;

            yield return new SearchTerm {
                ValidSyntax = term.ValidSyntax,
                Name = declaredTerm.Name,
                Operator = term.Operator,
                Value = term.Value,
                ExpressionProvider = declaredTerm.ExpressionProvider
            };
        }

    }

    /// <summary>
    /// Executes Apply.
    /// </summary>
    /// <param name="query">The query value.</param>
    /// <returns>The operation result.</returns>
    public IQueryable<TEntity> Apply(IQueryable<TEntity> query) {
        var terms = GetValidTerms().ToArray();
        if (terms.Length == 0) return query;

        var modifiedQuery = query;

        foreach (var term in terms) {
            if (string.IsNullOrEmpty(term.Name) ||
                string.IsNullOrEmpty(term.Operator) ||
                string.IsNullOrEmpty(term.Value)) continue;

            var proprInfo = ExpressionHelper.GetPropertyInfo<TEntity>(term.Name);

            var obj = ExpressionHelper.Parameter<TEntity>();

            // build up the linq expression backwards:
            // query = query.where(x => x.property == "Value")

            // x.prop
            var left = ExpressionHelper.GetPropertyExpression(obj, proprInfo);
            // value
            var right = term.ExpressionProvider.GetValue(term.Value!);//Expression.Constant(term.Value);

            // x.prop == value
            var comparisonExpression = term.ExpressionProvider
                .GetComparison(left, term.Operator!, right);//Expression.Equal(left, right);

            // x => x.property == value
            var lambdaExpression = ExpressionHelper
                .GetLambda<TEntity, bool>(obj, comparisonExpression);

            // query = query.where
            modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambdaExpression);

        }
        return modifiedQuery;
    }

    /// <summary>
    /// Gets Get Terms From Model.
    /// </summary>
    /// <returns>The operation result.</returns>
    private static IEnumerable<SearchTerm> GetTermsFromModel() =>
        typeof(TModel).GetTypeInfo()
        .DeclaredProperties
        .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
        .Select(p => {
            var attribute = p.GetCustomAttribute<SearchableAttribute>();
            return new SearchTerm {
                Name = p.Name,
                ExpressionProvider = attribute?.ExpressionProvider ?? new DefaultSearchExpressionProvider()
            };
        });

}
