namespace RagSystemFrontend.Core.DTOs;

/// <summary>Uno Space (tenant posseduto da un account platform). GET/POST/PATCH /spaces.</summary>
public sealed record SpaceDto
{
    public string Id { get; init; } = "";
    public string Slug { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Plan { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record CreateSpaceRequest
{
    public string Name { get; init; } = "";
    // Onorati dal backend solo se chi crea è superadmin: piano diverso da "starter" e/o
    // credenziali admin dedicate (ufficio non legato al proprio account platform).
    public string Plan { get; init; } = "starter";
    public string? AdminEmail { get; init; }
    public string? AdminPassword { get; init; }
}

public sealed record RenameSpaceRequest(string Name);
