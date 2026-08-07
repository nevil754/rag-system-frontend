namespace RagSystemFrontend.Core.DTOs;

/// <summary>
/// Stato di un job di ingestion. Stessa forma esatta per GET /documents/{id}/status,
/// GET /jobs e GET /jobs/{id} (vedi README4.md §5/§7).
/// </summary>
public sealed record IngestionJobDto
{
    public string Id { get; init; } = "";
    public string DocumentId { get; init; } = "";
    public string? CeleryTaskId { get; init; }
    public string Status { get; init; } = "";
    public int ProgressPct { get; init; }
    public string? ErrorMsg { get; init; }
    public int RetryCount { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record CancelJobResponse
{
    public string Message { get; init; } = "";
    public string JobId { get; init; } = "";
}
