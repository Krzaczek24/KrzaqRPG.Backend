using Krzaq.Converters.EnumToString;
using Krzaq.Exceptions.Http.Base;
using Krzaq.Exceptions.Http.Error.Base;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using NLog;
using System.Text.Json;

namespace KrzaqRPG.Backend.WebApi.Core.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    protected static NLog.ILogger Logger { get; } = LogManager.GetLogger(nameof(LoggingMiddleware));
    protected static JsonSerializerOptions JsonOpts { get; } = new()
    {
        Converters = { new EnumToStringConverter<ErrorCode>() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next.Invoke(httpContext);
        }
        catch (HttpException ex)
        {
            httpContext.Response.StatusCode = (int)ex.StatusCode;
            await HandleException(httpContext, ex);
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = 500;
            await HandleException(httpContext, ex);
        }
    }

    protected virtual async Task HandleException(HttpContext httpContext, Exception exception)
    {
        LogException(httpContext, exception);
        httpContext.Response.ContentType = "application/json";
        var errorResponse = GetErrorResponse(exception);
        string responseBody = JsonSerializer.Serialize(errorResponse, JsonOpts);
        await httpContext.Response.WriteAsync(responseBody);
    }

    protected virtual void LogException(HttpContext httpContext, Exception exception)
    {
        if (httpContext.Response.StatusCode >= 500)
        {
            Logger.Error(exception, exception.Message);
        }
    }

    protected virtual ErrorResponse GetErrorResponse(Exception exception) => exception switch
    {
        HttpErrorException<ErrorModel> ex => new ErrorResponse(ex.Errors),
        _ => new ErrorResponse([new(ErrorCode.Unknown, exception.Message)])
    };
}

public static class ExceptionHandlingMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
