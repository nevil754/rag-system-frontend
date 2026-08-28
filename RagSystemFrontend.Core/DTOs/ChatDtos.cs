namespace RagSystemFrontend.Core.DTOs;

public sealed record ChatQueryRequest
{
    public string Question { get; init; } = "";
    public string? ConversationId { get; init; }
    public string? CollectionId { get; init; }
}

public sealed record SourceDto
{
    public string ChunkId { get; init; } = "";
    public string DocumentId { get; init; } = "";
    public string Filename { get; init; } = "";
    public int? PageNumber { get; init; }
    public double Score { get; init; }
    public string? Snippet { get; init; }
}

public sealed record ChatQueryResponse
{
    public string Answer { get; init; } = "";
    public string ConversationId { get; init; } = "";
    public long MessageId { get; init; }
    public List<SourceDto> Sources { get; init; } = [];
    public int? TokensIn { get; init; }
    public int? TokensOut { get; init; }
    public int? LatencyMs { get; init; }
}

public sealed record ChatFeedbackRequest
{
    public long MessageId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

public sealed record ChatFeedbackResponse
{
    public string Message { get; init; } = "";
}

public sealed record MessageDto
{
    public long Id { get; init; }
    public string Role { get; init; } = "";
    public string Content { get; init; } = "";
    public List<SourceDto> Sources { get; init; } = [];
    public DateTimeOffset CreatedAt { get; init; }
    public double? HallucinationScore { get; init; }
}

public sealed record ChatHistoryResponse
{
    public string? ConversationId { get; init; }
    public List<MessageDto> Messages { get; init; } = [];
    public bool HasMore { get; init; }
}
