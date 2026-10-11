using Microsoft.AspNetCore.Http;

namespace PerspectiveV.Web.Rest.Responses;

/// <summary>
/// Created-resource response envelope for strongly-typed payloads.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiResourceCreatedResponse&lt;UserDto&gt;(createdUser);
/// ]]></code>
/// </example>
public class ApiResourceCreatedResponse<T> : ApiSuccessResponse<T>
    where T : class {
    /// <summary>
    /// Initializes a new instance of the ApiResourceCreatedResponse class.
    /// </summary>
    /// <param name="content">Created resource payload.</param>
    public ApiResourceCreatedResponse(T? content) : base(content) {
        Content = content ?? null;
        StatusCode = StatusCodes.Status201Created;
    }

    /// <summary>
    /// Initializes a new instance of the ApiResourceCreatedResponse class.
    /// </summary>
    /// <param name="content">Created resource payload.</param>
    /// <param name="statusCode">HTTP status code, defaults to 201 Created.</param>
    public ApiResourceCreatedResponse(T? content = null, int statusCode = StatusCodes.Status201Created) : base(content, statusCode) {
        Content = content ?? null;
        StatusCode = statusCode;
    }
}


/// <summary>
/// Created-resource response envelope for untyped payloads.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiResourceCreatedResponse(createdUser);
/// ]]></code>
/// </example>
public class ApiResourceCreatedResponse : ApiResponse {
    /// <summary>
    /// Initializes a new instance of the ApiResourceCreatedResponse class.
    /// </summary>
    /// <param name="content">Created resource payload.</param>
    public ApiResourceCreatedResponse(object? content) : base() {
        Content = content ?? null;
        StatusCode = StatusCodes.Status201Created;
    }
    /// <summary>
    /// Initializes a new instance of the ApiResourceCreatedResponse class.
    /// </summary>
    /// <param name="content">Created resource payload.</param>
    /// <param name="statusCode">HTTP status code, defaults to 201 Created.</param>
    public ApiResourceCreatedResponse(object? content = null, int? statusCode = StatusCodes.Status201Created) : base() {
        Content = content ?? null;
        StatusCode = statusCode;
    }
    public object? Content { get; set; }
}
