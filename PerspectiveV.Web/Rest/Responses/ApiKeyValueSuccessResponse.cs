using Microsoft.AspNetCore.Http;
using PerspectiveV.Primitives.Types.Models;

namespace PerspectiveV.Web.Rest.Responses;

/// <summary>
/// Success response wrapper for a key/value payload.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiKeyValueSuccessResponse&lt;int&gt;(
///     new KeyValuePair&lt;int&gt; { Key = "count", Value = 10 });
/// ]]></code>
/// </example>
public class ApiKeyValueSuccessResponse<T> : ApiSuccessResponse<KeyValuePair<T>> {
    /// <summary>
    /// Initializes a new instance of the ApiKeyValueSuccessResponse class.
    /// </summary>
    /// <param name="content">Key/value payload content.</param>
    public ApiKeyValueSuccessResponse(KeyValuePair<T>? content = null) : base(content) {
        Content = content ?? null;
        StatusCode = StatusCodes.Status200OK;
    }
    /// <summary>
    /// Initializes a new instance of the ApiKeyValueSuccessResponse class.
    /// </summary>
    /// <param name="statusCode">HTTP status code, defaults to 200 OK.</param>
    /// <param name="content">Key/value payload content.</param>
    public ApiKeyValueSuccessResponse(int statusCode = StatusCodes.Status200OK, KeyValuePair<T>? content = null)
        : base(content, statusCode) {
        Content = content ?? null;
        StatusCode = statusCode;
    }
}
