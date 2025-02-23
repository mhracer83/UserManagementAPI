using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log the request
            _logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

            // Copy the original response body stream to capture the response
            var originalResponseBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Continue processing the request
            await _next(context);

            // Log the response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            _logger.LogInformation("Response: {StatusCode} {ResponseText}", context.Response.StatusCode, responseText);

            // Copy the contents of the new memory stream to the original response body stream
            await responseBody.CopyToAsync(originalResponseBodyStream);
        }
    }
}
