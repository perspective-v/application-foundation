using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PerspectiveV.Web.Rest.Filters;

/// <summary>
/// Represents the ApiException type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ApiException.
/// var type = typeof(ApiException);
/// ]]></code>
/// </example>
public class ApiException : Exception {
    /// <summary>
    /// Initializes a new instance of the ApiException class.
    /// </summary>
    public ApiException() : base() {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
    /// <summary>
    /// Initializes a new instance of the ApiException class.
    /// </summary>
    /// <param name="ex">The ex value.</param>
    public ApiException(Exception ex) : base(ex.Message, ex) {
        StatusCode = StatusCodes.Status500InternalServerError;
        Message = ex.Message;
        StackTrace = ex.StackTrace;
        Source = ex.Source;
        HelpLink = ex.HelpLink;
    }

    /// <summary>
    /// Initializes a new instance of the ApiException class.
    /// </summary>
    /// <param name="message">The message value.</param>
    public ApiException(string message) {
        StatusCode = StatusCodes.Status500InternalServerError;
        Message = message;
    }
    /// <summary>
    /// Initializes a new instance of the ApiException class.
    /// </summary>
    /// <param name="message">The message value.</param>
    /// <param name="statusCode">The statusCode value.</param>
    public ApiException(string message, int statusCode) : this(message) {
        StatusCode = statusCode;
    }
    /// <summary>
    /// Initializes a new instance of the ApiException class.
    /// </summary>
    /// <param name="message">The message value.</param>
    /// <param name="stackTrace">The stackTrace value.</param>
    public ApiException(string message, string stackTrace) : this(message) {
        StackTrace = stackTrace;
    }

    /// <summary>
    /// Initializes a new instance of the ApiException class.
    /// </summary>
    /// <param name="modelState">The modelState value.</param>
    public ApiException(ModelStateDictionary modelState) : this("Invalid parameters") {
        var errorEntry = modelState.FirstOrDefault(x => x.Value != null && x.Value.Errors.Any());
        var firstError = errorEntry.Value?.Errors.FirstOrDefault()?.ErrorMessage;

        Detail = firstError ?? "An unknown validation error occurred.";
    }

    public new string? Message { get; set; }
    public string? Detail { get; set; }
    public int StatusCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [DefaultValue("")]
    public new string? StackTrace { get; set; }

}
