using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;


public class JobsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IJobsApiClient
{
    public Task<ApiResult<PaginatedResponse<IngestionJobDto>>> GetJobsAsync(
        int page, int pageSize, string? status, CancellationToken ct = default)
    {
        var query = $"jobs?page={page}&page_size={pageSize}";
        if (!string.IsNullOrWhiteSpace(status)) query += $"&status={Uri.EscapeDataString(status)}";
        return SendAsync<PaginatedResponse<IngestionJobDto>>(CreateTenantRequest(HttpMethod.Get, query), ct);
    }

    public Task<ApiResult<IngestionJobDto>> GetJobAsync(string jobId, CancellationToken ct = default) =>
        SendAsync<IngestionJobDto>(CreateTenantRequest(HttpMethod.Get, $"jobs/{jobId}"), ct);

    public Task<ApiResult<CancelJobResponse>> CancelAsync(string jobId, CancellationToken ct = default) =>
        SendAsync<CancelJobResponse>(CreateTenantRequest(HttpMethod.Post, $"jobs/{jobId}/cancel"), ct);
}


