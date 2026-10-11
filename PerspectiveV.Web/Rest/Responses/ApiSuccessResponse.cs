using Microsoft.AspNetCore.Http;

namespace PerspectiveV.Web.Rest.Responses;

/// <summary>
/// Standard success response envelope for untyped payloads.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiSuccessResponse(new { id = 1 });
/// ]]></code>
/// </example>
public class ApiSuccessResponse : ApiResponse {
    /// <summary>
    /// Initializes a new instance of the ApiSuccessResponse class.
    /// </summary>
    /// <param name="content">Payload content to return.</param>
    public ApiSuccessResponse(object? content = null) : base() {
        Content = content ?? null;
        StatusCode = StatusCodes.Status200OK;
    }
    /// <summary>
    /// Initializes a new instance of the ApiSuccessResponse class.
    /// </summary>
    /// <param name="content">Payload content to return.</param>
    /// <param name="statusCode">HTTP status code, defaults to 200 OK.</param>
    public ApiSuccessResponse(object? content = null, int? statusCode = StatusCodes.Status200OK) : base() {
        Content = content ?? null;
        StatusCode = statusCode;
    }
    public object? Content { get; set; }
}

/// <summary>
/// Standard success response envelope for strongly-typed payloads.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiSuccessResponse&lt;string&gt;("done");
/// ]]></code>
/// </example>
public class ApiSuccessResponse<T> : ApiResponse {
    /// <summary>
    /// Initializes a new instance of the ApiSuccessResponse class.
    /// </summary>
    /// <param name="content">Payload content to return.</param>
    public ApiSuccessResponse(T? content) : base() {
        Content = content;
        StatusCode = StatusCodes.Status200OK;
    }
    /// <summary>
    /// Initializes a new instance of the ApiSuccessResponse class.
    /// </summary>
    /// <param name="content">Payload content to return.</param>
    /// <param name="statusCode">HTTP status code, defaults to 200 OK.</param>
    public ApiSuccessResponse(T? content, int? statusCode = StatusCodes.Status200OK) : base() {
        Content = content;
        StatusCode = statusCode;
    }
    public T? Content { get; set; }
}
