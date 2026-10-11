namespace PerspectiveV.Web.Rest.Responses;
/// <summary>
/// Base API response envelope containing status metadata.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiResponse(200);
/// ]]></code>
/// </example>
public class ApiResponse {
    /// <summary>
    /// Initializes a new instance of the ApiResponse class.
    /// </summary>
    public ApiResponse() {
        TimeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
    }
    /// <summary>
    /// Initializes a new instance of the ApiResponse class.
    /// </summary>
    /// <param name="statusCode">HTTP status code for the response envelope.</param>
    public ApiResponse(int? statusCode) : this() {
        StatusCode = statusCode;
    }
    public int? StatusCode { get; set; }
    public DateTime TimeStamp { get; set; }
}

/// <summary>
/// Typed API response envelope containing metadata plus a strongly-typed payload.
/// </summary>
/// <example>
/// <code><![CDATA[
/// var response = new ApiResponse&lt;string&gt;("ok");
/// ]]></code>
/// </example>
public class ApiResponse<T>(T? content) : ApiResponse() {
    public T? Content { get; set; } = content;
}
