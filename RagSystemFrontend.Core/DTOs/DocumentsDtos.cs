namespace RagSystemFrontend.Core.DTOs;

public sealed record DocumentUploadResponse
{
    public string DocumentId { get; init; } = "";
    public string JobId { get; init; } = "";
    public string TaskId { get; init; } = "";
    public string Status { get; init; } = "";
    public string Message { get; init; } = "";
}

public sealed record DocumentDto
{
    public string Id { get; init; } = "";
    public string? CollectionId { get; init; }
    public string Filename { get; init; } = "";
    public string OriginalName { get; init; } = "";
    public long FileSize { get; init; }
    public string? MimeType { get; init; }
    public string Status { get; init; } = "";
    public int? ChunkCount { get; init; }
    public int? PageCount { get; init; }
    public string? Language { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}
