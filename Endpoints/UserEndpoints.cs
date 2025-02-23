using UserManagementAPI.Models;
using System.Text.Json;

namespace UserManagementAPI.Endpoints;

public static class UserEndpoints
{
    private static readonly List<User> Users = new();

    static UserEndpoints()
    {
        Users.Add(new() { Id = 1, UserName = "JohnDoe", EmailAddress = "JohnDoe@gmail.com" });
        Users.Add(new() { Id = 2, UserName = "JaneDoe", EmailAddress = "JaneDoe@gmail.com" });
    }

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        const string GetEndpointName = "GetUserById";
        var group = app.MapGroup("users");

        // GET /users
        group.MapGet("/", () => Results.Ok(Users));

        // GET /users/{id}
        group.MapGet("/{id}", (int id) =>
        {
            var user = Users.Find(user => user.Id == id);
            return user is not null ? Results.Ok(user) : Results.NotFound($"User with ID {id} not found.");
        }).WithName(GetEndpointName);

        // POST /users
        group.MapPost("/", async (HttpContext context) =>
        {
            try
            {
                var newUser = await context.Request.ReadFromJsonAsync<User>();
                if (newUser is null || string.IsNullOrWhiteSpace(newUser.UserName) || string.IsNullOrWhiteSpace(newUser.EmailAddress))
                {
                    return Results.BadRequest("Invalid user data. 'UserName' and 'EmailAddress' are required.");
                }
                var newId = Users.Count > 0 ? Users.Max(x => x.Id) + 1 : 1;
                var user = new User { Id = newId, UserName = newUser.UserName, EmailAddress = newUser.EmailAddress };
                Users.Add(user);
                return Results.CreatedAtRoute(GetEndpointName, new { id = user.Id }, user);
            }
            catch (JsonException)
            {
                return Results.BadRequest("Invalid JSON format.");
            }
        });

        // PUT /users/{id}
        group.MapPut("/{id}", async (HttpContext context, int id) =>
        {
            try
            {
                var updatedUser = await context.Request.ReadFromJsonAsync<User>();
                if (updatedUser is null || string.IsNullOrWhiteSpace(updatedUser.UserName) || string.IsNullOrWhiteSpace(updatedUser.EmailAddress))
                {
                    return Results.BadRequest("Invalid user data. 'UserName' and 'EmailAddress' are required.");
                }
                var existingUser = Users.FirstOrDefault(user => user.Id == id);
                if (existingUser is null)
                {
                    return Results.NotFound($"User with ID {id} not found.");
                }
                existingUser.UserName = updatedUser.UserName;
                existingUser.EmailAddress = updatedUser.EmailAddress;
                return Results.Ok(existingUser);
            }
            catch (JsonException)
            {
                return Results.BadRequest("Invalid JSON format.");
            }
        });

        // DELETE /users/{id}
        group.MapDelete("/{id}", (int id) =>
        {
            var existingUser = Users.FirstOrDefault(x => x.Id == id);
            if (existingUser is null)
            {
                return Results.NotFound($"User with ID {id} not found.");
            }
            Users.Remove(existingUser);
            return Results.NoContent();
        });

        return group;
    }
}
