using PerspectiveV.Common.Paging.Processors;
using System.ComponentModel.DataAnnotations;

namespace PerspectiveV.Common.Paging;
/// <summary>
/// Represents configuration options for Search Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SearchOptions.
/// var type = typeof(SearchOptions<,>);
/// ]]></code>
/// </example>
public class SearchOptions<TEntity, TModel> : IValidatableObject {

    public string[] Search { get; set; } = [];
    /// <summary>
    /// Executes Validate.
    /// </summary>
    /// <param name="validationContext">The validationContext value.</param>
    /// <returns>The operation result.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        var processor = new SearchOptionsProcessor<TEntity, TModel>(Search);

        var validTerms = processor.GetValidTerms().Select(x => x.Name);

        var invalidTerms = processor.GetAllTerms().Select(x => x.Name)
            .Except(validTerms, StringComparer.OrdinalIgnoreCase);

        foreach (var term in invalidTerms) {
            yield return new ValidationResult(
                $"Invalid search term '{term}'",
                new[] { nameof(Search) }
             );
        }
    }

    /// <summary>
    /// Executes Apply.
    /// </summary>
    /// <param name="query">The query value.</param>
    /// <returns>The operation result.</returns>
    public IQueryable<TEntity> Apply(IQueryable<TEntity> query) {
        var processor = new SearchOptionsProcessor<TEntity, TModel>(Search);
        return processor.Apply(query);

    }
}
