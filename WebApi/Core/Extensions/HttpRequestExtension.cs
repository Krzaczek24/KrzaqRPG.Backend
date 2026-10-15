namespace KrzaqRPG.Backend.WebApi.Core.Extensions
{
    public static class HttpRequestExtension
    {
        public static string GetId(this HttpRequest request)
        {
            return request.Headers["RequestId"].FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? string.Empty;
        }

        public static string GetPath(this HttpRequest request)
        {
            return $"{request.Method} {request.Path}{request.QueryString}";
        }
    }
}
