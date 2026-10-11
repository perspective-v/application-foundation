using Microsoft.EntityFrameworkCore.Query;
using PerspectiveV.Common.Paging;
using PerspectiveV.Primitives.Types.Models;
using System.Linq.Expressions;

namespace PerspectiveV.EntityFramework.Repository.Net6.Interfaces;
/// <summary>
/// Generic implementation of repository of type TModel for basic crud single/bulk operations.
/// And Any Db Context That Supports EntityFramework
/// </summary>
/// <typeparam name="EntityType"></typeparam>
/// <typeparam name="DbContextType"></typeparam>
[Obsolete("This class is deprecated and will be removed in future versions. Please use the updated repository implementation instead.", false)]
public interface IRepositoryAsync<EntityType, DbContextType>
{
    #region Get
    Task<List<object>?> GetAsync(
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        Expression<Func<EntityType, bool>>? where = null,
        Func<EntityType, object>? select = null,
        Expression<Func<EntityType, object>>? orderBy = null,
        OrderDirection orderDirection = OrderDirection.Ascending,
        bool trackable = false,
        bool getSoftDeleted = false);

    Task<List<TResult>?> GetAsync<TResult>(
       Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
       Expression<Func<EntityType, bool>>? where = null,
       Func<EntityType, object>? select = null,
       Expression<Func<EntityType, object>>? orderBy = null,
       OrderDirection orderDirection = OrderDirection.Ascending,
       bool trackable = false,
       bool getSoftDeleted = false);
    #endregion

    #region Paged
    Task<PagedResult<object>?> PagedAsync(
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        Expression<Func<EntityType, bool>>? where = null,
        Func<EntityType, object>? select = null,
        PagingOptions? pagingOptions = null,
        Expression<Func<EntityType, object>>? orderBy = null,
        OrderDirection orderDirection = OrderDirection.Ascending,
        bool trackable = false,
        bool getSoftDeleted = false);
    Task<PagedResult<TResult>?> PagedAsync<TResult>(
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        Expression<Func<EntityType, bool>>? where = null,
        Func<EntityType, object>? select = null,
        PagingOptions? pagingOptions = null,
        Expression<Func<EntityType, object>>? orderBy = null,
        OrderDirection orderDirection = OrderDirection.Ascending,
        bool trackable = false,
        bool getSoftDeleted = false);
    #endregion

    #region Single Or Default
    Task<EntityType?> GetSingleOrDefaultAsync(Expression<Func<EntityType, bool>>? where = null, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false);
    #endregion

    #region First Or Default
    Task<EntityType?> GetFirstOrDefaultAsync(
        Expression<Func<EntityType, bool>>? where = null,
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        bool trackable = false);
    #endregion

    #region Querable
    IQueryable<EntityType> GetQueryable(
        Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null,
        bool trackable = false);

    #endregion

    #region GetById
    Task<EntityType?> GetByIdAsync(string id);
    #endregion

    #region GetCount
    Task<long> GetCountAsync(Expression<Func<EntityType, bool>>? where = null);
    #endregion

    #region Create
    Task CreateAsync(params EntityType[] items);
    #endregion

    #region Update
    Task UpdateAsync(params EntityType[] items);
    #endregion

    #region Transaction
    Task TransactionAsync(Func<DbContextType, Task>? action = null);
    #endregion

    #region SoftDelete
    Task SoftDeleteAsync(string? id = null, Expression<Func<EntityType, bool>>? where = null, params EntityType[] items);

    Task SoftDeleteAsync(string itemId);

    Task SoftDeleteAsync(Expression<Func<EntityType, bool>> deleteCondition);

    Task SoftDeleteAsync(params EntityType[] itemsToDelte);
    #endregion

    #region Delete
    Task DeleteAsync(string? id = null, Expression<Func<EntityType, bool>>? where = null, params EntityType[] items);

    Task DeleteAsync(string itemId);

    Task DeleteAsync(params EntityType[] itemsToDelte);

    Task DeleteAsync(Expression<Func<EntityType, bool>> deleteCondition);
    #endregion

}
