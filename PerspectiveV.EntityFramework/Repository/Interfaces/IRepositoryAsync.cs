using Microsoft.EntityFrameworkCore.Query;
using PerspectiveV.Common.Paging;
using System.Linq.Expressions;

namespace PerspectiveV.EntityFramework.Repository.Interfaces;


/// <summary>
/// Generic implementation of repository of type TModel for basic crud single/bulk operations.
/// And Any Db Context That Supports EntityFramework
/// </summary>
/// <typeparam name="ModelType"></typeparam>
/// <typeparam name="EntityType"></typeparam>
/// <typeparam name="DbContextType"></typeparam>
/// <summary>
public interface IRepositoryAsync<IdType, EntityType, ModelType, DbContextType>
{
    #region Get
    Task<List<ModelType>> GetAsync(
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        Expression<Func<EntityType, bool>>? where = null,
        Func<EntityType, ModelType>? select = null,
        SortOptions<EntityType, ModelType>? sortOptions = null,
        bool trackable = false,
        bool getSoftDeleted = false,
        CancellationToken cancellationToken = default);

    Task<List<TResult>> GetAsync<TResult>(
       Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
       Expression<Func<EntityType, bool>>? where = null,
       Func<EntityType, TResult>? select = null,
       SortOptions<EntityType, ModelType>? sortOptions = null,
       bool trackable = false,
       bool getSoftDeleted = false,
       CancellationToken cancellationToken = default);
    #endregion

    #region Paged
    Task<PagedResult<ModelType>> PagedAsync(
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        Expression<Func<EntityType, bool>>? where = null,
        Func<EntityType, ModelType>? select = null,
        PagingOptions? pagingOptions = null,
        SearchOptions<EntityType, ModelType>? searchOptions = null,
        SortOptions<EntityType, ModelType>? sortOptions = null,
        bool trackable = false,
        bool getSoftDeleted = false,
        CancellationToken cancellationToken = default);
    #endregion

    #region Single Or Default
    Task<ModelType?> GetSingleOrDefaultAsync(
        Expression<Func<EntityType, bool>>? where = null,
        Func<IQueryable<EntityType>,
        IIncludableQueryable<EntityType, object>>? include = null,
        bool trackable = false,
        bool getSoftDeleted = false,
        CancellationToken cancellationToken = default);
    #endregion

    #region First Or Default
    Task<ModelType?> GetFirstOrDefaultAsync(
        Expression<Func<EntityType, bool>>? where = null,
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        bool trackable = false,
        bool getSoftDeleted = false,
        CancellationToken cancellationToken = default);
    #endregion

    #region Querable
    IQueryable<EntityType> GetQueryable(
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        bool trackable = false);

    #endregion

    #region GetById
    Task<ModelType?> GetByIdAsync(IdType id, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false, bool getSoftDeleted = false, CancellationToken cancellationToken = default);
    #endregion

    #region GetCount
    Task<long> GetCountAsync(Expression<Func<EntityType, bool>>? where = null, CancellationToken cancellationToken = default);
    #endregion

    #region Create
    Task<List<ModelType>> CreateAsync(CancellationToken cancellationToken = default, params EntityType[] items);
    Task<ModelType> CreateAsync(EntityType item, CancellationToken cancellationToken = default);
    #endregion

    #region Update
    Task<List<ModelType>> UpdateAsync(CancellationToken cancellationToken = default, params EntityType[] items);
    Task<ModelType> UpdateAsync(EntityType item, CancellationToken cancellationToken = default);
    #endregion

    #region Transaction
    Task TransactionAsync(Func<DbContextType, Task>? action = null, CancellationToken cancellationToken = default);
    #endregion

    #region Delete
    Task<List<ModelType>> DeleteAsync(IdType? id = default, Expression<Func<EntityType, bool>>? where = null, CancellationToken cancellationToken = default, params EntityType[] items);

    Task<ModelType> DeleteAsync(IdType? itemId = default, CancellationToken cancellationToken = default);

    Task<List<ModelType>> DeleteAsync(Expression<Func<EntityType, bool>> deleteCondition, CancellationToken cancellationToken = default);

    Task<List<ModelType>> DeleteAsync(CancellationToken cancellationToken = default, params EntityType[] itemsToDelte);
    #endregion

    #region Purge
    Task<bool> PurgeAsync(IdType? id = default, Expression<Func<EntityType, bool>>? where = null, CancellationToken cancellationToken = default, params EntityType[] items);

    Task<bool> PurgeAsync(IdType? itemId = default, CancellationToken cancellationToken = default);

    Task<bool> PurgeAsync(CancellationToken cancellationToken = default, params EntityType[] itemsToDelte);

    Task<bool> PurgeAsync(Expression<Func<EntityType, bool>> deleteCondition, CancellationToken cancellationToken = default);
    #endregion

}
