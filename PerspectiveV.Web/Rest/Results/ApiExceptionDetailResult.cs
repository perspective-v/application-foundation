using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerspectiveV.Web.Rest.Filters;
using PerspectiveV.Web.Rest.Responses;

namespace PerspectiveV.Web.Rest.Results;

/// <summary>
/// ObjectResult wrapper that emits detailed exception payloads.
/// </summary>
/// <example>
/// <code><![CDATA[
/// return new ApiExceptionDetailResult(ex);
/// ]]></code>
/// </example>
public class ApiExceptionDetailResult : ObjectResult {

    /// <summary>
    /// Initializes a new instance of the ApiExceptionDetailResult class.
    /// </summary>
    /// <param name="response">Detailed exception response payload.</param>
    public ApiExceptionDetailResult(ApiExceptionDetailResponse response) : base(response) {
        StatusCode = response.StatusCode;
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionDetailResult class.
    /// </summary>
    /// <param name="value">Exception-like value to convert into a detail response.</param>
    /// <param name="statusCode">HTTP status code to apply.</param>
    public ApiExceptionDetailResult(object? value, int? statusCode) : base(null) {
        StatusCode = statusCode;
        Value = new ApiExceptionDetailResponse(value, statusCode);
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionDetailResult class.
    /// </summary>
    /// <param name="value">Exception to wrap as an internal server error detail response.</param>
    public ApiExceptionDetailResult(Exception value) : base(null) {
        StatusCode = StatusCodes.Status500InternalServerError;
        Value = new ApiExceptionDetailResponse(new ApiException(value));
    }
}
