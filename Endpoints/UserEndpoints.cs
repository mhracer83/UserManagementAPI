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

        //GET /users
        group.MapGet("/", () =>
        {
            try
            {
                return Results.Ok(Users); // Wrap Users in Results.Ok
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while fetching the users. " + ex.Message);
            }
        });

        //GET /users/{id}
        group.MapGet("/{id}", (int id) =>
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

        //POST /users
        group.MapPost("/", (User newUser) =>
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

        // PUT /users/{id}
        group.MapPut("/{id}", (int id, User updatedUser) =>
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

                return Results.Ok(Users[existingUserIndex]); // Return updated user
            }
            catch (Exception ex)
            {
                return Results.Problem("An error occurred while updating the user. " + ex.Message);
            }
        });

       //DELETE /users/{id}
        group.MapDelete("/{id}", (int id) =>
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