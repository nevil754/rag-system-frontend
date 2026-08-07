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

public sealed record CreateSpaceRequest(string Name);

public sealed record RenameSpaceRequest(string Name);
