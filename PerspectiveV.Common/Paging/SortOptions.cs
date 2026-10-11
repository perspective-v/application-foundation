using PerspectiveV.Common.Paging.Processors;
using System.ComponentModel.DataAnnotations;

namespace PerspectiveV.Common.Paging;
/// <summary>
/// Represents configuration options for Sort Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SortOptions.
/// var type = typeof(SortOptions<,>);
/// ]]></code>
/// </example>
public class SortOptions<TEntity, TModel> : IValidatableObject {
    public string[]? OrderBy { get; set; }

    // asp.net core calls this method to validate the incoming parameters
    /// <summary>
    /// Executes Validate.
    /// </summary>
    /// <param name="validationContext">The validationContext value.</param>
    /// <returns>The operation result.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        var processor = new SortOptionsProcessor<TEntity, TModel>(OrderBy);
        var validTerms = processor.GetValidTerms().Select(x => x.Name);
        var invalidTerms = processor.GetAllTerms().Select(x => x.Name).Except(validTerms, StringComparer.OrdinalIgnoreCase);

        foreach (var term in invalidTerms) {
            yield return new ValidationResult($"Invalid sort term '{term}'", new[] { nameof(OrderBy) });
        }
    }
    // the service code will call this to apply these sort options to query
    /// <summary>
    /// Executes Apply.
    /// </summary>
    /// <param name="query">The query value.</param>
    /// <returns>The operation result.</returns>
    public IQueryable<TEntity> Apply(IQueryable<TEntity> query) {
        var processor = new SortOptionsProcessor<TEntity, TModel>(OrderBy);
        return processor.Apply(query);
    }
}
