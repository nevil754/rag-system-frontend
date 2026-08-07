using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>GET/POST /spaces, PATCH /spaces/{id}[/disable], POST /spaces/{id}/select. Auth: Bearer platform.</summary>
public interface ISpacesApiClient
{
    Task<ApiResult<List<SpaceDto>>> GetSpacesAsync(CancellationToken ct = default);

    Task<ApiResult<SpaceDto>> CreateSpaceAsync(CreateSpaceRequest request, CancellationToken ct = default);

    Task<ApiResult<SpaceDto>> RenameSpaceAsync(string spaceId, RenameSpaceRequest request, CancellationToken ct = default);

    Task<ApiResult> DisableSpaceAsync(string spaceId, CancellationToken ct = default);

    /// <summary>Emette un JWT tenant-scoped per lo Space selezionato.</summary>
    Task<ApiResult<TokenResponse>> SelectSpaceAsync(string spaceId, CancellationToken ct = default);
}
