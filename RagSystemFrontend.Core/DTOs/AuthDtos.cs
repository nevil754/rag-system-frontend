namespace RagSystemFrontend.Core.DTOs;

// --- Livello tenant: POST /auth/login, /auth/refresh ---

public sealed record TenantLoginRequest(string Email, string Password, string TenantSlug);

public sealed record TokenResponse
{
    public string AccessToken { get; init; } = "";
    public string TokenType { get; init; } = "bearer";
    public int ExpiresIn { get; init; }
    public string UserId { get; init; } = "";
    public string UserRole { get; init; } = "";
    public string TenantSlug { get; init; } = "";
}

public sealed record MeResponse
{
    public string UserId { get; init; } = "";
    public string Email { get; init; } = "";
    public string? FullName { get; init; }
    public string Role { get; init; } = "";
    public string TenantId { get; init; } = "";
    public string TenantSlug { get; init; } = "";
}

public sealed record LogoutResponse
{
    public string Message { get; init; } = "";
    public int SessionsDeleted { get; init; }
}

// --- Livello platform: POST /auth/platform-login ---

public sealed record PlatformLoginRequest(string Email, string Password);

public sealed record PlatformTokenResponse
{
    public string AccessToken { get; init; } = "";
    public string TokenType { get; init; } = "bearer";
    public int ExpiresIn { get; init; }
    public string PlatformUserId { get; init; } = "";
    public string Email { get; init; } = "";
    public bool IsSuperAdmin { get; init; }
}
