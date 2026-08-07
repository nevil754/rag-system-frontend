using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;


public class PlatformAuthApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IPlatformAuthApiClient
{
    public Task<ApiResult<PlatformTokenResponse>> LoginAsync(PlatformLoginRequest request, CancellationToken ct = default) =>
        SendAsync<PlatformTokenResponse>(CreatePlatformRequest(HttpMethod.Post, "auth/platform-login", request), ct);
}
