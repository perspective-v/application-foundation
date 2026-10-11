using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerspectiveV.Web.Rest.Responses;

namespace PerspectiveV.Web.Rest.Results;

/// <summary>
/// ObjectResult wrapper for created-resource responses.
/// </summary>
/// <example>
/// <code><![CDATA[
/// return new ApiResourceCreatedResult(createdItem);
/// ]]></code>
/// </example>
public class ApiResourceCreatedResult : ObjectResult {

    /// <summary>
    /// Initializes a new instance of the ApiResourceCreatedResult class.
    /// </summary>
    /// <param name="value">Created resource payload.</param>
    /// <param name="statusCode">HTTP status code, defaults to 201 Created.</param>
    public ApiResourceCreatedResult(object? value = null, int? statusCode = StatusCodes.Status201Created) : base(null) {
        StatusCode = statusCode;
        TimeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        Value = new ApiResourceCreatedResponse(value, statusCode);
    }
    /// <summary>
    /// Initializes a new instance of the ApiResourceCreatedResult class.
    /// </summary>
    /// <param name="value">Prebuilt created-resource response payload.</param>
    public ApiResourceCreatedResult(ApiResourceCreatedResponse? value) : base(value) {
        StatusCode = StatusCodes.Status201Created;
        TimeStamp = value?.TimeStamp ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        Content = new ApiResourceCreatedResponse(value);
    }
    /// <summary>
    /// Initializes a new instance of the ApiResourceCreatedResult class.
    /// </summary>
    /// <param name="value">Created resource payload.</param>
    public ApiResourceCreatedResult(object? value) : base(value) {
        StatusCode = StatusCodes.Status201Created;
        TimeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        Content = new ApiResourceCreatedResponse(value);
    }
    public DateTime TimeStamp { get; set; }
    public object? Content { get; set; }
}
