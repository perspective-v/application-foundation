using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PerspectiveV.Common.Mapper;
using PerspectiveV.EntityFramework.Repository.Implementation;
using PerspectiveV.EntityFramework.Repository.Net8.Interfaces;
using System.Linq.Expressions;

namespace PerspectiveV.EntityFramework.Repository.Net8.Implementation;

/// <summary>
/// This class is deprecated and will be removed in a future release. 
/// Use <see cref="RepositoryAsync{IdType,EntityType,ModelType,DbContextType}"/> instead.
/// </summary>
[Obsolete("This class is deprecated and will be removed in a future release. Use RepositoryAsync<IdType, EntityType, ModelType, DbContextType> instead.", false)]
public abstract class RepositoryAsync<EntityType, ModelType, DbContextType>(DbContextType db, IProjectionMapper<string,EntityType, ModelType> mapper) : RepositoryAsync<string, EntityType, ModelType, DbContext>(db, mapper), IRepositoryAsync<EntityType, ModelType, DbContextType>
    where ModelType : class
    where EntityType : class
    where DbContextType : DbContext
{

    private readonly DbContextType _db = db;
    private readonly IProjectionMapper<string, EntityType, ModelType> _mapper = mapper;

    public async Task<ModelType?> GetByIdAsync(string id, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false, CancellationToken cancellationToken = default)
    {
        return await base.GetByIdAsync(id, include, trackable, false, cancellationToken);
    }

    public async Task<ModelType?> GetFirstOrDefaultAsync(Expression<Func<EntityType, bool>>? where = null, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false, CancellationToken cancellationToken = default)
    {
        return await base.GetFirstOrDefaultAsync(where, include, trackable, false, cancellationToken);
    }

    public async Task<ModelType?> GetSingleOrDefaultAsync(Expression<Func<EntityType, bool>>? where = null, Func<IQueryable<EntityType>, IIncludableQueryable<EntityType, object>>? include = null, bool trackable = false, CancellationToken cancellationToken = default)
    {
        return await base.GetSingleOrDefaultAsync(where, include, trackable, false, cancellationToken);
    }

    public async Task TransactionAsync(Func<DbContextType, Task>? action = null, CancellationToken cancellationToken = default)
    {
        await base.TransactionAsync(async (db) => { if (action != null) await action(_db); }, cancellationToken);
    }
}
