using UserManagementAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = builder.Build();

app.MapUserEndpoints();

app.Run();