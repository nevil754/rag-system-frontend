using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;


public class SpacesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), ISpacesApiClient
{
    public Task<ApiResult<List<SpaceDto>>> GetSpacesAsync(CancellationToken ct = default) =>
        SendAsync<List<SpaceDto>>(CreatePlatformRequest(HttpMethod.Get, "spaces"), ct);

    public Task<ApiResult<SpaceDto>> CreateSpaceAsync(CreateSpaceRequest request, CancellationToken ct = default) =>
        SendAsync<SpaceDto>(CreatePlatformRequest(HttpMethod.Post, "spaces", request), ct);

    public Task<ApiResult<SpaceDto>> RenameSpaceAsync(string spaceId, RenameSpaceRequest request, CancellationToken ct = default) =>
        SendAsync<SpaceDto>(CreatePlatformRequest(HttpMethod.Patch, $"spaces/{spaceId}", request), ct);

    public Task<ApiResult> DisableSpaceAsync(string spaceId, CancellationToken ct = default) =>
        SendAsync(CreatePlatformRequest(HttpMethod.Patch, $"spaces/{spaceId}/disable"), ct);

    public Task<ApiResult<TokenResponse>> SelectSpaceAsync(string spaceId, CancellationToken ct = default) =>
        SendAsync<TokenResponse>(CreatePlatformRequest(HttpMethod.Post, $"spaces/{spaceId}/select"), ct);
}

