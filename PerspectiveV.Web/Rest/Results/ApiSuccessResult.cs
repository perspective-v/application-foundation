using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerspectiveV.Web.Rest.Responses;

namespace PerspectiveV.Web.Rest.Results;

/// <summary>
/// ObjectResult wrapper for standardized success responses.
/// </summary>
/// <example>
/// <code><![CDATA[
/// return new ApiSuccessResult(data);
/// ]]></code>
/// </example>
public class ApiSuccessResult : ObjectResult {
    /// <summary>
    /// Initializes a new instance of the ApiSuccessResult class.
    /// </summary>
    /// <param name="value">Prebuilt API success response payload.</param>
    public ApiSuccessResult(ApiSuccessResponse? value = null) : base(value) {

        if (value == null) {
            StatusCode = StatusCodes.Status200OK;
            TimeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            Content = null;
        }
        else {
            StatusCode = value.StatusCode;
            TimeStamp = value.TimeStamp;
            Content = value.Content;
        }
    }

    /// <summary>
    /// Initializes a new instance of the ApiSuccessResult class.
    /// </summary>
    /// <param name="value">Success payload content.</param>
    /// <param name="statusCode">HTTP status code, defaults to 200 OK.</param>
    public ApiSuccessResult(object? value = null, int? statusCode = StatusCodes.Status200OK) : base(null) {
        StatusCode = statusCode;
        TimeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        Value = new ApiSuccessResponse(value, statusCode);
    }

    public DateTime TimeStamp { get; set; }
    public object? Content { get; set; }
}
