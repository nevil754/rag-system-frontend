using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;

public class CollectionsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), ICollectionsApiClient
{
    public Task<ApiResult<CollectionDto>> CreateAsync(CreateCollectionRequest request, CancellationToken ct = default) =>
        SendAsync<CollectionDto>(CreateTenantRequest(HttpMethod.Post, "collections", request), ct);

    public Task<ApiResult<PaginatedResponse<CollectionDto>>> GetCollectionsAsync(int page, int pageSize, CancellationToken ct = default) =>
        SendAsync<PaginatedResponse<CollectionDto>>(
            CreateTenantRequest(HttpMethod.Get, $"collections?page={page}&page_size={pageSize}"), ct);

    public Task<ApiResult> DeleteAsync(string collectionId, CancellationToken ct = default) =>
        SendAsync(CreateTenantRequest(HttpMethod.Delete, $"collections/{collectionId}"), ct);
}
