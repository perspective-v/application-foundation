using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PerspectiveV.Web.Rest.Results;
using System.Text.Json;

namespace PerspectiveV.Web.Rest.Filters;
/// <summary>
/// Handles pipeline behavior for Api Exception Filter.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ApiExceptionFilter.
/// var type = typeof(ApiExceptionFilter);
/// ]]></code>
/// </example>
public class ApiExceptionFilter(IWebHostEnvironment env, ILogger<ApiExceptionFilter> logger) : IExceptionFilter {
    private static readonly JsonSerializerOptions LogSerializationOptions = new() { WriteIndented = true };

    /// <summary>
    /// Executes On Exception.
    /// </summary>
    /// <param name="context">The context value.</param>
    public void OnException(ExceptionContext context) {
        IActionResult? result = default;
        if (context.Exception is UnauthorizedAccessException) {
            result = new ApiExceptionResult(context.Exception.Message, StatusCodes.Status401Unauthorized);
        }
        else if (context.Exception is HttpRequestException httpRequestException) {
            result = new ApiExceptionResult(httpRequestException.Message, (int)httpRequestException.StatusCode.GetValueOrDefault());
        }
        else if (context.Exception is BadHttpRequestException badHttpRequestException) {
            result = new ApiExceptionResult(badHttpRequestException.Message, badHttpRequestException.StatusCode);
        }
        else {
            if (!env.IsProduction()) {
                result = new ApiExceptionDetailResult(context.Exception);
            }
            else {
                result = new ApiExceptionResult("Please report this issue to our dev team");
            }

            var safeError = FlattenException(context.Exception);

            var serialized = JsonSerializer.Serialize(safeError, LogSerializationOptions);
            logger.LogError("[-- {Class}.{Method}:{Path} --] : {Message} \n {ExceptionJson}",
                   nameof(ApiExceptionFilter),
                   nameof(OnException),
                   context.HttpContext.Request.Path,
                   context.Exception.Message,
                   serialized
            );
        }
        context.Result = result;
    }

    /// <summary>
    /// Executes Flatten Exception.
    /// </summary>
    /// <param name="ex">The ex value.</param>
    /// <returns>The operation result.</returns>
    private static List<object> FlattenException(Exception ex)
    {
        var flattened = new List<object>();
        Exception? current = ex;
        while (current != null)
        {
            flattened.Add(new
            {
                current.Message,
                current.Source,
                current.StackTrace
            });
            current = current.InnerException;
        }
        return flattened;
    }
}
