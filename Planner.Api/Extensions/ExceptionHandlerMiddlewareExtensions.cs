using Microsoft.AspNetCore.Builder;
using Planner.Api.Middlewares;

namespace Planner.Api.Services
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
