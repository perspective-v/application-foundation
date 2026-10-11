using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerspectiveV.Common.AppConfigurations;
using PerspectiveV.Common.Extensions;
using PerspectiveV.Common.Mapper;
using PerspectiveV.Common.Paging;
using PerspectiveV.EntityFramework.Repository.Net6.Interfaces;
using PerspectiveV.Primitives.Types.Entities;
using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Web.Rest.Controllers.Net6;
/// <summary>
/// Base REST controller for the Net6 stack that provides list, paged, get-by-id,
/// create, update, and soft-delete endpoints for an entity/model pair.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Inherit and bind your concrete types.
/// public class UsersController
///     : BaseControllerAsync<User, UserDto, AppDbContext, IAppUnitOfWork>
/// {
///     public UsersController(IAppUnitOfWork uow, IProjectionMapper<User, UserDto> mapper)
///         : base(uow, mapper)
///     {
///     }
/// }
/// ]]></code>
/// </example>
public class BaseControllerAsync<TEntity, TModel, TDBContext, TIUnitOfWork> : ControllerBase {
    private readonly TIUnitOfWork? _uow;
    private readonly IRepositoryAsync<TEntity, TDBContext> _service;
    private readonly IProjectionMapper<string, TEntity, TModel> _mapper;
    private readonly IWebHostEnvironment? _env;
    private readonly PagingOptions? _defaultPagingOptions;
    private readonly AppSettings? _appSettings;
    /// <summary>
    /// This will return ContentRootPath if virtual directory option is enabled otherwise WebRootPath
    /// </summary>
    protected internal string? ContentOrWebRootPath { get; private set; }
    /// <summary>
    /// Initializes a new instance of the BaseControllerAsync class.
    /// </summary>
    /// <param name="uow">Unit-of-work used to resolve the V2 repository service for <typeparamref name="TEntity"/>.</param>
    /// <param name="mapper">Projection mapper between entity and API model.</param>
    /// <param name="defaultPagingOptions">Default paging values applied when request values are omitted.</param>
    /// <param name="appSettingOptions">Application settings used for path resolution behavior.</param>
    /// <param name="env">Web host environment used to resolve content and web root paths.</param>
    public BaseControllerAsync(TIUnitOfWork uow, IProjectionMapper<string, TEntity, TModel> mapper, IOptions<PagingOptions>? defaultPagingOptions = null, IOptions<AppSettings>? appSettingOptions = null, IWebHostEnvironment? env = null) {
        _uow = uow;
        _mapper = mapper;
        if (env is not null) _env = env;

        _service = (IRepositoryAsync<TEntity, TDBContext>)typeof(TIUnitOfWork).GetProperty($"{typeof(TEntity).Name}Service")?.GetValue(_uow)!;

        if (appSettingOptions is not null) _appSettings = appSettingOptions.Value;

        if (defaultPagingOptions is not null) _defaultPagingOptions = defaultPagingOptions.Value;

        if (_appSettings is not null && _env is not null) {
            ContentOrWebRootPath = _appSettings.VirtualDirectory?.IsEnabled == true
                ? Path.Combine(_env.ContentRootPath, $"..\\{_appSettings.VirtualDirectory?.Name}")
                : _env.WebRootPath;
        }
    }

    /// <summary>
    /// Initializes a new instance of the BaseControllerAsync class.
    /// </summary>
    /// <param name="service">Repository service instance used directly by the controller.</param>
    /// <param name="mapper">Projection mapper between entity and API model.</param>
    /// <param name="defaultPagingOptions">Default paging values applied when request values are omitted.</param>
    public BaseControllerAsync(IRepositoryAsync<TEntity, TDBContext> service, IProjectionMapper<string, TEntity, TModel> mapper, IOptions<PagingOptions>? defaultPagingOptions = null) {
        _service = service;
        _mapper = mapper;
        if (defaultPagingOptions != null) _defaultPagingOptions = defaultPagingOptions.Value;

    }

    /// <summary>
    /// Returns all entities from the repository.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the repository result set.</returns>
    [HttpGet]
    public virtual async Task<IActionResult> Get() {
        var res = await _service.GetAsync();
        return Ok(res);
    }

    /// <summary>
    /// Returns a paged result using requested paging values.
    /// </summary>
    /// <param name="pagingOptions">Offset and limit values from the query string.</param>
    /// <returns>An <see cref="IActionResult"/> containing a paged payload.</returns>
    [HttpGet("[action]")]
    public virtual async Task<IActionResult> Paged(
        [FromQuery] PagingOptions pagingOptions
        ) {
        pagingOptions.Offset ??= _defaultPagingOptions?.Offset;
        pagingOptions.Limit ??= _defaultPagingOptions?.Limit;
        var pagedData = await _service.PagedAsync(
            pagingOptions: pagingOptions);
        var res = MapToPagedResult(pagedData);
        return Ok(res);
    }

    /// <summary>
    /// Returns one entity by identifier.
    /// </summary>
    /// <param name="id">Identifier value to locate.</param>
    /// <returns>An <see cref="IActionResult"/> with the mapped model when found.</returns>
    [HttpGet("{id}")]
    public virtual async Task<IActionResult> Get(string id) {
        if (string.IsNullOrEmpty(id)) return NotFound(id);

        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound(id);

        return Ok(MapToModel(item));
    }

    /// <summary>
    /// Creates a new entity from the supplied model.
    /// </summary>
    /// <param name="model">Incoming model payload.</param>
    /// <returns>An <see cref="IActionResult"/> with created status and persisted data.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> Post(TModel model) {
        var m = _mapper.ToEntity(model);
        if (m == null) return NotFound(model);
        if (model is IAuditable) {
            SetPropertyValueIfExists(m, nameof(AuditableEntity.CreatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "System");
        }
        await _service.CreateAsync(m);
        return StatusCode(StatusCodes.Status201Created, m);
    }

    /// <summary>
    /// Updates an existing entity from the supplied model.
    /// </summary>
    /// <param name="model">Incoming model payload.</param>
    /// <returns>An <see cref="IActionResult"/> with the updated data.</returns>
    [HttpPut]
    public virtual async Task<IActionResult> Put(TModel model) {
        if (model is null) return NotFound(model);
        var m = _mapper.ToEntity(model);
        if (m is null) return NotFound(model);

        if (model is IAuditable) {
            SetPropertyValueIfExists(m, nameof(AuditableEntity.UpdatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "System");
        }

        await _service.UpdateAsync(m);
        return StatusCode(StatusCodes.Status200OK, m);
    }

    /// <summary>
    /// Soft-deletes an entity by identifier.
    /// </summary>
    /// <param name="id">Identifier of the entity to delete.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the delete operation result.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(string id) {
        if (id == null) return NotFound(id);
        await _service.SoftDeleteAsync(id);
        return StatusCode(StatusCodes.Status200OK, id);
    }

    /// <summary>
    /// Maps an entity-based paged result into a model-based paged result.
    /// </summary>
    /// <param name="model">Entity paged result.</param>
    /// <returns>Mapped model paged result.</returns>
    protected internal PagedResult<TModel> MapToPagedResult(PagedResult<TEntity>? model) {
        var response = new PagedResult<TModel>();
        if (model is not null && model?.ItemsCount > 0) {
            var items = model?.Items?.Select(_mapper.ToModel);
            response = new PagedResult<TModel> {
                Items = items,
                Limit = model?.Limit,
                Offset = model?.Offset,
                ItemsCount = model!.ItemsCount,
                TotalItemsCount = model.TotalItemsCount,
            };
        }
        return response;
    }

    /// <summary>
    /// Normalizes an object-based paged result.
    /// </summary>
    /// <param name="model">Object paged result.</param>
    /// <returns>Normalized object paged result.</returns>
    protected internal PagedResult<object> MapToPagedResult(PagedResult<object>? model) {
        if (model is null) {
            return new PagedResult<object>(); // Return an empty response if model is null
        }

        var items = model.Items ?? [];

        return new PagedResult<object> {
            Items = [.. items],
            Limit = model.Limit,
            Offset = model.Offset,
            ItemsCount = model.ItemsCount,
            TotalItemsCount = model.TotalItemsCount,
        };
    }

    /// <summary>
    /// Maps an entity sequence to a model sequence.
    /// </summary>
    /// <param name="model">Entity sequence.</param>
    /// <returns>Mapped model sequence.</returns>
    protected internal IEnumerable<TModel>? MapToIEnumerable(IEnumerable<TEntity> model) {
        return model.Select(_mapper.ToModel);
    }
    /// <summary>
    /// Returns the provided object sequence unchanged.
    /// </summary>
    /// <param name="model">Object sequence.</param>
    /// <returns>Same object sequence.</returns>
    protected internal IEnumerable<object>? MapToIEnumerable(IEnumerable<object> model) {
        return model;
    }
    /// <summary>
    /// Maps an entity sequence to a model list.
    /// </summary>
    /// <param name="model">Entity sequence.</param>
    /// <returns>Mapped model list.</returns>
    protected internal List<TModel>? MapToList(IEnumerable<TEntity> model) {
        return [.. model.Select(_mapper.ToModel)];
    }
    /// <summary>
    /// Materializes an object sequence as a list.
    /// </summary>
    /// <param name="model">Object sequence.</param>
    /// <returns>Object list.</returns>
    protected internal List<object>? MapToList(IEnumerable<object> model) {
        return [.. model];
    }
    /// <summary>
    /// Maps one entity instance to its model representation.
    /// </summary>
    /// <param name="model">Entity instance.</param>
    /// <returns>Mapped model instance.</returns>
    protected internal TModel? MapToModel(TEntity model) {
        return _mapper.ToModel(model);
    }
    /// <summary>
    /// Returns the provided object unchanged.
    /// </summary>
    /// <param name="model">Object instance.</param>
    /// <returns>Same object instance.</returns>
    protected internal object? MapToModel(object model) {
        return model;
    }

    /// <summary>
    /// Sets a property by name when it exists on the target object.
    /// </summary>
    /// <param name="m">Target object instance.</param>
    /// <param name="propertyName">Property name to set.</param>
    /// <param name="value">Value to assign.</param>
    protected internal void SetPropertyValueIfExists<T>(T m, string propertyName, object value) {
        var property = typeof(T).GetProperty(propertyName);
        if (property != null) {
            typeof(T).GetProperty(propertyName)?.SetValue(m, value);
        }
    }
}
