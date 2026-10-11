using PerspectiveV.Common.Extensions;
using PerspectiveV.Common.Helpers;
using PerspectiveV.Common.Paging.Attributes;
using PerspectiveV.Primitives.Types.Models;
using System.Reflection;

namespace PerspectiveV.Common.Paging.Processors;
/// <summary>
/// Represents the SortOptionsProcessor type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SortOptionsProcessor.
/// var type = typeof(SortOptionsProcessor<,>);
/// ]]></code>
/// </example>
public class SortOptionsProcessor<TEntity, TModel> {
    private readonly string[]? _orderBy;

    /// <summary>
    /// Initializes a new instance of the SortOptionsProcessor class.
    /// </summary>
    /// <param name="orderBy">The orderBy value.</param>
    public SortOptionsProcessor(params string[]? orderBy) {
        _orderBy = orderBy;
    }

    /// <summary>
    /// Gets Get All Terms.
    /// </summary>
    /// <returns>The operation result.</returns>
    public IEnumerable<SortTerm> GetAllTerms() {
        if (_orderBy == null) yield break;

        foreach (var term in _orderBy) {
            if (string.IsNullOrEmpty(term)) continue;

            var tokens = term.Split(' ');

            if (tokens.Length == 0) {
                yield return new SortTerm { Name = term };
                continue;
            }

            var desc = tokens.Length > 1 && tokens[1].Equals(EnumExtension.GetDescription(OrderDirection.Descending), StringComparison.OrdinalIgnoreCase);
            yield return new SortTerm {
                Name = tokens[0],
                Descending = desc,
            };
        }
    }

    /// <summary>
    /// Gets Get Valid Terms.
    /// </summary>
    /// <returns>The operation result.</returns>
    public IEnumerable<SortTerm> GetValidTerms() {
        var queryTerms = GetAllTerms().ToArray();
        if (queryTerms.Length == 0) yield break;

        var declaredTerms = GetTermsFromModel();
        foreach (var term in queryTerms) {
            var declaredTerm = declaredTerms.SingleOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));
            if (declaredTerm == null) continue;

            yield return new SortTerm {
                Name = declaredTerm.Name,
                Descending = term.Descending,
                Default = declaredTerm.Default
            };
        }
    }

    /// <summary>
    /// Gets Get Terms From Model.
    /// </summary>
    /// <returns>The operation result.</returns>
    private static IEnumerable<SortTerm> GetTermsFromModel() {
        //var properties = typeof(TModel).GetTypeInfo()
        //    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy) //BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy
        //    .Where(p => p.GetCustomAttributes<SortableAttribute>().Any())
        //    .Select(p => new SortTerm { Name = p.Name, Default = p.GetCustomAttribute<SortableAttribute>().Default });
        //return properties;
        return typeof(TModel).GetTypeInfo()
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SortableAttribute>().Any())
            .Select(p => new SortTerm { Name = p.Name, Default = p.GetCustomAttribute<SortableAttribute>()?.Default ?? false });
    }

    /// <summary>
    /// Executes Apply.
    /// </summary>
    /// <param name="query">The query value.</param>
    /// <returns>The operation result.</returns>
    public IQueryable<TEntity> Apply(IQueryable<TEntity> query) {
        var terms = GetValidTerms().ToArray();
        if (terms.Length == 0) {
            terms = GetTermsFromModel().Where(t => t.Default).ToArray();
        }

        if (terms.Length == 0) return query;

        var modifiedQuery = query;
        var useThenBy = false;
        foreach (var term in terms) {
            if (string.IsNullOrEmpty(term.Name)) continue;
            var propertyInfo = ExpressionHelper.GetPropertyInfo<TEntity>(term.Name);
            var obj = ExpressionHelper.Parameter<TEntity>();

            // build the linkq expression backwards;
            // query = query.Orderby(x => x.property);

            // x => x.Property
            var key = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);

            var keySelector = ExpressionHelper.GetLambda(typeof(TEntity), propertyInfo.PropertyType, obj, key);

            // then by
            // query.OrderBy/ThenBy[Descending](x => x.Propery)
            modifiedQuery = ExpressionHelper.CallOrderByOrThenBy(modifiedQuery, useThenBy, term.Descending, propertyInfo.PropertyType, keySelector);

            useThenBy = true;
        }
        return modifiedQuery;
    }
}
