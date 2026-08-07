using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>GET/POST /collections, DELETE /collections/{id}.</summary>
public interface ICollectionsApiClient
{
    Task<ApiResult<CollectionDto>> CreateAsync(CreateCollectionRequest request, CancellationToken ct = default);

    Task<ApiResult<PaginatedResponse<CollectionDto>>> GetCollectionsAsync(int page, int pageSize, CancellationToken ct = default);

    /// <summary>Richiede ruolo admin sul tenant.</summary>
    Task<ApiResult> DeleteAsync(string collectionId, CancellationToken ct = default);
}
