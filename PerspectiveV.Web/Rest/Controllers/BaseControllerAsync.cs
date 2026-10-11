using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerspectiveV.Common.AppConfigurations;
using PerspectiveV.Common.Paging;

namespace PerspectiveV.Web.Rest.Controllers;

/// <summary>
/// Provides controller behavior for Base Controller Async.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Derive from BaseControllerAsync and implement required members.
/// var type = typeof(BaseControllerAsync);
/// ]]></code>
/// </example>
public abstract class BaseControllerAsync<DBContextType, IUnitOfWorkType> : ControllerBase
{
    protected readonly IUnitOfWorkType _uow;
    protected readonly IWebHostEnvironment? _env;
    protected readonly PagingOptions? _defaultPagingOptions;
    protected readonly AppSettings? _appSettings;
    private MediaSaveOptions? _mediaSaveOptions;
    private S3BucketOptions? _s3BucketOptions;

    public MediaSaveOptions? MediaSaveOptions
    {
        get { return _mediaSaveOptions; }
        set { _mediaSaveOptions = value; }
    }

    public S3BucketOptions? S3BucketOptions
    {
        get { return _s3BucketOptions; }
        set { _s3BucketOptions = value; }
    }

    /// <summary>
    /// This will return ContentRootPath if virtual directory option is enabled otherwise WebRootPath
    /// </summary>
    protected string? ContentOrWebRootPath { get; private set; }
    /// <summary>
    /// Initializes a new instance of the BaseControllerAsync class.
    /// </summary>
    /// <param name="uow">The uow value.</param>
    /// <param name="defaultPagingOptions">The defaultPagingOptions value.</param>
    /// <param name="appSettingOptions">The appSettingOptions value.</param>
    /// <param name="env">The env value.</param>
    public BaseControllerAsync(IUnitOfWorkType uow, IOptions<PagingOptions>? defaultPagingOptions = null, IOptions<AppSettings>? appSettingOptions = null, IWebHostEnvironment? env = null)
    {
        _uow = uow;

        if (env != null) _env = env;

        if (appSettingOptions is not null) _appSettings = appSettingOptions.Value;

        if (defaultPagingOptions is not null) _defaultPagingOptions = defaultPagingOptions.Value;

        if (_appSettings is not null && _env is not null)
        {
            ContentOrWebRootPath = _appSettings.VirtualDirectory?.IsEnabled == true
                ? Path.Combine(_env.ContentRootPath, $"..\\{_appSettings.VirtualDirectory?.Name}")
                : _env.WebRootPath;

            if (_appSettings.S3Storage?.IsEnabled == true)
            {
                S3BucketOptions = new S3BucketOptions
                {
                    AccessKey = _appSettings.S3Storage?.AccessKey ?? string.Empty,
                    SecretKey = _appSettings.S3Storage?.SecretKey ?? string.Empty,
                    ServiceHost = _appSettings.S3Storage?.ServiceHost ?? string.Empty,
                    Region = _appSettings.S3Storage?.Region ?? string.Empty,
                    Bucket = _appSettings.S3Storage?.Bucket ?? string.Empty,
                    IsEnabled = _appSettings.S3Storage?.IsEnabled ?? false,
                };
            }
        }
    }


    /// <summary>
    /// Sets Set Property Value If Exists.
    /// </summary>
    /// <param name="m">The m value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <param name="value">The value value.</param>
    /// <returns>The operation result.</returns>
    protected internal void SetPropertyValueIfExists<T>(T m, string propertyName, object? value)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property is not null)
        {
            typeof(T).GetProperty(propertyName)?.SetValue(m, value);
        }
    }

    /// <summary>
    /// Gets Get Property Value If Exists.
    /// </summary>
    /// <param name="m">The m value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <returns>The operation result.</returns>
    protected internal TResult? GetPropertyValueIfExists<T, TResult>(T m, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property is not null)
        {
            var value = property.GetValue(m);
            return value is TResult castedValue ? castedValue : default;
        }
        return default;
    }

    /// <summary>
    /// Determines whether the current HTTP request contains a supported content type
    /// for processing form data. 
    /// </summary>
    /// <remarks>
    /// This property checks the <see cref="HttpContext.Request.ContentType"/> and returns <c>true</c>
    /// if the content type is either <c>application/x-www-form-urlencoded</c> or 
    /// <c>multipart/form-data</c>. These content types are typically used for form submissions
    /// and file uploads.
    /// </remarks>
    protected bool IsSupportedContentType =>
        !string.IsNullOrEmpty(HttpContext?.Request?.ContentType) &&
        (HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) ||
         HttpContext.Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase));


    /// <summary>
    /// Determines whether the current HTTP request contains valid file upload data.
    /// </summary>
    /// <remarks>
    /// This property verifies that the request supports form content types (either 
    /// <c>application/x-www-form-urlencoded</c> or <c>multipart/form-data</c>) and 
    /// that the form contains one or more uploaded files. 
    /// Returns <c>true</c> only when both conditions are satisfied.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// <![CDATA[
    /// if (HasValidFileUpload)
    /// {
    ///     var files = HttpContext.Request.Form.Files;
    ///     // Process uploaded files
    /// }
    /// ]]>
    /// ]]></code>
    /// </example>
    protected bool HasValidFileUpload =>
        HttpContext?.Request is { HasFormContentType: true } request &&
        ((request.ContentType?.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) == true) ||
         (request.ContentType?.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) == true)) &&
        request.Form?.Files?.Count > 0;
}

