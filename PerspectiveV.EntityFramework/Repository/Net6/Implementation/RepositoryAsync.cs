using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PerspectiveV.Common.Paging;
using PerspectiveV.EntityFramework.Extensions;
using PerspectiveV.EntityFramework.Repository.Net6.Interfaces;
using PerspectiveV.Primitives.Types.Interfaces;
using PerspectiveV.Primitives.Types.Models;
using System.Linq.Expressions;

namespace PerspectiveV.EntityFramework.Repository.Net6.Implementation;
/// <summary>
/// Provides repository behavior for Repository Async.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Derive from RepositoryAsync and implement required members.
/// var type = typeof(RepositoryAsync);
/// ]]></code>
/// </example>
[Obsolete("This class is deprecated and will be removed in future versions. Please use the updated repository implementation instead.", false)]
public abstract class RepositoryAsync<EntityType, DbContextType> : IRepositoryAsync<EntityType, DbContextType>
    where EntityType : class
    where DbContextType : DbContext {

    private readonly DbContextType _db;

    /// <summary>
    /// Initializes a new instance of the RepositoryAsync class.
    /// </summary>
    /// <param name="db">The db value.</param>
    public RepositoryAsync(DbContextType db) {
        _db = db;
    }

    #region Get
    /// <summary>
    /// Gets Get Async.
    /// </summary>
    /// <param name="include">The include value.</param>
    /// <param name="where">The where value.</param>
    /// <param name="select">The select value.</param>
    /// <param name="orderBy">The orderBy value.</param>
    /// <param name="orderDirection">The orderDirection value.</param>
    /// <param name="trackable">The trackable value.</param>
    /// <param name="getSoftDeleted">The getSoftDeleted value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<List<object>?> GetAsync(Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        Expression<Func<EntityType, bool>>? where = null,
        Func<EntityType, object>? select = null,
        Expression<Func<EntityType, object>>? orderBy = null,
        OrderDirection orderDirection = OrderDirection.Ascending,
        bool trackable = false,
        bool getSoftDeleted = false) {
        try {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();

            if (trackable == false) {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include is not null) {
                dbQuery = include(dbQuery);
            }

            // if entity implements ISoftDeletable interface
            if (ImplementsSoftDeletable()) {
                if (where is null) dbQuery = dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == getSoftDeleted).AsQueryable();
                else dbQuery = dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == getSoftDeleted).Where(where).AsQueryable();
            }
            else if (where is not null) {
                dbQuery = dbQuery.Where(where).AsQueryable();
            }

            if (orderBy is not null) {
                if (orderDirection == OrderDirection.Ascending) {
                    dbQuery = dbQuery.OrderBy(orderBy).AsQueryable();
                }
                else if (orderDirection == OrderDirection.Descending) {
                    dbQuery = dbQuery.OrderByDescending(orderBy).AsQueryable();
                }
            }

            var result = select is not null
               ? dbQuery.Select(select)
               : dbQuery;

            var list = result.ToList();
            return await Task.FromResult(list);
        }
        catch (Exception) {
            throw;
        }
    }

    /// <summary>
    /// Gets Get Async.
    /// </summary>
    /// <param name="include">The include value.</param>
    /// <param name="where">The where value.</param>
    /// <param name="select">The select value.</param>
    /// <param name="orderBy">The orderBy value.</param>
    /// <param name="orderDirection">The orderDirection value.</param>
    /// <param name="trackable">The trackable value.</param>
    /// <param name="getSoftDeleted">The getSoftDeleted value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<List<TResult>?> GetAsync<TResult>(Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
    Expression<Func<EntityType, bool>>? where = null,
    Func<EntityType, object>? select = null,
    Expression<Func<EntityType, object>>? orderBy = null,
    OrderDirection orderDirection = OrderDirection.Ascending,
    bool trackable = false,
    bool getSoftDeleted = false) {
        try {
            var data = await GetAsync(include, where, select, orderBy, orderDirection, trackable, getSoftDeleted);
            var result = new List<TResult>();
            if (data is not null && data.Count > 0) {
                foreach (var item in data) {
                    var asitem = (TResult)item;
                    result.Add(asitem);
                }
            }

            return result;
        }
        catch (Exception) {
            throw;
        }
    }

    #endregion

    #region Paged
    /// <summary>
    /// Executes Paged Async.
    /// </summary>
    /// <param name="include">The include value.</param>
    /// <param name="where">The where value.</param>
    /// <param name="select">The select value.</param>
    /// <param name="pagingOptions">The pagingOptions value.</param>
    /// <param name="orderBy">The orderBy value.</param>
    /// <param name="orderDirection">The orderDirection value.</param>
    /// <param name="trackable">The trackable value.</param>
    /// <param name="getSoftDeleted">The getSoftDeleted value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<PagedResult<object>?> PagedAsync(Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        Expression<Func<EntityType, bool>>? where = null,
        Func<EntityType, object>? select = null,
        PagingOptions? pagingOptions = null,
        Expression<Func<EntityType, object>>? orderBy = null,
        OrderDirection orderDirection = OrderDirection.Ascending,
        bool trackable = false,
        bool getSoftDeleted = false) {
        try {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();
            var totalRecordsCount = 0;  // getSoftDeleted ? await dbQuery.CountAsync() : await dbQuery.Where(x => !x.IsDeleted).CountAsync();

            if (trackable == false) {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include is not null) {
                dbQuery = include(dbQuery);
            }

            // if entity implements ISoftDeletable interface
            if (ImplementsSoftDeletable()) {
                if (where is null) {
                    dbQuery = dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == getSoftDeleted).AsQueryable();
                    totalRecordsCount = await dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == getSoftDeleted).CountAsync();
                }
                else {
                    dbQuery = dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == getSoftDeleted).Where(where).AsQueryable();
                    totalRecordsCount = await dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == getSoftDeleted).Where(where).CountAsync();
                }
            }
            else if (where is not null) {
                dbQuery = dbQuery.Where(where).AsQueryable();
                totalRecordsCount = await dbQuery.Where(where).CountAsync();
            }

            if (orderBy is not null) {
                if (orderDirection == OrderDirection.Ascending) {
                    dbQuery = dbQuery.OrderBy(orderBy);
                }
                else if (orderDirection == OrderDirection.Descending) {
                    dbQuery = dbQuery.OrderByDescending(orderBy);
                }
            }

            if (pagingOptions is not null) {
                dbQuery = dbQuery
                    .Skip(pagingOptions.Offset.GetValueOrDefault())
                    .Take(pagingOptions.Limit.GetValueOrDefault());
            }

            var result = select is not null
                ? dbQuery.Select(select)
                : dbQuery.AsQueryable();

            var list = result.ToList();


            return new PagedResult<object> {
                Items = list,
                Limit = pagingOptions?.Limit.GetValueOrDefault(),
                Offset = pagingOptions?.Offset.GetValueOrDefault(),
                ItemsCount = list.Count,
                TotalItemsCount = totalRecordsCount
            };
        }
        catch (Exception) {
            throw;
        }
    }

    /// <summary>
    /// Executes Paged Async.
    /// </summary>
    /// <param name="include">The include value.</param>
    /// <param name="where">The where value.</param>
    /// <param name="select">The select value.</param>
    /// <param name="pagingOptions">The pagingOptions value.</param>
    /// <param name="orderBy">The orderBy value.</param>
    /// <param name="orderDirection">The orderDirection value.</param>
    /// <param name="trackable">The trackable value.</param>
    /// <param name="getSoftDeleted">The getSoftDeleted value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<PagedResult<TResult>?> PagedAsync<TResult>(Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
      Expression<Func<EntityType, bool>>? where = null,
      Func<EntityType, object>? select = null,
      PagingOptions? pagingOptions = null,
      Expression<Func<EntityType, object>>? orderBy = null,
      OrderDirection orderDirection = OrderDirection.Ascending,
      bool trackable = false,
      bool getSoftDeleted = false) {
        try {
            // TODO: Add typed query projection support for TResult when a projection mapper is available.
            var data = await PagedAsync(include, where, select, pagingOptions, orderBy, orderDirection, trackable, getSoftDeleted);
            if (data is null) throw new InvalidOperationException("Paged query returned null data.");

            var result = new PagedResult<TResult>() {
                ItemsCount = data.ItemsCount,
                TotalItemsCount = data.TotalItemsCount,
                Limit = data.Limit,
                Offset = data.Offset
            };
            if (data?.Items is not null && data.ItemsCount > 0) {
                var items = new List<TResult>();

                foreach (var item in data.Items) {
                    if (item is EntityType asEntity) {
                        if (asEntity is TResult asItem) {
                            items.Add(asItem);
                        }
                    }
                }

                result.Items = items ?? [];
            }

            return result;

        }
        catch (Exception) {
            throw;
        }
    }
    #endregion

    #region Single Or Default
    /// <summary>
    /// Gets Get Single Or Default Async.
    /// </summary>
    /// <param name="where">The where value.</param>
    /// <param name="include">The include value.</param>
    /// <param name="trackable">The trackable value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<EntityType?> GetSingleOrDefaultAsync(Expression<Func<EntityType, bool>>? where = null, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false) {
        try {
            EntityType? item = default;
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();

            if (trackable is false) {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include != null) {
                dbQuery = include(dbQuery);
            }

            if (ImplementsSoftDeletable()) {
                if (where is null) dbQuery = dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == false).AsQueryable();
                else dbQuery = dbQuery.Where(where).AsQueryable();
            }
            else if (where is not null) {
                dbQuery = dbQuery.Where(where).AsQueryable();
            }


            item = where != null ? await dbQuery.FirstOrDefaultAsync()
                : await dbQuery.FirstOrDefaultAsync();

            return item;
        }
        catch (Exception) {
            throw;
        }
    }
    #endregion

    #region First Or Default
    /// <summary>
    /// Gets Get First Or Default Async.
    /// </summary>
    /// <param name="where">The where value.</param>
    /// <param name="include">The include value.</param>
    /// <param name="trackable">The trackable value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<EntityType?> GetFirstOrDefaultAsync(Expression<Func<EntityType, bool>>? where = null, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false) {
        try {
            EntityType? item = default;
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();

            if (trackable is false) {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include is not null) {
                dbQuery = include(dbQuery);
            }

            if (ImplementsSoftDeletable()) {
                if (where is null) dbQuery = dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == false).AsQueryable();
                else dbQuery = dbQuery.Where(where).AsQueryable();
            }
            else if (where is not null) {
                dbQuery = dbQuery.Where(where).AsQueryable();
            }

            item = where != null ? await dbQuery.FirstOrDefaultAsync()
                : await dbQuery.FirstOrDefaultAsync();
            return item;
        }
        catch (Exception) {
            throw;
        }
    }
    #endregion

    #region Queryable
    /// <summary>
    /// Gets Get Queryable.
    /// </summary>
    /// <param name="include">The include value.</param>
    /// <param name="trackable">The trackable value.</param>
    /// <returns>The operation result.</returns>
    public virtual IQueryable<EntityType> GetQueryable(Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false) {
        try {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();

            if (trackable is false) {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include is not null) {
                dbQuery = include(dbQuery);
            }

            return dbQuery;
        }
        catch (Exception) {
            throw;
        }
    }
    #endregion

    #region GetById
    /// <summary>
    /// Gets Get By Id Async.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<EntityType?> GetByIdAsync(string id) {
        try {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();
            Expression<Func<EntityType, bool>>? where = null;

            if (ImplementsSoftDeletable() && ImplementsEntity()) {
                where = x => ((IEntity)x).Id.ToString() == id && ((ISoftDeletable)x).IsDeleted == false;
            }
            else if (ImplementsEntity()) {
                where = x => ((IEntity)x).Id.ToString() == id;
            }

            EntityType? item = default;
            if (where is not null) {
                item = await dbQuery.FirstOrDefaultAsync(where);
            }

            return item;
        }
        catch (Exception) {
            throw;
        }
    }
    #endregion

    #region GetCount
    /// <summary>
    /// Gets Get Count Async.
    /// </summary>
    /// <param name="where">The where value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<long> GetCountAsync(Expression<Func<EntityType, bool>>? where = null) {
        try {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsNoTracking().AsQueryable();
            if (where != null) {
                dbQuery = dbQuery.Where(where).AsQueryable();
            }
            return await dbQuery.CountAsync();
        }
        catch (Exception) {
            throw;
        }
    }
    #endregion

    #region Create
    /// <summary>
    /// Creates Create Async.
    /// </summary>
    /// <param name="items">The items value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task CreateAsync(params EntityType[] items) {
        using var comit = await _db.Database.BeginTransactionAsync();
        try {
            foreach (EntityType item in items) {
                _db.Entry(item).State = EntityState.Added;
                await _db.SaveChangesAsync();
            }
            await comit.CommitAsync();
        }
        catch (Exception) {
            await comit.RollbackAsync();
            throw;
        }
    }
    #endregion

    #region Update
    /// <summary>
    /// Updates Update Async.
    /// </summary>
    /// <param name="items">The items value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task UpdateAsync(params EntityType[] items) {
        using var comit = await _db.Database.BeginTransactionAsync();
        try {
            foreach (EntityType item in items) {
                if (ImplementsEntity(item)) {
                    _db.DetachLocalObject(item, $"{GetPropertyValueIfExists(item, $"{nameof(IEntity.Id)}")}");
                }
                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            await comit.CommitAsync();
        }
        catch (Exception) {
            await comit.RollbackAsync();
            throw;
        }
    }
    #endregion

    #region Transaction
    /// <summary>
    /// Executes Transaction Async.
    /// </summary>
    /// <param name="action">The action value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task TransactionAsync(Func<DbContextType, Task>? action = null) {
        using var comit = await _db.Database.BeginTransactionAsync();
        try {
            if (action != null) {
                await action(_db);
            }
            await comit.CommitAsync();
            _db.ClearChanges();
        }
        catch (Exception) {
            await comit.RollbackAsync();
            throw;
        }
    }
    #endregion

    #region Soft Delete
    /// <summary>
    /// Executes Soft Delete Async.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <param name="where">The where value.</param>
    /// <param name="items">The items value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task SoftDeleteAsync(string? id = null, Expression<Func<EntityType, bool>>? where = null, params EntityType[] items) {
        using var commit = await _db.Database.BeginTransactionAsync();
        try {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();
            #region if Items
            if (items != null && items.Length != 0) {
                foreach (EntityType item in items) {
                    if (item is ISoftDeletable) {
                        SetPropertyValueIfExists(item, nameof(ISoftDeletable.IsDeleted), true);
                    }
                    _db.Entry(item).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
            }
            #endregion
            #region else If Id or where
            else if (!string.IsNullOrEmpty(id) || where != null) {
                Expression<Func<EntityType, bool>> expresssion = !string.IsNullOrEmpty(id) ? x => ((IEntity)x).Id.ToString() == id : where ?? (x => true);

                if (expresssion != null) {
                    var itemsToDelete = await dbQuery.Where(expresssion).ToListAsync();
                    foreach (EntityType item in itemsToDelete) {
                        if (item is ISoftDeletable) {
                            SetPropertyValueIfExists(item, nameof(ISoftDeletable.IsDeleted), true);
                        }
                        _db.Entry(item).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                }
            }
            #endregion
            await commit.CommitAsync();
        }
        catch (Exception) {
            await commit.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Executes Soft Delete Async.
    /// </summary>
    /// <param name="itemId">The itemId value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task SoftDeleteAsync(string itemId) => await SoftDeleteAsync(id: itemId);

    /// <summary>
    /// Executes Soft Delete Async.
    /// </summary>
    /// <param name="itemsToDelete">The itemsToDelete value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task SoftDeleteAsync(params EntityType[] itemsToDelete) => await SoftDeleteAsync(items: itemsToDelete);

    /// <summary>
    /// Executes Soft Delete Async.
    /// </summary>
    /// <param name="deleteCondition">The deleteCondition value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task SoftDeleteAsync(Expression<Func<EntityType, bool>> deleteCondition) => await SoftDeleteAsync(where: deleteCondition);
    #endregion

    #region Delete
    /// <summary>
    /// Deletes Delete Async.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <param name="where">The where value.</param>
    /// <param name="items">The items value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task DeleteAsync(string? id = null, Expression<Func<EntityType, bool>>? where = null, params EntityType[] items) {
        using var commit = await _db.Database.BeginTransactionAsync();
        try {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();
            #region if Items
            if (items != null && items.Length != 0) {
                foreach (EntityType item in items) {
                    if (ImplementsEntity(item)) {
                        _db.DetachLocalObject(item, $"{GetPropertyValueIfExists(item, $"{nameof(IEntity.Id)}")}");
                    }

                    _db.Entry(item).State = EntityState.Deleted;
                    await _db.SaveChangesAsync();
                }
            }
            #endregion
            #region else If Id or where
            else if (!string.IsNullOrEmpty(id) || where != null) {
                Expression<Func<EntityType, bool>>? expresssion = null;

                if (!string.IsNullOrEmpty(id)) expresssion = x => ((IEntity)x).Id.ToString() == id;
                else if (where != null) expresssion = where;

                if (expresssion != null && items is not null) {
                    var itemsToDelete = await dbQuery.Where(expresssion).ToListAsync();
                    foreach (EntityType item in items) {
                        if (ImplementsEntity(item)) {
                            _db.DetachLocalObject(item, $"{GetPropertyValueIfExists(item, $"{nameof(IEntity.Id)}")}");
                        }
                        _db.Entry(item).State = EntityState.Deleted;
                        await _db.SaveChangesAsync();
                    }
                }
            }
            #endregion
            await commit.CommitAsync();
        }
        catch (Exception) {
            await commit.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Deletes Delete Async.
    /// </summary>
    /// <param name="itemId">The itemId value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task DeleteAsync(string itemId) => await DeleteAsync(id: itemId);

    /// <summary>
    /// Deletes Delete Async.
    /// </summary>
    /// <param name="itemsToDelete">The itemsToDelete value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task DeleteAsync(params EntityType[] itemsToDelete) => await DeleteAsync(items: itemsToDelete);

    /// <summary>
    /// Deletes Delete Async.
    /// </summary>
    /// <param name="deleteCondition">The deleteCondition value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task DeleteAsync(Expression<Func<EntityType, bool>> deleteCondition) => await DeleteAsync(where: deleteCondition);
    #endregion

    /// <summary>
    /// Executes Implements Soft Deletable.
    /// </summary>
    /// <returns>The operation result.</returns>
    private static bool ImplementsSoftDeletable() {
        return typeof(ISoftDeletable).IsAssignableFrom(typeof(EntityType));
    }
    /// <summary>
    /// Executes Implements Entity.
    /// </summary>
    /// <returns>The operation result.</returns>
    private static bool ImplementsEntity() {
        return
            typeof(IEntity).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<int>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<long>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<double>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<char>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<string>).IsAssignableFrom(typeof(EntityType))
            ;
    }
    /// <summary>
    /// Executes Implements Entity.
    /// </summary>
    /// <param name="item">The item value.</param>
    /// <returns>The operation result.</returns>
    private static bool ImplementsEntity(EntityType item) {
        return
            item is IEntity ||
            item is IEntity<int> ||
            item is IEntity<long> ||
            item is IEntity<double> ||
            item is IEntity<char> ||
            item is IEntity<string>
            ;
    }

    /// <summary>
    /// Sets Set Property Value If Exists.
    /// </summary>
    /// <param name="m">The m value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <param name="value">The value value.</param>
    private static void SetPropertyValueIfExists<T>(T m, string propertyName, object value) {
        var property = typeof(T).GetProperty(propertyName);
        if (property != null) {
            typeof(T).GetProperty(propertyName)?.SetValue(m, value);
        }
    }
    /// <summary>
    /// Gets Get Property Value If Exists.
    /// </summary>
    /// <param name="m">The m value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <returns>The operation result.</returns>
    private static object? GetPropertyValueIfExists<T>(T m, string propertyName) {
        return typeof(T).GetProperty(propertyName)?.GetValue(m);
    }
}
