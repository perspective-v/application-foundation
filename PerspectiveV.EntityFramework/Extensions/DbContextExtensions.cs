using Microsoft.EntityFrameworkCore;
using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.EntityFramework.Extensions;

/// <summary>
/// Provides extension methods for Db Context Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by DbContextExtensions.
/// var typeName = nameof(DbContextExtensions);
/// ]]></code>
/// </example>
public static class DbContextExtensions {
    public static void DetachLocal<T>(this DbContext context, T t, string entryId) where T : class, IEntity {
        var local = context.Set<T>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(entryId));
        if (local != null) {
            context.Entry(local).State = EntityState.Detached;
        }
        //context.Entry(t).State = EntityState.Modified;
    }

    public static void DetachLocalObject<T>(this DbContext context, T t, string entryId) where T : class {
        Func<T, bool>? where = null;

        if (ImplementsEntity(t)) {
            where = x => ((IEntity)x).Id.ToString().Equals(entryId);
        }

        if (where is not null) {
            var local = context.Set<T>()
                .Local
                .FirstOrDefault(where);

            if (local != null) {
                context.Entry(local).State = EntityState.Detached;
            }
        }
    }

    /// <summary>
    /// Executes Clear Changes.
    /// </summary>
    /// <param name="ctx">The ctx value.</param>
    public static void ClearChanges(this DbContext ctx) {
        var changedEntriesCopy = ctx.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in changedEntriesCopy)
            entry.State = EntityState.Detached;
    }

    /// <summary>
    /// Executes Implements Entity.
    /// </summary>
    /// <param name="item">The item value.</param>
    /// <returns>The operation result.</returns>
    private static bool ImplementsEntity<T>(T item) {
        return (
            item is IEntity ||
            item is IEntity<int> ||
            item is IEntity<long> ||
            item is IEntity<double> ||
            item is IEntity<decimal> ||
            item is IEntity<short> ||
            item is IEntity<char> ||
            item is IEntity<Guid> ||
            item is IEntity<string>
            );
    }
}
