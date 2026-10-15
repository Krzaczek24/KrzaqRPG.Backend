using KrzaqRPG.Backend.WebApi.Core.Extensions;
using NLog;
using System.Text;

namespace KrzaqRPG.Backend.WebApi.Core.Middlewares
{
    public class LoggingMiddleware(RequestDelegate next)
    {
        protected static NLog.ILogger Logger { get; } = LogManager.GetLogger(nameof(LoggingMiddleware));

        public async Task Invoke(HttpContext httpContext)
        {
            await HandleRequest(httpContext);
            await HandleResponse(httpContext);
        }

        protected virtual async Task HandleRequest(HttpContext httpContext)
        {
            string bodyText = string.Empty;
            httpContext.Request.EnableBuffering();
            using (StreamReader reader = new(httpContext.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 1024, leaveOpen: true))
            {
                bodyText = await reader.ReadToEndAsync();
                httpContext.Request.Body.Position = 0L;
            }

            LogRequest(httpContext, bodyText);
        }

        protected virtual async Task HandleResponse(HttpContext httpContext)
        {
            var originalBodyStream = httpContext.Response.Body;
            var responseBody = new MemoryStream();
            httpContext.Response.Body = responseBody;
            await next.Invoke(httpContext);
            httpContext.Response.Body.Seek(0L, SeekOrigin.Begin);
            string bodyText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            httpContext.Response.Body.Seek(0L, SeekOrigin.Begin);
            LogResponse(httpContext, bodyText);
            await responseBody.CopyToAsync(originalBodyStream);
        }

        protected virtual void LogRequest(HttpContext httpContext, string bodyText)
        {
            Logger.Info($"REQUEST  ({httpContext.GetRequestId()}) | PATH ({httpContext.Request.GetPath()}) | BODY ({bodyText})");
        }

        protected virtual void LogResponse(HttpContext httpContext, string bodyText)
        {
            Logger.Info($"RESPONSE ({httpContext.GetRequestId()}) | CODE ({httpContext.Response.StatusCode}) | BODY ({bodyText})");
        }
    }

    public static class LoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app) => app.UseMiddleware<LoggingMiddleware>();
    }
}
