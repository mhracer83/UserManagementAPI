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
        group.MapGet("/", () => Users);

        //GET /users/{id}
        group.MapGet("/{id}", (int id) => Users.Find(user => user.Id == id))
        .WithName(GetEndpointName);

        //POST /users
        group.MapPost("/", (User newUser) =>
        {
            User user = new User
            {
                Id = Users.Max(x => x.Id) + 1,
                UserName = newUser.UserName,
                EmailAddress = newUser.EmailAddress
            };
            Users.Add(user);
            return Results.CreatedAtRoute(GetEndpointName, new {id = user.Id}, user);
        });

        // PUT /users/{id}
        group.MapPut("/{id}", (int id, User updatedUser) =>
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
        });


        //DELETE /users/{id}
        group.MapDelete("/{id}", (int id) =>
        {
            var existingUser = Users.FirstOrDefault(x => x.Id == id);
            if (existingUser is null)
            {
                return Results.NotFound();
            }

            Users.Remove(existingUser);

            return Results.NoContent();
        });

        return group;
    }
}




