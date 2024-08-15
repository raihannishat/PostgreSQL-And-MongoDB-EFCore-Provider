namespace EFCoreForPostgreSQLAndMongoDB.Endpoints;

public static class UserEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var userGroup = app.MapGroup("/api/user");

        // Get all users
        userGroup.MapGet("/", async (IUserService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(users);
        });

        // Get user by ID
        userGroup.MapGet("/{id}", async (string id, IUserService userService) =>
        {
            var user = await userService.GetUserByIdAsync(id);
            return user == null ? Results.NotFound() : Results.Ok(user);
        });

        // Create a new user
        userGroup.MapPost("/", async (User user, IUserService userService) =>
        {
            await userService.AddUserAsync(user);
            return Results.Created($"/api/user/{user.Id}", user);
        });

        // Update an existing user
        userGroup.MapPut("/{id}", async (string id, User user, IUserService userService) =>
        {
            var existingUser = await userService.GetUserByIdAsync(id);
            if (existingUser == null) return Results.NotFound();

            await userService.UpdateUserAsync(user);
            return Results.NoContent();
        });

        // Delete a user
        userGroup.MapDelete("/{id}", async (string id, IUserService userService) =>
        {
            var existingUser = await userService.GetUserByIdAsync(id);
            if (existingUser == null) return Results.NotFound();

            await userService.DeleteUserAsync(id);
            return Results.NoContent();
        });

        // Search users by name
        userGroup.MapGet("/search", async (string name, IUserService userService) =>
        {
            var users = await userService.SearchUserAsync(c => c.Name!.Contains(name));
            return Results.Ok(users);
        });

        // Add a contact to a user
        userGroup.MapPost("/{id}/contacts", async (string id, Phone phone, IUserService userService) =>
        {
            await userService.AddPhoneNumberAsync(id, phone);
            return Results.Ok(phone);
        });

        // Get all contacts for a user
        userGroup.MapGet("/{id}/contacts", async (string id, IUserService userService) =>
        {
            var contacts = await userService.GetUserPhoneNumberByIdAsync(id);
            return Results.Ok(contacts);
        });

        // Add or update an address for a user
        userGroup.MapPost("/{id}/address", async (string id, Address address, IUserService userService) =>
        {
            await userService.AddAddressAsync(id, address);
            return Results.Ok(address);
        });

        // Get a user's address
        userGroup.MapGet("/{id}/address", async (string id, IUserService userService) =>
        {
            var address = await userService.GetUserAddressByIdAsync(id);
            return Results.Ok(address);
        });
    }
}