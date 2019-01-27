using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Planner.Api.Extensions.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
