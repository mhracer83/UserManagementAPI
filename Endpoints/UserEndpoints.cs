using Microsoft.AspNetCore.Authorization;
using UserManagementAPI.Models;

namespace UserManagementAPI.Endpoints;

public static class UserEndpoints
{
    private static readonly List<User> Users = new List<User>
    {
        new() { Id = 1, UserName = "JohnDoe", EmailAddress = "JohnDoe@gmail.com" },
        new() { Id = 2, UserName = "JaneDoe", EmailAddress = "JaneDoe@gmail.com"}
    };

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        const string GetEndpointName = "GetUserById";

        var group = app.MapGroup("users");

        // Secure endpoints with the [Authorize] attribute
        group.MapGet("/", [Authorize] () =>
        {
            try
            {
                return Results.Ok(Users);
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while fetching the users. " + ex.Message);
            }
        });

        group.MapGet("/{id}", [Authorize] (int id) =>
        {
            try
            {
                return Results.Ok(Users.FirstOrDefault(user => user.Id == id)) ?? Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while fetching the user. " + ex.Message);
            }
        })
        .WithName(GetEndpointName);

        group.MapPost("/", [Authorize] (User newUser) =>
        {
            try
            {
                if (newUser == null || string.IsNullOrWhiteSpace(newUser.UserName) || string.IsNullOrWhiteSpace(newUser.EmailAddress))
                {
                    return Results.BadRequest("Invalid user data.");
                }

                User user = new User
                {
                    Id = Users.Max(x => x.Id) + 1,
                    UserName = newUser.UserName,
                    EmailAddress = newUser.EmailAddress
                };
                Users.Add(user);
                return Results.CreatedAtRoute(GetEndpointName, new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while creating the user. " + ex.Message);
            }
        });

        group.MapPut("/{id}", [Authorize] (int id, User updatedUser) =>
        {
            try
            {
                if (updatedUser == null || string.IsNullOrWhiteSpace(updatedUser.UserName) || string.IsNullOrWhiteSpace(updatedUser.EmailAddress))
                {
                    return Results.BadRequest("Invalid user data.");
                }

                var existingUserIndex = Users.FindIndex(user => user.Id == id);
                if (existingUserIndex == -1)
                {
                    return Results.NotFound();
                }

                Users[existingUserIndex].UserName = updatedUser.UserName;
                Users[existingUserIndex].EmailAddress = updatedUser.EmailAddress;

                return Results.Ok(Users[existingUserIndex]);
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while updating the user. " + ex.Message);
            }
        });

        group.MapDelete("/{id}", [Authorize] (int id) =>
        {
            try
            {
                var existingUser = Users.FirstOrDefault(x => x.Id == id);
                if (existingUser is null)
                {
                    return Results.NotFound();
                }

                Users.Remove(existingUser);

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while deleting the user. " + ex.Message);
            }
        });

        return group;
    }
}
