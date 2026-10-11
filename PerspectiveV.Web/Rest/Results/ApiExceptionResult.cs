using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerspectiveV.Web.Rest.Responses;

namespace PerspectiveV.Web.Rest.Results;
/// <summary>
/// ObjectResult wrapper that emits standardized API exception responses.
/// </summary>
/// <example>
/// <code><![CDATA[
/// return new ApiExceptionResult("Validation failed", 400);
/// ]]></code>
/// </example>
public class ApiExceptionResult : ObjectResult {
    /// <summary>
    /// Initializes a new instance of the ApiExceptionResult class.
    /// </summary>
    /// <param name="value">Prebuilt API exception response payload.</param>
    public ApiExceptionResult(ApiExceptionResponse? value) : base(value) {
        Message = value?.Message;
        StatusCode = value?.StatusCode;
    }

    /// <summary>
    /// Initializes a new instance of the ApiExceptionResult class.
    /// </summary>
    /// <param name="message">Error message to include in the payload.</param>
    /// <param name="statusCode">HTTP status code for the response.</param>
    public ApiExceptionResult(string message, int statusCode) : base(null) {
        StatusCode = statusCode;
        Value = new ApiExceptionResponse(message, statusCode);
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionResult class.
    /// </summary>
    /// <param name="message">Error message to include in the payload.</param>
    public ApiExceptionResult(string message) : this(message, StatusCodes.Status500InternalServerError) {
    }
    public string? Message { get; set; }
    public DateTime TimeStamp { get; set; }
}
