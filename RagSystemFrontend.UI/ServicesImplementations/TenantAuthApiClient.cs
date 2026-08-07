using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;


public class TenantAuthApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), ITenantAuthApiClient
{
    public Task<ApiResult<TokenResponse>> LoginAsync(TenantLoginRequest request, CancellationToken ct = default) =>
        SendAsync<TokenResponse>(CreateTenantRequest(HttpMethod.Post, "auth/login", request), ct);

    public Task<ApiResult<TokenResponse>> RefreshAsync(CancellationToken ct = default) =>
        SendAsync<TokenResponse>(CreateTenantRequest(HttpMethod.Post, "auth/refresh"), ct);

    public Task<ApiResult<MeResponse>> GetMeAsync(CancellationToken ct = default) =>
        SendAsync<MeResponse>(CreateTenantRequest(HttpMethod.Get, "auth/me"), ct);

    public Task<ApiResult<LogoutResponse>> LogoutAsync(CancellationToken ct = default) =>
        SendAsync<LogoutResponse>(CreateTenantRequest(HttpMethod.Post, "auth/logout"), ct);
}


