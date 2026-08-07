namespace RagSystemFrontend.Core.DTOs.Common;

/// <summary>
/// Mirror di PaginatedResponse del backend (usato da GET /documents, /collections, /jobs).
/// </summary>
public sealed record PaginatedResponse<T>
{
    public List<T> Items { get; init; } = [];
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public bool HasMore { get; init; }
}
