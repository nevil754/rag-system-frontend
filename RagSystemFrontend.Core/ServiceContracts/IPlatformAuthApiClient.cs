using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>Livello platform: POST /auth/platform-login.</summary>
public interface IPlatformAuthApiClient
{
    Task<ApiResult<PlatformTokenResponse>> LoginAsync(PlatformLoginRequest request, CancellationToken ct = default);
}
