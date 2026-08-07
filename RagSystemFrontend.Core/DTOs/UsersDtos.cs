namespace RagSystemFrontend.Core.DTOs;

public sealed record CreateUserRequest
{
    public string Email { get; init; } = "";
    public string? FullName { get; init; }
    public string Password { get; init; } = "";
    public string Role { get; init; } = "user";
}

public sealed record UserDto
{
    public string Id { get; init; } = "";
    public string Email { get; init; } = "";
    public string? FullName { get; init; }
    public string Role { get; init; } = "";
    public bool IsActive { get; init; }
}
