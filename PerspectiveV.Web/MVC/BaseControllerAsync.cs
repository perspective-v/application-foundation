using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerspectiveV.Common;
using PerspectiveV.Common.AppConfigurations;
using PerspectiveV.Common.Extensions;
using PerspectiveV.Common.Identity;
using PerspectiveV.Common.Mapper;
using PerspectiveV.Common.Paging;
using PerspectiveV.EntityFramework.Repository.Net6.Interfaces;
using PerspectiveV.Primitives.Types.Entities;
using PerspectiveV.Web.Extensions;

namespace PerspectiveV.Web.MVC;

/// <summary>
/// Provides controller behavior for Base Controller Async.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of BaseControllerAsync.
/// var type = typeof(BaseControllerAsync<,,,>);
/// ]]></code>
/// </example>
public class BaseControllerAsync<TEntity, TViewModel, TDBContext, TIUnitOfWork> : Controller {
    private readonly TIUnitOfWork? _uow;
    private readonly IRepositoryAsync<TEntity, TDBContext> _service;
    private readonly IProjectionMapper<string, TEntity, TViewModel> _mapper;
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
    /// <param name="uow">The uow value.</param>
    /// <param name="mapper">The mapper value.</param>
    /// <param name="defaultPagingOptions">The defaultPagingOptions value.</param>
    /// <param name="appSettingOptions">The appSettingOptions value.</param>
    /// <param name="env">The env value.</param>
    public BaseControllerAsync(TIUnitOfWork uow, IProjectionMapper<string, TEntity, TViewModel> mapper, IOptions<PagingOptions>? defaultPagingOptions = null, IOptions<AppSettings>? appSettingOptions = null, IWebHostEnvironment? env = null) {
        _uow = uow;
        _mapper = mapper;
        if (env != null) _env = env;

        _service = (IRepositoryAsync<TEntity, TDBContext>)typeof(TIUnitOfWork).GetProperty($"{typeof(TEntity).Name}Service")?.GetValue(_uow)!;

        if (appSettingOptions != null) _appSettings = appSettingOptions.Value;

        if (defaultPagingOptions != null) _defaultPagingOptions = defaultPagingOptions.Value;

        if (_appSettings is not null && _env is not null) {
            ContentOrWebRootPath = _appSettings.VirtualDirectory?.IsEnabled == true
                ? Path.Combine(_env.ContentRootPath, $"..\\{_appSettings.VirtualDirectory?.Name}")
                : _env.WebRootPath;
        }
    }

    /// <summary>
    /// Initializes a new instance of the BaseControllerAsync class.
    /// </summary>
    /// <param name="service">The service value.</param>
    /// <param name="mapper">The mapper value.</param>
    /// <param name="defaultPagingOptions">The defaultPagingOptions value.</param>
    public BaseControllerAsync(IRepositoryAsync<TEntity, TDBContext> service, IProjectionMapper<string, TEntity, TViewModel> mapper, IOptions<PagingOptions>? defaultPagingOptions = null) {
        _service = service;
        _mapper = mapper;
        if (defaultPagingOptions != null) _defaultPagingOptions = defaultPagingOptions.Value;
    }

    /// <summary>
    /// Executes Index Custom Query.
    /// </summary>
    /// <param name="action">The action value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    public virtual async Task<IActionResult> IndexCustomQuery(Func<Task<IEnumerable<TEntity>>> action) {
        if (NotHasViewPermission()) return Unauthorized();

        if (action is not null) {
            var list = await action();
            return View(MapToList(list));
        }
        else {
            var list = (await _service.GetAsync());
            return View(MapToList(list ?? []));
        }
    }

    /// <summary>
    /// Executes Index.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public virtual async Task<IActionResult> Index() {
        if (NotHasViewPermission()) {
            return Unauthorized();
        }

        return View(MapToList(await _service.GetAsync() ?? []));
    }

    /// <summary>
    /// Executes Paged.
    /// </summary>
    /// <param name="pagingOptions">The pagingOptions value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    [HttpGet("[action]")]
    public virtual async Task<IActionResult> Paged([FromQuery] PagingOptions pagingOptions) {
        pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions?.Offset;
        pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions?.Limit;
        var result = await _service.PagedAsync(pagingOptions: pagingOptions);
        var res = MapToPagedResult(result!);
        return View(res);
    }

    /// <summary>
    /// Creates Create Or Update.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    [HttpGet]
    public virtual async Task<IActionResult> CreateOrUpdate(string id) {
        if (NotHasCreateOrEditPermission(id))
            return Unauthorized();

        var vm = Activator.CreateInstance<TViewModel>();
        if (!string.IsNullOrEmpty(id)) {
            var entity = await _service.GetByIdAsync(id);
            vm = entity is not null ? _mapper.ToModel(entity) : vm;
        }
        return View(vm);
    }

    /// <summary>
    /// Creates Create Or Update.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <param name="vm">The vm value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> CreateOrUpdate(string id, TViewModel vm) {
        //ModelState.Remove(nameof(vm.Id));
        if (ModelState.IsValid) {
            try {

                var m = _mapper.ToEntity(vm);
                if (string.IsNullOrEmpty(id)) {
                    if (!User.HasPermission(typeof(TEntity).Name, AccountClaimConsts.PERMISSION_CREATE)) {
                        TempData.SetError(AppConstants.AuthorizationErrors.CREATE);
                        return View(vm);
                    }
                    SetPropertyValueIfExists(m, nameof(AuditableEntity.CreatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "System");
                    SetPropertyValueIfExists(m, nameof(AuditableEntity.CreatedDate), TimeProvider.System.GetUtcNow());
                    // create
                    await _service.CreateAsync(m);
                    TempData.SetMSG("Successfully Created");
                    return RedirectToAction(nameof(Index));

                }
                else {
                    if (!User.HasPermission(typeof(TEntity).Name, AccountClaimConsts.PERMISSION_EDIT)) {
                        TempData.SetError(AppConstants.AuthorizationErrors.EDIT);
                        return View(vm);
                    }
                    //update
                    var m2 = await _service.GetByIdAsync(id);
                    m2 = _mapper.ToEntity(vm);

                    SetPropertyValueIfExists(m, nameof(AuditableEntity.UpdatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "System");
                    SetPropertyValueIfExists(m, nameof(AuditableEntity.UpdatedDate), TimeProvider.System.GetUtcNow());

                    await _service.UpdateAsync(m2);
                    TempData.SetMSG("Successfully Updated");
                    return RedirectToAction(nameof(Index));

                    //return RedirectToAction(nameof(CreateOrUpdate), new { id = id });
                }
            }
            catch (Exception ex) {
                TempData.SetError(ex.Message);
                throw;
            }
        }

        return View(vm);
        //return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Executes Delete.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> Delete(string id) {
        try {
            if (string.IsNullOrEmpty(id)) {
                return new BadRequestObjectResult($"{nameof(id)} can not be null");
            }

            if (!User.HasPermission(typeof(TEntity).Name, AccountClaimConsts.PERMISSION_DELETE)) {
                TempData.SetError(AppConstants.AuthorizationErrors.DELETE);
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            await _service.SoftDeleteAsync(id);
            // _service.SoftDelete(_service.GetById(id));

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
        catch (Exception) {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            throw;
        }

    }

    /// <summary>
    /// Executes Not Has View Permission.
    /// </summary>
    /// <returns>The operation result.</returns>
    protected internal bool NotHasViewPermission() => !User.HasPermission(typeof(TEntity).Name, AccountClaimConsts.PERMISSION_VIEW);
    /// <summary>
    /// Executes Not Has Create Or Edit Permission.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <returns>The operation result.</returns>
    protected internal bool NotHasCreateOrEditPermission(string id) => string.IsNullOrEmpty(id) && !User.HasPermission(typeof(TEntity).Name, AccountClaimConsts.PERMISSION_CREATE) ||
            !string.IsNullOrEmpty(id) && !User.HasPermission(typeof(TEntity).Name, AccountClaimConsts.PERMISSION_EDIT);

    /// <summary>
    /// Maps Map To Paged Result.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal PagedResult<TViewModel> MapToPagedResult(PagedResult<TEntity> model) {
        var items = model.Items?.Select(_mapper.ToModel);
        return new PagedResult<TViewModel> {
            Items = items,
            Limit = model.Limit,
            Offset = model.Offset,
            ItemsCount = model.ItemsCount,
            TotalItemsCount = model.TotalItemsCount,
        };
    }

    /// <summary>
    /// Maps Map To Paged Result.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal PagedResult<object> MapToPagedResult(PagedResult<object> model) {
        var items = model.Items;
        return new PagedResult<object> {
            Items = items,
            Limit = model.Limit,
            Offset = model.Offset,
            ItemsCount = model.ItemsCount,
            TotalItemsCount = model.TotalItemsCount,
        };
    }
    /// <summary>
    /// Maps Map To IEnumerable.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal IEnumerable<TEntity>? MapToIEnumerable(IEnumerable<TEntity> model) {
        return model;
    }
    /// <summary>
    /// Maps Map To IEnumerable.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal IEnumerable<object>? MapToIEnumerable(IEnumerable<object> model) {
        return model;
    }
    /// <summary>
    /// Maps Map To List.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal List<TEntity>? MapToList(IEnumerable<TEntity> model) {
        return [.. model];
    }
    /// <summary>
    /// Maps Map To List.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal List<object>? MapToList(IEnumerable<object> model) {
        return [.. model];
    }
    /// <summary>
    /// Maps Map To View Model.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal TViewModel? MapToViewModel(TEntity? model) {
        return model is not null ? _mapper.ToModel(model) : default;
    }

    /// <summary>
    /// Maps Map To View Model.
    /// </summary>
    /// <param name="model">The model value.</param>
    /// <returns>The operation result.</returns>
    protected internal object? MapToViewModel(object model) {
        return model;
    }

    /// <summary>
    /// Sets Set Audit Info.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <param name="m">The m value.</param>
    /// <returns>The operation result.</returns>
    protected internal TEntity SetAuditInfo(string id, TEntity m) {
        // TODO: Implement this logic in mapper or somewhere better.
        if (string.IsNullOrEmpty(id)) {

            SetPropertyValueIfExists(m, nameof(AuditableEntity.CreatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "System");
            SetPropertyValueIfExists(m, nameof(AuditableEntity.CreatedDate), TimeProvider.System.GetUtcNow());
        }
        else {

            SetPropertyValueIfExists(m, nameof(AuditableEntity.UpdatedBy), HttpContext?.User?.Identity?.GetUserId() ?? "System");
            SetPropertyValueIfExists(m, nameof(AuditableEntity.UpdatedDate), TimeProvider.System.GetUtcNow());
        }
        return m;
    }

    /// <summary>
    /// Sets Set Property Value If Exists.
    /// </summary>
    /// <param name="m">The m value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <param name="value">The value value.</param>
    /// <returns>The operation result.</returns>
    protected internal void SetPropertyValueIfExists<T>(T m, string propertyName, object value) {
        var property = typeof(T).GetProperty(propertyName);
        property?.SetValue(m, value);
    }
}
