using System.Security.Claims;

namespace KrzaqRPG.Backend.WebApi.Core.Extensions
{
    public static class HttpContextExtension
    {
        public static ClaimsPrincipal? GetUser(this IHttpContextAccessor contextAccessor) => contextAccessor.HttpContext?.User;

        public static string? GetClientIp(this IHttpContextAccessor contextAccessor) => contextAccessor.HttpContext?.GetClientIp();
        public static string? GetClientIp(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString();
        }

        public static void SetRequestId(this HttpContext context, Guid guid)
        {
            context.Features.Set(guid);
        }

        public static Guid GetRequestId(this HttpContext context)
        {
            return context.Features.Get<Guid>();
        }

        public static void PassRequestId(this HttpContext context)
        {
            context.SetRequestId(Guid.TryParse(context.Request.GetId(), out var result) ? result : Guid.NewGuid());
        }
    }
}
