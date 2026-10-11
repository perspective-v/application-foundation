using Microsoft.AspNetCore.Http;
using PerspectiveV.Primitives.Types.Models;

namespace PerspectiveV.Web.Rest.Responses;

/// <summary>
/// Success response wrapper for a name/value payload.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiNameValueSuccessResponse&lt;string&gt;(
///     new NameValuePair&lt;string&gt; { Name = "status", Value = "ok" });
/// ]]></code>
/// </example>
public class ApiNameValueSuccessResponse<T> : ApiSuccessResponse<NameValuePair<T>> {
    /// <summary>
    /// Initializes a new instance of the ApiNameValueSuccessResponse class.
    /// </summary>
    /// <param name="content">Name/value payload content.</param>
    public ApiNameValueSuccessResponse(NameValuePair<T>? content = null) : base(content) {
        Content = content ?? null;
        StatusCode = StatusCodes.Status200OK;
    }
    /// <summary>
    /// Initializes a new instance of the ApiNameValueSuccessResponse class.
    /// </summary>
    /// <param name="content">Name/value payload content.</param>
    /// <param name="statusCode">HTTP status code, defaults to 200 OK.</param>
    public ApiNameValueSuccessResponse(NameValuePair<T>? content = null, int statusCode = StatusCodes.Status200OK)
        : base(content, statusCode) {
        Content = content ?? null;
        StatusCode = statusCode;
    }
}
