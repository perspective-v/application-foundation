using PerspectiveV.Primitives.Types.Models;
using System.Linq.Expressions;

namespace PerspectiveV.Common.Mapper;

/// <summary>
/// Defines the entity/model mapping contract used by repository and web abstractions.
/// <para>
/// This contract provides a stable adapter over Mapperly-generated methods so consumers can perform
/// in-memory mapping, query projection, and in-place updates using a single dependency.
/// </para>
/// <para>
/// Usage example:
/// <code><![CDATA[
/// public sealed class UserService
/// {
///     private readonly IProjectionMapper<UserEntity, UserModel> _mapper;
///
///     public UserService(IProjectionMapper<UserEntity, UserModel> mapper)
///     {
///         _mapper = mapper;
///     }
///
///     public UserModel? Convert(UserEntity? entity)
///     {
///         return entity is null ? null : _mapper.ToModel(entity);
///     }
/// }
/// ]]></code>
/// </para>
/// <para>
/// Mandatory Mapperly members for wrapper-free direct usage:
/// </para>
/// <list type="number">
/// <item>
/// <description>
/// In-memory methods:
/// <code><![CDATA[
/// public partial UserModel ToModel(UserEntity entity);
/// public partial UserEntity ToEntity(UserModel model);
/// ]]></code>
/// </description>
/// </item>
/// <item>
/// <description>
/// Queryable projection expressions:
/// <code><![CDATA[
/// public static partial Expression<Func<UserEntity, UserModel>> ToModelExpression();
/// public static partial Expression<Func<UserModel, UserEntity>> ToEntityExpression();
/// ]]></code>
/// </description>
/// </item>
/// </list>
/// <para>
/// Without this wrapper, use the mapper directly:
/// <code><![CDATA[
/// var mapper = new UserMapper();
/// var model = mapper.ToModel(entity);
///
/// var projected = dbContext.Users
///     .Select(UserMapper.ToModelExpression())
///     .ToList();
/// ]]></code>
/// </para>
/// </summary>
/// <typeparam name="TEntity">Entity type used by persistence and domain operations.</typeparam>
/// <typeparam name="TModel">Model type used by API/UI layers.</typeparam>
public interface IProjectionMapper<IdType,TEntity, TModel>
{
    // =========================================================
    // BASIC MAPPING
    // =========================================================

    /// <summary>
    /// Maps a single entity instance to a model instance.
    /// </summary>
    /// <example><![CDATA[
    /// UserModel model = mapper.ToModel(entity);
    /// ]]></example>
    TModel ToModel(TEntity source);

    /// <summary>
    /// Maps a single model instance back to an entity instance.
    /// </summary>
    /// <example><![CDATA[
    /// UserEntity entity = mapper.ToEntity(model);
    /// ]]></example>
    TEntity ToEntity(TModel destination);

    // =========================================================
    // COLLECTION MAPPING
    // =========================================================

    /// <summary>
    /// Maps a collection of entities to models.
    /// This executes in-memory using LINQ Select.
    /// </summary>
    /// <example><![CDATA[
    /// IEnumerable<UserModel> models = mapper.ToModelList(entities);
    /// ]]></example>
    IEnumerable<TModel> ToModelList(IEnumerable<TEntity> source);

    /// <summary>
    /// Maps a collection of models back to entities.
    /// </summary>
    /// <example><![CDATA[
    /// IEnumerable<UserEntity> entities = mapper.ToEntityList(models);
    /// ]]></example>
    IEnumerable<TEntity> ToEntityList(IEnumerable<TModel> destination);

    // =========================================================
    // EF CORE QUERY PROJECTION
    // =========================================================

    /// <summary>
    /// Projects an <see cref="IQueryable{T}"/> of entities into models.
    /// This should be translated to SQL by EF Core when using expression-based implementations.
    /// </summary>
    /// <example><![CDATA[
    /// IQueryable<UserModel> query = mapper.ProjectToModel(dbContext.Users);
    /// ]]></example>
    IQueryable<TModel> ProjectToModel(IQueryable<TEntity> query);

    /// <summary>
    /// Projects an <see cref="IQueryable{T}"/> of models into entities.
    /// </summary>
    IQueryable<TEntity> ProjectToEntity(IQueryable<TModel> query);

    // =========================================================
    // ASYNC QUERY EXECUTION HELPERS
    // =========================================================

    /// <summary>
    /// Projects an entity query and materializes the model list using in-memory enumeration.
    /// Prefer provider async APIs: <c>ProjectToModel(query).ToListAsync(cancellationToken)</c>.
    /// </summary>
    /// <example><![CDATA[
    /// List<UserModel> users = await mapper.ProjectToModelAsync(dbContext.Users, cancellationToken);
    /// ]]></example>
    [Obsolete("Use ProjectToModel(query).ToListAsync(cancellationToken) to use the provider async pipeline.")]
    Task<List<TModel>> ProjectToModelAsync(
        IQueryable<TEntity> query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects a model query and materializes the entity list using in-memory enumeration.
    /// Prefer provider async APIs: <c>ProjectToEntity(query).ToListAsync(cancellationToken)</c>.
    /// </summary>
    [Obsolete("Use ProjectToEntity(query).ToListAsync(cancellationToken) to use the provider async pipeline.")]
    Task<List<TEntity>> ProjectToEntityAsync(
        IQueryable<TModel> query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects an entity query and returns the first projected model, if any.
    /// Prefer provider async APIs: <c>ProjectToModel(query).FirstOrDefaultAsync(cancellationToken)</c>.
    /// </summary>
    /// <example><![CDATA[
    /// UserModel? user = await mapper.FirstOrDefaultProjectedModelAsync(
    ///     dbContext.Users.Where(x => x.Id == id),
    ///     cancellationToken);
    /// ]]></example>
    [Obsolete("Use ProjectToModel(query).FirstOrDefaultAsync(cancellationToken) to use the provider async pipeline.")]
    Task<TModel?> FirstOrDefaultProjectedModelAsync(
        IQueryable<TEntity> query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects a model query and returns the first projected entity, if any.
    /// Prefer provider async APIs: <c>ProjectToEntity(query).FirstOrDefaultAsync(cancellationToken)</c>.
    /// </summary>
    [Obsolete("Use ProjectToEntity(query).FirstOrDefaultAsync(cancellationToken) to use the provider async pipeline.")]
    Task<TEntity?> FirstOrDefaultProjectedEntityAsync(
        IQueryable<TModel> query,
        CancellationToken cancellationToken = default);

    // =========================================================
    // EXPRESSION-BASED MAPPING (KEY FOR EF CORE + MAPPERLY STYLE)
    // =========================================================

    /// <summary>
    /// Returns an expression tree for entity-to-model projection.
    /// This enables EF Core translation and reusable LINQ projections.
    /// </summary>
    /// <example><![CDATA[
    /// Expression<Func<UserEntity, UserModel>> expr = mapper.ToModelExpression();
    /// IQueryable<UserModel> query = dbContext.Users.Select(expr);
    /// ]]></example>
    Expression<Func<TEntity, TModel>> ToModelExpression();

    /// <summary>
    /// Returns an expression tree for model-to-entity projection.
    /// </summary>
    Expression<Func<TModel, TEntity>> ToEntityExpression();

    Expression<Func<TEntity, NameValuePair<IdType>>> ToNameValueExpression();

    // =========================================================
    // IN-PLACE MAPPING (EF TRACKING SUPPORT)
    // =========================================================

    /// <summary>
    /// Maps values from an entity into an existing model instance.
    /// Useful for updating API/UI model instances while preserving object references.
    /// </summary>
    /// <example><![CDATA[
    /// mapper.UpdateModel(entity, model);
    /// ]]></example>
    void UpdateModel(TEntity source, TModel destination);

    /// <summary>
    /// Maps values from a model into an existing entity instance.
    /// Useful for updating EF tracked entities.
    /// </summary>
    /// <example><![CDATA[
    /// mapper.UpdateEntity(model, entity);
    /// await dbContext.SaveChangesAsync(cancellationToken);
    /// ]]></example>
    void UpdateEntity(TModel source, TEntity destination);

}

