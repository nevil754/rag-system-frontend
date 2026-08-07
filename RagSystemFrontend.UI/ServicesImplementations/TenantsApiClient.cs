using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;


public class TenantsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), ITenantsApiClient
{
    public Task<ApiResult<CreateTenantResponse>> CreateAsync(CreateTenantRequest request, CancellationToken ct = default) =>
        SendAsync<CreateTenantResponse>(CreatePlatformRequest(HttpMethod.Post, "tenants", request), ct);

    public Task<ApiResult<List<TenantDto>>> GetTenantsAsync(CancellationToken ct = default) =>
        SendAsync<List<TenantDto>>(CreatePlatformRequest(HttpMethod.Get, "tenants"), ct);

    public Task<ApiResult<DisableTenantResponse>> DisableAsync(string slug, CancellationToken ct = default) =>
        SendAsync<DisableTenantResponse>(CreatePlatformRequest(HttpMethod.Patch, $"tenants/{slug}/disable"), ct);
}

