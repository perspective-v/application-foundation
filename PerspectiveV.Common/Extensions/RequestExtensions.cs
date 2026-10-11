using Microsoft.AspNetCore.Http;

namespace PerspectiveV.Common.Extensions; 
/// <summary>
/// Provides extension methods for Request Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by RequestExtensions.
/// var typeName = nameof(RequestExtensions);
/// ]]></code>
/// </example>
public static class RequestExtensions {
    /// <summary>
    /// Gets Get Absolute Uri.
    /// </summary>
    /// <param name="request">The request value.</param>
    /// <returns>The operation result.</returns>
    public static string GetAbsoluteUri(this HttpRequest request) {
        return new Uri($"{request.Scheme}://{request.Host}/").AbsoluteUri;
    }
    /// <summary>
    /// Gets Get Absolute Uri.
    /// </summary>
    /// <param name="request">The request value.</param>
    /// <param name="scheme">The scheme value.</param>
    /// <param name="host">The host value.</param>
    /// <param name="port">The port value.</param>
    /// <returns>The operation result.</returns>
    public static string GetAbsoluteUri(this HttpRequest request,string? scheme = null,string? host = null, int? port = null)
    {
        return new Uri($"{scheme??request.Scheme}://{host??request.Host.Host}:{port ?? request.Host.Port}/").AbsoluteUri;
    }

    /// <summary>
    /// Executes Check And Get Dynamic Uri.
    /// </summary>
    /// <param name="request">The request value.</param>
    /// <param name="uriToCheck">The uriToCheck value.</param>
    /// <param name="virtualDirectoryName">The virtualDirectoryName value.</param>
    /// <returns>The operation result.</returns>
    public static string CheckAndGetDynamicUri(this HttpRequest request, string uriToCheck, string virtualDirectoryName = "") {
        var absoluteUri = request.GetAbsoluteUri();
        if (uriToCheck.Contains(absoluteUri)) {
            if (!string.IsNullOrEmpty(virtualDirectoryName)) {
                uriToCheck = uriToCheck.Replace($"/{virtualDirectoryName}", "");
            }
            uriToCheck = uriToCheck.Replace(absoluteUri, "~/");
        }
        return uriToCheck;
    }

    /// <summary>
    /// Executes Check And Get Absolute Uri.
    /// </summary>
    /// <param name="request">The request value.</param>
    /// <param name="uriToCheck">The uriToCheck value.</param>
    /// <param name="virtualDirectoryName">The virtualDirectoryName value.</param>
    /// <returns>The operation result.</returns>
    public static string CheckAndGetAbsoluteUri(this HttpRequest request, string uriToCheck, string virtualDirectoryName = "") {
        var absoluteUri = request.GetAbsoluteUri();
        if (uriToCheck.Contains("~/")) {
            if (!string.IsNullOrEmpty(virtualDirectoryName)) {
                uriToCheck = uriToCheck.Replace("~/", $"~/{virtualDirectoryName}/");
            }
            uriToCheck = uriToCheck.Replace("~/", absoluteUri);
        }
        return uriToCheck;
    }

    /// <summary>
    /// Executes Check And Get Absolute Uri.
    /// </summary>
    /// <param name="request">The request value.</param>
    /// <param name="uriToCheck">The uriToCheck value.</param>
    /// <param name="virtualDirectoryName">The virtualDirectoryName value.</param>
    /// <param name="scheme">The scheme value.</param>
    /// <param name="host">The host value.</param>
    /// <param name="port">The port value.</param>
    /// <returns>The operation result.</returns>
    public static string CheckAndGetAbsoluteUri(this HttpRequest request, string uriToCheck, string virtualDirectoryName = "", string? scheme = null, string? host = null, int? port = null)
    {
        var absoluteUri = request.GetAbsoluteUri(scheme, host, port);
        if (uriToCheck.Contains("~/"))
        {
            if (!string.IsNullOrEmpty(virtualDirectoryName))
            {
                uriToCheck = uriToCheck.Replace("~/", $"~/{virtualDirectoryName}/");
            }
            uriToCheck = uriToCheck.Replace("~/", absoluteUri);
        }
        return uriToCheck;
    }
}
