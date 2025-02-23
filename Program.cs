using UserManagementAPI.Endpoints;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Register the exception handling middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // Register the logging middleware

app.MapUserEndpoints();

app.Run();
