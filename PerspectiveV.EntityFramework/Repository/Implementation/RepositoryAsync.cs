using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PerspectiveV.Common.Extensions;
using PerspectiveV.Common.Mapper;
using PerspectiveV.Common.Paging;
using PerspectiveV.EntityFramework.Extensions;
using PerspectiveV.EntityFramework.Repository.Interfaces;
using PerspectiveV.Primitives.Types.Interfaces;
using System.Linq.Expressions;

namespace PerspectiveV.EntityFramework.Repository.Implementation;


/// <summary>
/// Generic EF Core repository for query, paging, create, update, soft delete, and purge workflows.
/// </summary>
/// <typeparam name="IdType"></typeparam>
/// <typeparam name="EntityType"></typeparam>
/// <typeparam name="ModelType"></typeparam>
/// <typeparam name="DbContextType"></typeparam>
/// <param name="db"></param>
/// <param name="mapper"></param>
/// <example>
/// <code><![CDATA[
/// // Inherit and bind your concrete types.
/// public class UserRepository
///     : RepositoryAsync<User, UserDto, AppDbContext>
/// {
///     public UserRepository(AppDbContext db, IProjectionMapper<User, UserDto> mapper)
///         : base(db, mapper)
///     {
///     }
/// }
/// ]]></code>
/// </example>
public abstract class RepositoryAsync<IdType, EntityType, ModelType, DbContextType>(DbContextType db, IProjectionMapper<IdType, EntityType, ModelType> mapper) : IRepositoryAsync<IdType, EntityType, ModelType, DbContextType>
    where ModelType : class
    where EntityType : class
    where DbContextType : DbContext
{

    private readonly DbContextType _db = db;
    private readonly IProjectionMapper<IdType, EntityType, ModelType> _mapper = mapper;

    #region Get
    /// <summary>
    /// Returns a list of projected models with optional include/filter/sort behavior.
    /// </summary>
    /// <param name="include">Optional include pipeline for eager-loading navigation properties.</param>
    /// <param name="where">Optional predicate used to filter rows.</param>
    /// <param name="select">Optional selector that overrides default mapper projection.</param>
    /// <param name="sortOptions">Sort options applied before materialization.</param>
    /// <param name="trackable">When <see langword="true"/>, query results remain tracked by EF Core.</param>
    /// <param name="getSoftDeleted">When <see langword="true"/>, requests soft-deleted rows for soft-deletable entities.</param>
    /// <returns>Projected model list.</returns>
    public virtual async Task<List<ModelType>> GetAsync(
            Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
            Expression<Func<EntityType, bool>>? where = null,
            Func<EntityType, ModelType>? select = null,
            SortOptions<EntityType, ModelType>? sortOptions = null,
            bool trackable = false,
            bool getSoftDeleted = false,
            CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();

            if (trackable == false) dbQuery = dbQuery.AsNoTracking();

            // if entity implements ISoftDeletable interface
            if (IsSoftDeletable && getSoftDeleted == false)
            {
                var softDeleteFilter = EfCoreExtensions.IsDeleted<EntityType>(false);
                where = (where != null) ? where.And(softDeleteFilter) : softDeleteFilter;
            }

            if (where is not null)
            {
                dbQuery = dbQuery.Where(where);
            }

            if (sortOptions?.OrderBy != null && Array.Exists(sortOptions.OrderBy, s => !string.IsNullOrWhiteSpace(s)))
            {
                dbQuery = sortOptions.Apply(dbQuery);
            }
            else
            {
                // fallback for sorting
                if (IsIEntity)
                {
                    dbQuery = dbQuery.OrderBy(e => EF.Property<EntityType>(e, nameof(IEntity.Id)));
                }
            }

            if (include is not null) dbQuery = include(dbQuery);


            List<ModelType> list = [];
            // If a custom selector is provided, use it; otherwise, map to TResult
            if (select is not null)
            {
                list = [.. dbQuery.Select(select)];
            }
            else
            {
                list = await _mapper.ProjectToModel(dbQuery).ToListAsync(cancellationToken);
            }

            return list;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Returns a list of projected values for a requested result type.
    /// </summary>
    /// <param name="include">Optional include pipeline for eager-loading navigation properties.</param>
    /// <param name="where">Optional predicate used to filter rows.</param>
    /// <param name="select">Optional selector that overrides default projection logic.</param>
    /// <param name="sortOptions">Sort options applied before materialization.</param>
    /// <param name="trackable">When <see langword="true"/>, query results remain tracked by EF Core.</param>
    /// <param name="getSoftDeleted">When <see langword="true"/>, requests soft-deleted rows for soft-deletable entities.</param>
    /// <returns>Projected result list.</returns>
    public virtual async Task<List<TResult>> GetAsync<TResult>(Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
            Expression<Func<EntityType, bool>>? where = null,
            Func<EntityType, TResult>? select = null,
            SortOptions<EntityType, ModelType>? sortOptions = null,
            bool trackable = false,
            bool getSoftDeleted = false,
            CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();

            if (trackable == false) dbQuery = dbQuery.AsNoTracking();

            // if entity implements ISoftDeletable interface
            if (IsSoftDeletable)
            {
                dbQuery = dbQuery.Where(x => ((ISoftDeletable)x).IsDeleted == getSoftDeleted);
            }

            if (where is not null)
            {
                dbQuery = dbQuery.Where(where);
            }

            if (sortOptions?.OrderBy != null && Array.Exists(sortOptions.OrderBy, s => !string.IsNullOrWhiteSpace(s)))
            {
                dbQuery = sortOptions.Apply(dbQuery);
            }
            else
            {
                // fallback for sorting
                if (IsIEntity)
                {
                    dbQuery = dbQuery.OrderBy(e => EF.Property<EntityType>(e, nameof(IEntity.Id)));
                }
            }

            if (include is not null) dbQuery = include(dbQuery);

            List<TResult> list = [];
            // If a custom selector is provided, use it; otherwise, map to TResult
            if (select is not null)
            {
                list = [.. dbQuery.Select(select)];
            }
            else if (typeof(TResult) == typeof(ModelType))
            {
                var projected = await _mapper.ProjectToModel(dbQuery).ToListAsync(cancellationToken);
                list = [.. projected.Cast<TResult>()];
            }
            else if (typeof(TResult) == typeof(EntityType))
            {
                list = await dbQuery.Cast<TResult>().ToListAsync(cancellationToken);
            }
            else
            {
                list = [.. dbQuery.AsEnumerable().Select(x => (TResult)(object)x)];
            }

            return list;
        }
        catch (Exception)
        {
            throw;
        }
    }

    #endregion

    #region Paged
    /// <summary>
    /// Returns a paged result with optional filtering, searching, sorting, and projection.
    /// </summary>
    /// <param name="include">Optional include pipeline for eager-loading navigation properties.</param>
    /// <param name="where">Optional predicate used to filter rows.</param>
    /// <param name="select">Optional selector that overrides default mapper projection.</param>
    /// <param name="pagingOptions">Offset/limit values for paging.</param>
    /// <param name="searchOptions">Search expressions applied before paging.</param>
    /// <param name="sortOptions">Sort expressions applied before paging.</param>
    /// <param name="trackable">When <see langword="true"/>, query results remain tracked by EF Core.</param>
    /// <param name="getSoftDeleted">When <see langword="true"/>, requests soft-deleted rows for soft-deletable entities.</param>
    /// <returns>Paged model response with total count metadata.</returns>
    public virtual async Task<PagedResult<ModelType>> PagedAsync(
            Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
            Expression<Func<EntityType, bool>>? where = null,
            Func<EntityType, ModelType>? select = null,
            PagingOptions? pagingOptions = null,
            SearchOptions<EntityType, ModelType>? searchOptions = null,
            SortOptions<EntityType, ModelType>? sortOptions = null,
            bool trackable = false,
            bool getSoftDeleted = false,
            CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();

            if (trackable == false) dbQuery = dbQuery.AsNoTracking();

            // if entity implements ISoftDeletable interface
            if (IsSoftDeletable && getSoftDeleted == false)
            {
                var softDeleteFilter = EfCoreExtensions.IsDeleted<EntityType>(false);
                where = (where != null) ? where.And(softDeleteFilter) : softDeleteFilter;
            }

            if (where is not null)
            {
                dbQuery = dbQuery.Where(where);
            }

            if (searchOptions?.Search != null && Array.Exists(searchOptions.Search, s => !string.IsNullOrWhiteSpace(s)))
            {
                dbQuery = searchOptions.Apply(dbQuery);
            }

            if (sortOptions?.OrderBy != null && Array.Exists(sortOptions.OrderBy, s => !string.IsNullOrWhiteSpace(s)))
            {
                dbQuery = sortOptions.Apply(dbQuery);
            }
            else
            {
                // fallback for sorting
                if (IsIEntity)
                {
                    dbQuery = dbQuery.OrderBy(e => EF.Property<EntityType>(e, nameof(IEntity.Id)));
                }
            }

            var totalRecordsCount = await dbQuery.CountAsync(cancellationToken);

            var offset = pagingOptions?.Offset.GetValueOrDefault() ?? 0;
            var limit = pagingOptions?.Limit.GetValueOrDefault() ?? int.MaxValue;

            if (pagingOptions is not null)
            {
                dbQuery = dbQuery.Skip(offset).Take(limit);
            }

            if (include is not null) dbQuery = include(dbQuery);

            List<ModelType> list = [];
            // If a custom selector is provided, use it; otherwise, map to TResult
            if (select is not null)
            {
                list = [.. dbQuery.Select(select)];
            }
            else
            {
                list = await _mapper.ProjectToModel(dbQuery).ToListAsync(cancellationToken);
            }

            return new PagedResult<ModelType>
            {
                Items = list,
                Offset = offset,
                Limit = limit,
                ItemsCount = list.Count,
                TotalItemsCount = totalRecordsCount
            };
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region Single Or Default
    /// <summary>
    /// Returns a single model that matches the predicate, or <see langword="null"/> when no row exists.
    /// </summary>
    /// <param name="where">Optional predicate used to select the row.</param>
    /// <param name="include">Optional include pipeline for eager-loading navigation properties.</param>
    /// <param name="trackable">When <see langword="true"/>, query results remain tracked by EF Core.</param>
    /// <returns>Mapped model when found; otherwise <see langword="null"/>.</returns>
    public virtual async Task<ModelType?> GetSingleOrDefaultAsync(Expression<Func<EntityType, bool>>? where = null, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false, bool getSoftDeleted = false, CancellationToken cancellationToken = default)
    {
        try
        {
            EntityType? item = default;
            ModelType? modelType = default;
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();

            if (trackable is false)
            {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include != null)
            {
                dbQuery = include(dbQuery);
            }

            if (IsSoftDeletable && getSoftDeleted == false)
            {
                var softDeleteFilter = EfCoreExtensions.IsDeleted<EntityType>(false);
                where = (where != null) ? where.And(softDeleteFilter) : softDeleteFilter;
            }

            if (where is not null)
            {
                dbQuery = dbQuery.Where(where);
            }

            item = await dbQuery.SingleOrDefaultAsync(cancellationToken);

            if (item is not null)
            {
                modelType = Activator.CreateInstance<ModelType>();
                modelType = _mapper.ToModel(item);
            }

            return modelType;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region First Or Default
    /// <summary>
    /// Returns the first model that matches the predicate, or <see langword="null"/> when no row exists.
    /// </summary>
    /// <param name="where">Optional predicate used to select rows.</param>
    /// <param name="include">Optional include pipeline for eager-loading navigation properties.</param>
    /// <param name="trackable">When <see langword="true"/>, query results remain tracked by EF Core.</param>
    /// <returns>Mapped model when found; otherwise <see langword="null"/>.</returns>
    public virtual async Task<ModelType?> GetFirstOrDefaultAsync(Expression<Func<EntityType, bool>>? where = null, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false, bool getSoftDeleted = false, CancellationToken cancellationToken = default)
    {
        try
        {
            EntityType? item = default;
            ModelType? modelType = default;
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();

            if (trackable is false)
            {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include != null)
            {
                dbQuery = include(dbQuery);
            }

            if (IsSoftDeletable && getSoftDeleted == false)
            {
                var softDeleteFilter = EfCoreExtensions.IsDeleted<EntityType>(false);
                where = (where != null) ? where.And(softDeleteFilter) : softDeleteFilter;
            }

            if (where is not null)
            {
                dbQuery = dbQuery.Where(where);
            }

            item = await dbQuery.FirstOrDefaultAsync(cancellationToken);

            if (item is not null)
            {
                modelType = Activator.CreateInstance<ModelType>();
                modelType = _mapper.ToModel(item);
            }

            return modelType;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region Queryable
    /// <summary>
    /// Returns the base queryable for the entity with optional includes and tracking mode.
    /// </summary>
    /// <param name="include">Optional include pipeline for eager-loading navigation properties.</param>
    /// <param name="trackable">When <see langword="true"/>, query results remain tracked by EF Core.</param>
    /// <returns>Configurable entity queryable.</returns>
    public virtual IQueryable<EntityType> GetQueryable(Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false)
    {
        try
        {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();

            if (trackable is false)
            {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include is not null)
            {
                dbQuery = include(dbQuery);
            }

            return dbQuery;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region GetById
    /// <summary>
    /// Returns a model by identifier.
    /// </summary>
    /// <param name="id">Identifier value to locate.</param>
    /// <returns>Mapped model when found; otherwise <see langword="null"/>.</returns>
    public virtual async Task<ModelType?> GetByIdAsync(IdType id, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false, bool getSoftDeleted = false, CancellationToken cancellationToken = default)
    {
        try
        {
            EntityType? item = default;
            ModelType? modelType = default;

            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsQueryable();
            Expression<Func<EntityType, bool>>? filter = null;

            if (trackable is false)
            {
                dbQuery = dbQuery.AsNoTracking();
            }

            if (include is not null)
            {
                dbQuery = include(dbQuery);
            }

            if (IsIEntity)
            {
                // Filter by Id
                filter = EfCoreExtensions.ByTId<EntityType, IdType>(id);
            }

            if (IsSoftDeletable && !getSoftDeleted)
            {
                // Filter by soft-delete only if getSoftDeleted is false
                var softDeleteFilter = EfCoreExtensions.IsDeleted<EntityType>(false);
                filter = filter != null ? filter.And(softDeleteFilter) : softDeleteFilter;
            }

            // Apply external 'where' if provided
            if (filter is not null)
            {
                dbQuery = dbQuery.Where(filter);
            }

            item = await dbQuery.FirstOrDefaultAsync(cancellationToken);

            if (item is not null)
            {
                modelType = _mapper.ToModel(item);
            }

            return modelType;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region GetCount
    /// <summary>
    /// Returns the number of rows that satisfy the optional predicate.
    /// </summary>
    /// <param name="where">Optional predicate used for counting.</param>
    /// <returns>Total matching row count.</returns>
    public virtual async Task<long> GetCountAsync(Expression<Func<EntityType, bool>>? where = null, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>().AsNoTracking().AsQueryable();
            if (where != null)
            {
                dbQuery = dbQuery.Where(where);
            }
            return await dbQuery.CountAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region Create
    /// <summary>
    /// Creates one or more entities and returns their mapped models.
    /// </summary>
    /// <param name="items">Entities to create.</param>
    /// <returns>Mapped models for created entities.</returns>
    public virtual async Task<List<ModelType>> CreateAsync(CancellationToken cancellationToken = default, params EntityType[] items)
    {
        using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        List<ModelType> itemsToReturen = [];
        try
        {
            foreach (EntityType item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsAuditable)
                {
                    SetPropertyValueIfExists(item, nameof(IAuditable.CreatedDate), TimeProvider.System.GetUtcNow().UtcDateTime);
                }
                _db.Entry(item).State = EntityState.Added;
                await _db.SaveChangesAsync(cancellationToken);

                var model = _mapper.ToModel(item);
                if (model is not null) itemsToReturen.Add(model);
            }
            await transaction.CommitAsync(cancellationToken);
            return itemsToReturen;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    /// <summary>
    /// Creates a single entity and returns its mapped model.
    /// </summary>
    /// <param name="item">Entity to create.</param>
    /// <returns>Mapped model for the created entity.</returns>
    public virtual async Task<ModelType> CreateAsync(EntityType item, CancellationToken cancellationToken = default)
    {
        var res = await CreateAsync(cancellationToken, [item]);
        return res.First();
    }
    #endregion

    #region Update
    /// <summary>
    /// Updates one or more entities and returns their mapped models.
    /// </summary>
    /// <param name="items">Entities to update.</param>
    /// <returns>Mapped models for updated entities.</returns>
    public virtual async Task<List<ModelType>> UpdateAsync(CancellationToken cancellationToken = default, params EntityType[] items)
    {
        using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        List<ModelType> itemsToReturen = [];
        try
        {
            foreach (EntityType item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ImplementsEntity(item))
                {
                    _db.DetachLocalObject(item, $"{GetPropertyValueIfExists(item, $"{nameof(IEntity.Id)}")}");
                }

                if (IsAuditable)
                {
                    SetPropertyValueIfExists(item, nameof(IAuditable.UpdatedDate), TimeProvider.System.GetUtcNow().UtcDateTime);
                }
                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync(cancellationToken);

                var model = _mapper.ToModel(item);
                if (model is not null) itemsToReturen.Add(model);
            }
            await transaction.CommitAsync(cancellationToken);
            return itemsToReturen;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    /// <summary>
    /// Updates a single entity and returns its mapped model.
    /// </summary>
    /// <param name="item">Entity to update.</param>
    /// <returns>Mapped model for the updated entity.</returns>
    public virtual async Task<ModelType> UpdateAsync(EntityType item, CancellationToken cancellationToken = default)
    {
        var res = await UpdateAsync(cancellationToken, [item]);
        return res.First();
    }
    #endregion

    #region Transaction
    /// <summary>
    /// Runs a custom repository action inside a database transaction.
    /// </summary>
    /// <param name="action">Callback that receives the active DbContext and performs data operations.</param>
    /// <returns>A task that completes when the transaction is committed or rolled back.</returns>
    public virtual async Task TransactionAsync(Func<DbContextType, Task>? action = null, CancellationToken cancellationToken = default)
    {
        using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (action != null)
            {
                await action(_db);
            }
            await transaction.CommitAsync(cancellationToken);
            _db.ClearChanges();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    #endregion

    #region Delete
    /// <summary>
    /// Soft-deletes entities by id, predicate, or explicit entity instances and returns mapped models.
    /// </summary>
    /// <param name="id">Optional identifier value for selecting entities.</param>
    /// <param name="where">Optional predicate for selecting entities.</param>
    /// <param name="items">Optional explicit entities to delete.</param>
    /// <returns>Mapped models for entities affected by the delete operation.</returns>
    public virtual async Task<List<ModelType>> DeleteAsync(IdType? id = default, Expression<Func<EntityType, bool>>? where = null, CancellationToken cancellationToken = default, params EntityType[] items)
    {
        using var commit = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();
            List<ModelType> itemsToReturen = [];

            #region if Items
            if (items != null && items.Length != 0)
            {
                foreach (EntityType item in items)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (IsSoftDeletable)
                    {
                        SetPropertyValueIfExists(item, nameof(ISoftDeletable.IsDeleted), true);
                    }

                    _db.Entry(item).State = EntityState.Modified;
                    await _db.SaveChangesAsync(cancellationToken);

                    var model = _mapper.ToModel(item);
                    if (model is not null) itemsToReturen.Add(model);
                }
            }
            #endregion
            #region else If Id or where
            else if (id is not null || where != null)
            {
                Expression<Func<EntityType, bool>>? expression = null;
                if (id is not null) expression = EfCoreExtensions.ByIdDynamic<EntityType>(id);
                else if (where is not null) expression = where;
                if (expression != null)
                {
                    var itemsToDelete = await dbQuery.Where(expression).ToListAsync(cancellationToken);
                    foreach (EntityType item in itemsToDelete)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (IsSoftDeletable)
                        {
                            SetPropertyValueIfExists(item, nameof(ISoftDeletable.IsDeleted), true);
                        }

                        _db.Entry(item).State = EntityState.Modified;
                        await _db.SaveChangesAsync(cancellationToken);

                        var model = _mapper.ToModel(item);
                        if (model is not null) itemsToReturen.Add(model);
                    }
                }
            }
            #endregion
            await commit.CommitAsync(cancellationToken);
            return itemsToReturen;
        }
        catch (Exception)
        {
            await commit.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Soft-deletes a single entity by identifier.
    /// </summary>
    /// <param name="itemId">Identifier of the entity to delete.</param>
    /// <returns>Mapped model for the deleted entity.</returns>
    public virtual async Task<ModelType> DeleteAsync(IdType? itemId = default, CancellationToken cancellationToken = default)
    {
        var response = await DeleteAsync(id: itemId, cancellationToken: cancellationToken);
        return response.First();
    }

    /// <summary>
    /// Soft-deletes the provided entities.
    /// </summary>
    /// <param name="itemsToDelete">Entities to delete.</param>
    /// <returns>Mapped models for entities affected by the delete operation.</returns>
    public virtual async Task<List<ModelType>> DeleteAsync(CancellationToken cancellationToken = default, params EntityType[] itemsToDelete) => await DeleteAsync(cancellationToken: cancellationToken, items: itemsToDelete);

    /// <summary>
    /// Soft-deletes entities that match a predicate.
    /// </summary>
    /// <param name="deleteCondition">Predicate used to select entities for deletion.</param>
    /// <returns>Mapped models for entities affected by the delete operation.</returns>
    public virtual async Task<List<ModelType>> DeleteAsync(Expression<Func<EntityType, bool>> deleteCondition, CancellationToken cancellationToken = default) => await DeleteAsync(where: deleteCondition, cancellationToken: cancellationToken);
    #endregion

    #region Purge
    /// <summary>
    /// Permanently deletes entities by id, predicate, or explicit entity instances.
    /// </summary>
    /// <param name="id">Optional identifier value for selecting entities.</param>
    /// <param name="where">Optional predicate for selecting entities.</param>
    /// <param name="items">Optional explicit entities to purge.</param>
    /// <returns><see langword="true"/> when purge succeeds.</returns>
    public async Task<bool> PurgeAsync(IdType? id = default, Expression<Func<EntityType, bool>>? where = null, CancellationToken cancellationToken = default, params EntityType[] items)
    {
        using var commit = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            IQueryable<EntityType> dbQuery = _db.Set<EntityType>();
            #region if Items
            if (items != null && items.Length != 0)
            {
                foreach (EntityType item in items)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (IsIEntity)
                    {
                        _db.DetachLocalObject(item, $"{GetPropertyValueIfExists(item, $"{nameof(IEntity.Id)}")}");
                    }
                    _db.Entry(item).State = EntityState.Deleted;
                    await _db.SaveChangesAsync(cancellationToken);
                }
            }
            #endregion

            #region else If Id or where
            else if (id is not null || where != null)
            {

                Expression<Func<EntityType, bool>>? expression = null;
                if (id is not null) expression = EfCoreExtensions.ByIdDynamic<EntityType>(id);
                else if (where is not null) expression = where;

                if (expression != null && items is not null)
                {
                    var itemsToDelete = await dbQuery.Where(expression).ToListAsync(cancellationToken);
                    foreach (EntityType item in itemsToDelete)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (ImplementsEntity(item))
                        {
                            _db.DetachLocalObject(item, $"{GetPropertyValueIfExists(item, $"{nameof(IEntity.Id)}")}");
                        }
                        _db.Entry(item).State = EntityState.Deleted;
                        await _db.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            #endregion
            await commit.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            await commit.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Permanently deletes a single entity by identifier.
    /// </summary>
    /// <param name="itemId">Identifier of the entity to purge.</param>
    /// <returns><see langword="true"/> when purge succeeds.</returns>
    public virtual async Task<bool> PurgeAsync(IdType? itemId = default, CancellationToken cancellationToken = default) => await PurgeAsync(id: itemId, cancellationToken: cancellationToken);

    /// <summary>
    /// Permanently deletes the provided entities.
    /// </summary>
    /// <param name="itemsToDelete">Entities to purge.</param>
    /// <returns><see langword="true"/> when purge succeeds.</returns>
    public virtual async Task<bool> PurgeAsync(CancellationToken cancellationToken = default, params EntityType[] itemsToDelete) => await PurgeAsync(cancellationToken: cancellationToken, items: itemsToDelete);

    /// <summary>
    /// Permanently deletes entities that match a predicate.
    /// </summary>
    /// <param name="deleteCondition">Predicate used to select entities for purge.</param>
    /// <returns><see langword="true"/> when purge succeeds.</returns>
    public virtual async Task<bool> PurgeAsync(Expression<Func<EntityType, bool>> deleteCondition, CancellationToken cancellationToken = default) => await PurgeAsync(where: deleteCondition, cancellationToken: cancellationToken);
    #endregion

    private static bool IsSoftDeletable => typeof(ISoftDeletable).IsAssignableFrom(typeof(EntityType));
    private static bool IsAuditable => typeof(IAuditable).IsAssignableFrom(typeof(EntityType));
    private static bool IsIEntity => typeof(IEntity).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<int>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<long>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<double>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<Guid>).IsAssignableFrom(typeof(EntityType)) ||
            typeof(IEntity<string>).IsAssignableFrom(typeof(EntityType));

    private static Type GetIdType()
    {
        if (typeof(IEntity<Guid>).IsAssignableFrom(typeof(EntityType)))
            return typeof(Guid);

        if (typeof(IEntity<int>).IsAssignableFrom(typeof(EntityType)))
            return typeof(int);

        if (typeof(IEntity<long>).IsAssignableFrom(typeof(EntityType)))
            return typeof(long);

        if (typeof(IEntity<string>).IsAssignableFrom(typeof(EntityType)))
            return typeof(string);

        throw new NotSupportedException($"Unsupported Id type for {typeof(EntityType)}");
    }

    /// <summary>
    /// Determines whether a runtime instance implements one of the supported IEntity contracts.
    /// </summary>
    /// <param name="item">Entity instance to inspect.</param>
    /// <returns><see langword="true"/> when the instance implements an IEntity variation.</returns>
    private static bool ImplementsEntity(EntityType item)
    {
        return
            item is IEntity ||
            item is IEntity<int> ||
            item is IEntity<long> ||
            item is IEntity<double> ||
            item is IEntity<Guid> ||
            item is IEntity<string>;
    }

    /// <summary>
    /// Sets a property by name when it exists on the target object.
    /// </summary>
    /// <param name="m">Target object instance.</param>
    /// <param name="propertyName">Property name to set.</param>
    /// <param name="value">Value to assign.</param>
    private static void SetPropertyValueIfExists<T>(T m, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property != null)
        {
            typeof(T).GetProperty(propertyName)?.SetValue(m, value);
        }
    }
    /// <summary>
    /// Gets a property value by name when the property exists.
    /// </summary>
    /// <param name="m">Target object instance.</param>
    /// <param name="propertyName">Property name to read.</param>
    /// <returns>Property value when found; otherwise <see langword="null"/>.</returns>
    private static object? GetPropertyValueIfExists<T>(T m, string propertyName)
    {
        return typeof(T).GetProperty(propertyName)?.GetValue(m);
    }

    /// <summary>
    /// Maps an entity sequence to a model sequence.
    /// </summary>
    /// <param name="model">Entity sequence.</param>
    /// <returns>Mapped model sequence.</returns>
    protected internal IEnumerable<ModelType>? MapToIEnumerable(IEnumerable<EntityType> model)
    {
        return model.Select(_mapper.ToModel);
    }
    /// <summary>
    /// Returns the provided object sequence unchanged.
    /// </summary>
    /// <param name="model">Object sequence.</param>
    /// <returns>Same object sequence.</returns>
    protected internal IEnumerable<object>? MapToIEnumerable(IEnumerable<object> model)
    {
        return model;
    }
    /// <summary>
    /// Maps an entity sequence to a model list.
    /// </summary>
    /// <param name="model">Entity sequence.</param>
    /// <returns>Mapped model list.</returns>
    protected internal List<ModelType>? MapToList(IEnumerable<EntityType> model)
    {
        return [.. model.Select(_mapper.ToModel)];
    }
    /// <summary>
    /// Materializes an object sequence as a list.
    /// </summary>
    /// <param name="model">Object sequence.</param>
    /// <returns>Object list.</returns>
    protected internal List<object>? MapToList(IEnumerable<object> model)
    {
        return [.. model];
    }
    /// <summary>
    /// Maps a single entity to its model representation.
    /// </summary>
    /// <param name="model">Entity instance.</param>
    /// <returns>Mapped model instance.</returns>
    protected internal ModelType? MapToModel(EntityType model)
    {
        return _mapper.ToModel(model);
    }
    /// <summary>
    /// Returns the provided object unchanged.
    /// </summary>
    /// <param name="model">Object instance.</param>
    /// <returns>Same object instance.</returns>
    protected internal object? MapToModel(object model)
    {
        return model;
    }
}
