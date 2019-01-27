using Microsoft.AspNetCore.Builder;
using Planner.Api.Extensions.Middlewares;

namespace Planner.Api.Extensions
{
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
