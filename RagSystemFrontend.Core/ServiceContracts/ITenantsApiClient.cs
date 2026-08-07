using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>POST/GET /tenants, PATCH /tenants/{slug}/disable. Auth: Bearer platform + is_superadmin.</summary>
public interface ITenantsApiClient
{
    Task<ApiResult<CreateTenantResponse>> CreateAsync(CreateTenantRequest request, CancellationToken ct = default);

    Task<ApiResult<List<TenantDto>>> GetTenantsAsync(CancellationToken ct = default);

    Task<ApiResult<DisableTenantResponse>> DisableAsync(string slug, CancellationToken ct = default);
}
