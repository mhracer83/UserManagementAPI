using UserManagementAPI.Endpoints;
using UserManagementAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MyAPI",
            ValidAudience = "MyAPIUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_ultra_super_secret_key_1234567890123456"))
        };
    });


builder.Services.AddAuthorization(); // Add this line to add authorization services
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Register the exception handling middleware
app.UseAuthentication(); // Register the authentication middleware
app.UseAuthorization(); // Register the authorization middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // Register the logging middleware


app.MapUserEndpoints();
app.MapPost("/generateToken", () =>
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, "user123"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_ultra_super_secret_key_1234567890123456"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "MyAPI",
        audience: "MyAPIUsers",
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
});

app.Run();
