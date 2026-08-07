namespace RagSystemFrontend.Core.DTOs;

public sealed record CollectionDto
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    public string? QdrantName { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record CreateCollectionRequest
{
    public string Name { get; init; } = "";
    public string? Description { get; init; }
}
