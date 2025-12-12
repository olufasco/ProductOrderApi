using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Common;

namespace ProductOrderApi.Middleware
{
    public class ValidationErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Let the pipeline run
            await _next(context);

            // If response is already handled, skip
            if (context.Response.HasStarted) return;

            // Check for validation errors
            if (context.Items.TryGetValue("ValidationErrors", out var errorsObj) && errorsObj is IDictionary<string, string[]> errors)
            {
                var response = ApiResponse<object>.Fail("Validation failed", errors);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}