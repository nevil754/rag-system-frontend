using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>POST /documents/upload, GET /documents[/{id}/status], DELETE /documents/{id}.</summary>
public interface IDocumentsApiClient
{
    Task<ApiResult<DocumentUploadResponse>> UploadAsync(
        Stream fileStream, string fileName, string? contentType, string? collectionId, CancellationToken ct = default);

    Task<ApiResult<PaginatedResponse<DocumentDto>>> GetDocumentsAsync(
        int page, int pageSize, string? collectionId, string? statusFilter, CancellationToken ct = default);

    Task<ApiResult<IngestionJobDto>> GetStatusAsync(string documentId, CancellationToken ct = default);

    /// <summary>Richiede ruolo admin sul tenant.</summary>
    Task<ApiResult> DeleteAsync(string documentId, CancellationToken ct = default);
}
