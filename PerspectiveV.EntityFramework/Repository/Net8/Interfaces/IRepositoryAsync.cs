using PerspectiveV.EntityFramework.Repository.Interfaces;

namespace PerspectiveV.EntityFramework.Repository.Net8.Interfaces;

/// <summary>
/// This interface is deprecated and will be removed in a future release. 
/// Use <see cref="IRepositoryAsync{IdType,EntityType,ModelType,DbContextType}"/> instead.
/// </summary>
[Obsolete("This interface is deprecated and will be removed in a future release. Use IRepositoryAsync<IdType, EntityType, ModelType, DbContextType> instead.", false)]
public interface IRepositoryAsync<EntityType, ModelType, DbContextType> : IRepositoryAsync<string, EntityType, ModelType, DbContextType> { }