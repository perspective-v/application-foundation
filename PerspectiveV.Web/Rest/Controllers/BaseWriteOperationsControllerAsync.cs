using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerspectiveV.Common.AppConfigurations;
using PerspectiveV.Common.Extensions;
using PerspectiveV.Common.Mapper;
using PerspectiveV.Common.Paging;
using PerspectiveV.EntityFramework.Repository.Interfaces;
using PerspectiveV.Primitives.Types.Entities;
using PerspectiveV.Primitives.Types.Interfaces;
using PerspectiveV.Web.S3Storage.Extensions;
using PerspectiveV.Web.S3Storage.Interface;

namespace PerspectiveV.Web.Rest.Controllers.Net8;
/// <summary>
/// Base REST controller that exposes write endpoints (create, update, delete, and purge)
/// for an entity/model pair.
/// </summary>
/// <example>
/// <code>
/// <![CDATA[
/// // Inherit and bind your concrete types.
/// public class UsersWriteController
///     : BaseWriteOperationsControllerAsync<User, UserDto, AppDbContext, IAppUnitOfWork>
/// {
///     public UsersWriteController(IAppUnitOfWork uow, IProjectionMapper<User, UserDto> mapper)
///         : base(uow, mapper)
///     {
///     }
/// }
/// ]]>
/// </code>
/// </example>
public abstract class BaseWriteOperationsControllerAsync<IdType, EntityType, ModelType, DBContextType, IUnitOfWorkType> : BaseControllerAsync<DBContextType, IUnitOfWorkType>
{
    protected readonly IRepositoryAsync<IdType, EntityType, ModelType, DBContextType> _service;
    protected readonly IProjectionMapper<string, EntityType, ModelType> _mapper;
    protected readonly IS3StorageService? _s3StorageService;

    /// <summary>
    /// Initializes a new instance of the BaseWriteOperationsControllerAsync class.
    /// </summary>
    /// <param name="uow">Unit-of-work used to resolve the repository service for <typeparamref name="EntityType"/>.</param>
    /// <param name="mapper">Projection mapper between entity and API model.</param>
    /// <param name="defaultPagingOptions">Default paging values applied by shared base behavior.</param>
    /// <param name="appSettingOptions">Application settings used for virtual-directory and media handling logic.</param>
    /// <param name="env">Web host environment used to resolve physical storage paths.</param>
    /// <param name="mediaSaveOptions">Optional media upload behavior for create/update endpoints.</param>
    public BaseWriteOperationsControllerAsync(IUnitOfWorkType uow, IProjectionMapper<string, EntityType, ModelType> mapper, IOptions<PagingOptions>? defaultPagingOptions = null, IOptions<AppSettings>? appSettingOptions = null, IWebHostEnvironment? env = null, MediaSaveOptions? mediaSaveOptions = null, IS3StorageService? s3StorageService = null)
        : base(uow, defaultPagingOptions, appSettingOptions, env)
    {
        _mapper = mapper;
        _s3StorageService = s3StorageService;

        _service = (IRepositoryAsync<IdType, EntityType, ModelType, DBContextType>)typeof(IUnitOfWorkType).GetProperty($"{typeof(EntityType).Name}Service")?.GetValue(_uow)!;
        _ = mediaSaveOptions ?? new MediaSaveOptions();
    }

    /// <summary>
    /// Creates a new entity from the supplied model.
    /// </summary>
    /// <param name="model">Incoming model payload.</param>
    /// <returns>An <see cref="IActionResult"/> with created status and the persisted entity payload.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> Post(ModelType model, CancellationToken cancellationToken = default)
    {
        if (model is null) return BadRequest(model);

        var entity = _mapper.ToEntity(model);
        if (entity == null) return BadRequest(model);

        if (IsAuditable)
        {
            SetPropertyValueIfExists(entity, nameof(AuditableEntity.CreatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "Missing Account ID");
        }

        if (HasValidFileUpload && MediaSaveOptions is not null)
        {
            IFormFileCollection? files = HttpContext?.Request?.Form?.Files;

            if (_appSettings?.S3Storage?.IsEnabled == true)
            {
                await PostFilesS3StorageAsync(entity, files, cancellationToken);
            }
            else
            {
                await PostFilesLocalStorageAsync(entity, files, cancellationToken);
            }
        }

        await _service.CreateAsync(entity, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, entity);
    }

    /// <summary>
    /// Updates an existing entity from the supplied model.
    /// </summary>
    /// <param name="model">Incoming model payload.</param>
    /// <returns>An <see cref="IActionResult"/> with the updated entity payload.</returns>
    [HttpPut]
    public virtual async Task<IActionResult> Put(ModelType model, CancellationToken cancellationToken = default)
    {
        if (model is null) return BadRequest(model);

        var entity = _mapper.ToEntity(model);
        if (entity is null) return BadRequest(model);

        if (IsAuditable)
        {
            SetPropertyValueIfExists(entity, nameof(AuditableEntity.UpdatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "Missing Account ID");
        }

        if (HasValidFileUpload && MediaSaveOptions is not null)
        {
            IFormFileCollection? files = HttpContext?.Request?.Form?.Files;

            if (_appSettings?.S3Storage?.IsEnabled == true)
            {
                await PutFilesS3StorageAsync(entity, model, files, cancellationToken);
            }
            else
            {
                await PutFilesLocalStorageAsync(entity, model, files, cancellationToken);
            }
        }

        await _service.UpdateAsync(entity, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, entity);
    }

    /// <summary>
    /// Soft-deletes an entity by identifier when supported by the entity type.
    /// </summary>
    /// <param name="id">Identifier of the entity to delete.</param>
    /// <returns>An <see cref="IActionResult"/> containing repository delete output.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(IdType id, CancellationToken cancellationToken = default)
    {
        if (id == null || id.Equals(default(IdType))) return NotFound(id);
        var result = await _service.DeleteAsync(id, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, result);
    }

    /// <summary>
    /// Permanently deletes an entity by identifier.
    /// </summary>
    /// <param name="id">Identifier of the entity to remove.</param>
    /// <returns>An <see cref="IActionResult"/> indicating purge success/failure.</returns>
    [HttpDelete("{id}/[action]")]
    public virtual async Task<IActionResult> Purge(IdType id, CancellationToken cancellationToken = default)
    {
        if (id == null || id.Equals(default(IdType))) return NotFound(id);
        var result = await _service.PurgeAsync(id, cancellationToken);
        return StatusCode(StatusCodes.Status200OK, result);
    }

    #region Storage Methods
    protected async Task PostFilesS3StorageAsync(EntityType entity, IFormFileCollection? files, CancellationToken cancellationToken = default)
    {
        if (files is null || files.Count == 0)
        {
            return;
        }

        var mediaSaveOptions = MediaSaveOptions ?? throw new InvalidOperationException("Media save options are not configured.");
        var s3StorageService = _s3StorageService ?? throw new InvalidOperationException("S3 storage service is not configured.");

        var filePropertyName = mediaSaveOptions.PropertyName ?? throw new InvalidOperationException("Media save option property name cannot be null.");
        var fileFolderPath = mediaSaveOptions.FolderPath ?? throw new InvalidOperationException("Media save option folder path cannot be null.");

        if (mediaSaveOptions.HasImageCollection == true)
        {
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveToS3StorageGetKeys(s3StorageService, fileFolderPath, cancellationToken));
        }
        else
        {
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveToS3StorageGetKey(s3StorageService, fileFolderPath, cancellationToken));
        }
    }

    protected async Task PutFilesS3StorageAsync(EntityType entity, ModelType model, IFormFileCollection? files, CancellationToken cancellationToken = default)
    {
        if (files is null || files.Count == 0)
        {
            return;
        }

        var mediaSaveOptions = MediaSaveOptions ?? throw new InvalidOperationException("Media save options are not configured.");
        var s3StorageService = _s3StorageService ?? throw new InvalidOperationException("S3 storage service is not configured.");

        var filePropertyName = mediaSaveOptions.PropertyName ?? throw new InvalidOperationException("Media save option property name cannot be null.");
        var fileFolderPath = mediaSaveOptions.FolderPath ?? throw new InvalidOperationException("Media save option folder path cannot be null.");

        if (mediaSaveOptions.HasImageCollection == true)
        {
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveToS3StorageGetKeys(s3StorageService, fileFolderPath, cancellationToken));
        }
        else
        {
            // Retrieve and delete existing file if necessary
            var fileKey = GetPropertyValueIfExists<ModelType, string>(model, filePropertyName);
            if (!string.IsNullOrEmpty(fileKey))
            {
                await files.DeleteFileFromS3Storage(s3StorageService, fileKey, cancellationToken);
            }

            // Save new file and update model property
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveToS3StorageGetKey(s3StorageService, fileFolderPath, cancellationToken));
        }
    }

    protected async Task PostFilesLocalStorageAsync(EntityType entity, IFormFileCollection? files, CancellationToken cancellationToken = default)
    {
        if (files is null || files.Count == 0)
        {
            return;
        }

        var mediaSaveOptions = MediaSaveOptions ?? throw new InvalidOperationException("Media save options are not configured.");

        if (_appSettings?.VirtualDirectory?.IsEnabled == true)
        {
            var virtualDirectoryName = _appSettings?.VirtualDirectory?.Name ?? throw new InvalidOperationException("Virtual directory name cannot be null.");
        }
        var webRootPath = ContentOrWebRootPath ?? throw new InvalidOperationException("Content/Web root path cannot be null.");
        var filePropertyName = mediaSaveOptions.PropertyName ?? throw new InvalidOperationException("Media save option property name cannot be null.");
        var fileFolderPath = mediaSaveOptions.FolderPath ?? throw new InvalidOperationException("Media save option folder path cannot be null.");

        if (mediaSaveOptions.HasImageCollection == true)
        {
            // don't need to delete old files here
            // Save multiple files in single request and update model property
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveFilesGetServerPathsAsync(fileFolderPath, webRootPath, cancellationToken));
        }
        else
        {
            // Save new file and update model property
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveFileGetServerPathAsync(fileFolderPath, webRootPath, cancellationToken));
        }
    }

    protected async Task PutFilesLocalStorageAsync(EntityType entity, ModelType model, IFormFileCollection? files, CancellationToken cancellationToken = default)
    {
        if (files is null || files.Count == 0)
        {
            return;
        }

        var mediaSaveOptions = MediaSaveOptions ?? throw new InvalidOperationException("Media save options are not configured.");

        var webRootPath = ContentOrWebRootPath ?? throw new InvalidOperationException("Content/Web root path cannot be null.");

        var filePropertyName = mediaSaveOptions.PropertyName ?? throw new InvalidOperationException("Media save option property name cannot be null.");
        var fileFolderPath = mediaSaveOptions.FolderPath ?? throw new InvalidOperationException("Media save option folder path cannot be null.");

        var virtualDirectoryName = string.Empty;
        if (_appSettings?.VirtualDirectory?.IsEnabled == true)
        {
            virtualDirectoryName = _appSettings?.VirtualDirectory?.Name ?? throw new InvalidOperationException("Virtual directory name cannot be null.");
        }

        if (mediaSaveOptions.HasImageCollection == true)
        {
            // don't need to delete old files here
            // Save multiple files in single request and update model property
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveFilesGetServerPathsAsync(fileFolderPath, webRootPath, cancellationToken));
        }
        else
        {
            // Retrieve and delete existing file if necessary
            var existingFilePath = GetPropertyValueIfExists<ModelType, string>(model, filePropertyName);
            if (!string.IsNullOrEmpty(existingFilePath))
            {
                await files.DeleteFileFromServerAsync(Request.CheckAndGetDynamicUri(existingFilePath, virtualDirectoryName), webRootPath, cancellationToken);
            }

            // Save new file and update model property
            SetPropertyValueIfExists(entity, filePropertyName, await files.SaveFileGetServerPathAsync(fileFolderPath, webRootPath, cancellationToken));
        }
    }
    #endregion Storage Methods

    /// <summary>
    /// Maps an entity-based paged response into a model-based paged response.
    /// </summary>
    /// <param name="model">Entity paged response.</param>
    /// <returns>Mapped model paged response.</returns>
    protected internal PagedResult<ModelType> MapToPagedResult(PagedResult<EntityType>? model)
    {
        var response = new PagedResult<ModelType>();
        if (model is not null && model?.ItemsCount > 0)
        {
            var items = model?.Items?.Select(_mapper.ToModel);
            response = new PagedResult<ModelType>
            {
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
    /// Normalizes an object-based paged response.
    /// </summary>
    /// <param name="model">Source object paged response.</param>
    /// <returns>Normalized object paged response.</returns>
    protected internal PagedResult<object> MapToPagedResult(PagedResult<object>? model)
    {
        if (model is null)
        {
            return new PagedResult<object>(); // Return an empty response if model is null
        }

        var items = model.Items ?? [];

        return new PagedResult<object>
        {
            Items = [.. items],
            Limit = model.Limit,
            Offset = model.Offset,
            ItemsCount = model.ItemsCount,
            TotalItemsCount = model.TotalItemsCount,
        };
    }

    protected internal bool IsIEntity => typeof(IEntity).IsAssignableFrom(typeof(EntityType)) ||
        typeof(IEntity<int>).IsAssignableFrom(typeof(EntityType)) ||
        typeof(IEntity<long>).IsAssignableFrom(typeof(EntityType)) ||
        typeof(IEntity<double>).IsAssignableFrom(typeof(EntityType)) ||
        typeof(IEntity<char>).IsAssignableFrom(typeof(EntityType)) ||
        typeof(IEntity<string>).IsAssignableFrom(typeof(EntityType));

    /// <summary>
    /// Determines whether a runtime instance implements one of the supported IEntity contracts.
    /// </summary>
    /// <param name="item">Entity instance to inspect.</param>
    /// <returns><see langword="true"/> when the instance implements an IEntity variation.</returns>
    protected internal bool ImplementsEntity(EntityType item) => item is IEntity || item is IEntity<int> ||
        item is IEntity<long> ||
        item is IEntity<double> ||
        item is IEntity<char> ||
        item is IEntity<string>;

    protected bool IsNamedEntity => typeof(INamedEntity).IsAssignableFrom(typeof(EntityType));
    protected bool IsSoftDeletable => typeof(ISoftDeletable).IsAssignableFrom(typeof(EntityType));
    protected bool IsAuditable => typeof(IAuditable).IsAssignableFrom(typeof(EntityType));
    /// <summary>
    /// Maps an entity sequence to a model sequence.
    /// </summary>
    /// <param name="model">Entity sequence.</param>
    /// <returns>Mapped model sequence.</returns>
    protected internal IEnumerable<ModelType>? MapToIEnumerable(IEnumerable<EntityType> model) => model.Select(_mapper.ToModel);
    /// <summary>
    /// Returns the provided object sequence unchanged.
    /// </summary>
    /// <param name="model">Object sequence.</param>
    /// <returns>Same object sequence.</returns>
    protected internal IEnumerable<object>? MapToIEnumerable(IEnumerable<object> model) => model;
    /// <summary>
    /// Maps an entity sequence to a model list.
    /// </summary>
    /// <param name="model">Entity sequence.</param>
    /// <returns>Mapped model list.</returns>
    protected internal List<ModelType>? MapToList(IEnumerable<EntityType> model) => [.. model.Select(_mapper.ToModel)];
    /// <summary>
    /// Materializes an object sequence as a list.
    /// </summary>
    /// <param name="model">Object sequence.</param>
    /// <returns>Object list.</returns>
    protected internal List<object>? MapToList(IEnumerable<object> model) => [.. model];
    /// <summary>
    /// Maps a single entity to its API model.
    /// </summary>
    /// <param name="model">Entity instance.</param>
    /// <returns>Mapped model instance.</returns>
    protected internal ModelType? MapToModel(EntityType model) => _mapper.ToModel(model);
    /// <summary>
    /// Returns the provided object unchanged.
    /// </summary>
    /// <param name="model">Object instance.</param>
    /// <returns>Same object instance.</returns>
    protected internal object? MapToModel(object model) => model;

}
