using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>GET /jobs[/{id}], POST /jobs/{id}/cancel.</summary>
public interface IJobsApiClient
{
    Task<ApiResult<PaginatedResponse<IngestionJobDto>>> GetJobsAsync(int page, int pageSize, string? status, CancellationToken ct = default);

    Task<ApiResult<IngestionJobDto>> GetJobAsync(string jobId, CancellationToken ct = default);

    /// <summary>Richiede ruolo admin sul tenant.</summary>
    Task<ApiResult<CancelJobResponse>> CancelAsync(string jobId, CancellationToken ct = default);
}
