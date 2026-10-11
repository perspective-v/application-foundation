using Microsoft.AspNetCore.Http;

namespace PerspectiveV.Web.Rest.Responses;

/// <summary>
/// Standard error response envelope for API failures.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiExceptionResponse("Validation failed", 400);
/// ]]></code>
/// </example>
public class ApiExceptionResponse : ApiResponse {
    /// <summary>
    /// Initializes a new instance of the ApiExceptionResponse class.
    /// </summary>
    public ApiExceptionResponse() : base() {
        StatusCode = StatusCodes.Status400BadRequest;
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionResponse class.
    /// </summary>
    /// <param name="message">Error message text.</param>
    public ApiExceptionResponse(string? message) : base() {
        StatusCode = StatusCodes.Status400BadRequest;
        Message = message;
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionResponse class.
    /// </summary>
    /// <param name="statusCode">HTTP status code for the error response.</param>
    public ApiExceptionResponse(int? statusCode) : base(statusCode) {
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionResponse class.
    /// </summary>
    /// <param name="message">Error message text.</param>
    /// <param name="statusCode">HTTP status code for the error response.</param>
    public ApiExceptionResponse(string? message, int? statusCode) : base(statusCode) {
        Message = message;
    }

    public string? Message { get; set; }
}
