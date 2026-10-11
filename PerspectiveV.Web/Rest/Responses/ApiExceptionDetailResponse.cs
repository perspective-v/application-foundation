using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerspectiveV.Web.Rest.Filters;

namespace PerspectiveV.Web.Rest.Responses;

/// <summary>
/// Detailed error response envelope that can include stack trace, nested exception,
/// and validation error payloads.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiExceptionDetailResponse(exception, 500);
/// ]]></code>
/// </example>
public class ApiExceptionDetailResponse : ApiExceptionResponse {
    //public ApiExceptionDetailResponse(int statusCode, string? message, string? stackTrace) : this(statusCode, message) {
    //    StackTrace = stackTrace;
    //}
    //public ApiExceptionDetailResponse(int statusCode, string? message, string? stackTrace, Exception? innerException) : this(statusCode, message, stackTrace) {
    //    Exception = innerException;
    //}
    /// <summary>
    /// Initializes a new instance of the ApiExceptionDetailResponse class.
    /// </summary>
    /// <param name="ex">Exception used to populate message and stack trace details.</param>
    /// <param name="statusCode">HTTP status code for the error response.</param>
    public ApiExceptionDetailResponse(Exception? ex, int? statusCode) : base(ex?.Message, statusCode) {
        Message = ex?.Message;
        StackTrace = ex?.StackTrace;
        ExceptionType = ex?.GetType().FullName;
        InnerMessage = ex?.InnerException?.Message;
    }

    /// <summary>
    /// Initializes a new instance of the ApiExceptionDetailResponse class.
    /// </summary>
    /// <param name="ex">Exception used to populate an internal-server-error response.</param>
    public ApiExceptionDetailResponse(Exception ex) : base(ex.Message) {
        StatusCode = StatusCodes.Status500InternalServerError;
        Message = ex?.Message;
        StackTrace = ex?.StackTrace;
        ExceptionType = ex?.GetType().FullName;
        InnerMessage = ex?.InnerException?.Message;
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionDetailResponse class.
    /// </summary>
    /// <param name="ex">API exception carrying status code and message details.</param>
    public ApiExceptionDetailResponse(ApiException ex) : base(ex.Message) {
        StatusCode = ex.StatusCode;
        Message = ex?.Message;
        StackTrace = ex?.StackTrace;
        ExceptionType = ex?.GetType().FullName;
        InnerMessage = ex?.InnerException?.Message;
    }
    /// <summary>
    /// Initializes a new instance of the ApiExceptionDetailResponse class.
    /// </summary>
    /// <param name="exception">Exception-like object or validation detail payload.</param>
    /// <param name="statusCode">HTTP status code for the error response.</param>
    public ApiExceptionDetailResponse(object? exception, int? statusCode) : base(statusCode) {
        if (exception is not null && exception is Exception) {
            var ex = exception as Exception;
            Message = ex?.Message;
            StackTrace = ex?.StackTrace;
            ExceptionType = ex?.GetType().FullName;
            InnerMessage = ex?.InnerException?.Message;
            Errors = null;
        }
        else if (exception is ValidationProblemDetails problemDetail) {
            Message = problemDetail.Title;
            Errors = problemDetail.Errors;
        }
        else {
            Errors = exception;
        }
    }
    public object? Errors { get; set; }
    public string? StackTrace { get; set; }
    public string? ExceptionType { get; set; }
    public string? InnerMessage { get; set; }
}
