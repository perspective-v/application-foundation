using Microsoft.AspNetCore.Http;
using PerspectiveV.Common.Paging;

namespace PerspectiveV.Web.Rest.Responses;

/// <summary>
/// Success response wrapper for paged payloads.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiPagedSuccessResponse&lt;UserDto&gt;(pagedUsers);
/// ]]></code>
/// </example>
public class ApiPagedSuccessResponse<T> : ApiSuccessResponse<PagedResult<T>> {
    /// <summary>
    /// Initializes a new instance of the ApiPagedSuccessResponse class.
    /// </summary>
    /// <param name="content">Paged payload content.</param>
    public ApiPagedSuccessResponse(PagedResult<T>? content = null) : base(content) {
        Content = content ?? null;
        StatusCode = StatusCodes.Status200OK;
    }
    /// <summary>
    /// Initializes a new instance of the ApiPagedSuccessResponse class.
    /// </summary>
    /// <param name="content">Paged payload content.</param>
    /// <param name="statusCode">HTTP status code, defaults to 200 OK.</param>
    public ApiPagedSuccessResponse(PagedResult<T>? content = null, int statusCode = StatusCodes.Status200OK)
        : base(content, statusCode) {
        Content = content ?? null;
        StatusCode = statusCode;
    }
}
