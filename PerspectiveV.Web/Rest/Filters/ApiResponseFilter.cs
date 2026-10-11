using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PerspectiveV.Web.Rest.Results;
using System.Text.Json;

namespace PerspectiveV.Web.Rest.Filters;

/// <summary>
/// Handles pipeline behavior for Api Response Filter.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ApiResponseFilter.
/// var type = typeof(ApiResponseFilter);
/// ]]></code>
/// </example>
public class ApiResponseFilter : IAsyncResultFilter {
    private static readonly JsonSerializerOptions LogSerializationOptions = new() { WriteIndented = true };

    private readonly ILogger<ApiResponseFilter> _logger;
    /// <summary>
    /// Initializes a new instance of the ApiResponseFilter class.
    /// </summary>
    /// <param name="logger">The logger value.</param>
    public ApiResponseFilter(ILogger<ApiResponseFilter> logger) {
        _logger = logger;
    }

    /// <summary>
    /// Executes On Result Execution Async.
    /// </summary>
    /// <param name="context">The context value.</param>
    /// <param name="next">The next value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {

        ObjectResult? result = context.Result switch
        {
            ObjectResult or => or,
            StatusCodeResult scr => new ObjectResult(null) { StatusCode = scr.StatusCode },
            _ => null
        };

        var isNull = result is null || result?.Value is null;

        var statusCode = result?.StatusCode ?? StatusCodes.Status200OK;
        bool isError = result == null ||
               result.Value == null ||
               statusCode >= 400;

        var payload = result?.Value;

        if (isError)
        {
            // Safe logging (prevents serializer crashes)
            string json;
            try
            {
                json = JsonSerializer.Serialize(context.Result, LogSerializationOptions);
            }
            catch
            {
                json = "[unserializable result]";
            }

            _logger.LogError(
                "[-- {Filter}.{Method}:{Controller} ---] : {ResultJson}",
                nameof(ApiResponseFilter),
                nameof(OnResultExecutionAsync),
                nameof(context.Controller),
                json);

            if (payload is HttpValidationProblemDetails validationDetails)
            {
                context.Result = new ApiExceptionDetailResult(validationDetails, statusCode);
            }
            else if (payload is ProblemDetails problemDetails)
            {
                context.Result = new ApiExceptionDetailResult(problemDetails, statusCode);
            }
            else if (payload is Exception exception)
            {
                context.Result = new ApiExceptionDetailResult(exception, statusCode);
            }
            else
            {
                context.Result = new ApiExceptionDetailResult(payload, statusCode);
            }

            return;
        }

        context.Result = (statusCode == StatusCodes.Status201Created) ? new ApiResourceCreatedResult(payload ?? null, statusCode) : new ApiSuccessResult(payload ?? null, statusCode);

        await next();
    }
}
