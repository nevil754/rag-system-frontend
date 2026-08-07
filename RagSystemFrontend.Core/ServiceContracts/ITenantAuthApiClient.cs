using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>Livello tenant: POST /auth/login, /auth/refresh, GET /auth/me, POST /auth/logout.</summary>
public interface ITenantAuthApiClient
{
    Task<ApiResult<TokenResponse>> LoginAsync(TenantLoginRequest request, CancellationToken ct = default);

    Task<ApiResult<TokenResponse>> RefreshAsync(CancellationToken ct = default);

    Task<ApiResult<MeResponse>> GetMeAsync(CancellationToken ct = default);

    Task<ApiResult<LogoutResponse>> LogoutAsync(CancellationToken ct = default);
}
