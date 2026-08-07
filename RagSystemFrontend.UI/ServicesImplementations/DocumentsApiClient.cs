using System.Net.Http.Headers;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;

public class DocumentsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IDocumentsApiClient
{
    public Task<ApiResult<DocumentUploadResponse>> UploadAsync(
        Stream fileStream, string fileName, string? contentType, string? collectionId, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "documents/upload");
        if (!string.IsNullOrEmpty(TenantToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TenantToken);
        }

        var form = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
        form.Add(fileContent, "file", fileName);

        if (!string.IsNullOrWhiteSpace(collectionId))
        {
            form.Add(new StringContent(collectionId), "collection_id");
        }

        request.Content = form;
        return SendAsync<DocumentUploadResponse>(request, ct);
    }

    public Task<ApiResult<PaginatedResponse<DocumentDto>>> GetDocumentsAsync(
        int page, int pageSize, string? collectionId, string? statusFilter, CancellationToken ct = default)
    {
        var query = new List<string> { $"page={page}", $"page_size={pageSize}" };
        if (!string.IsNullOrWhiteSpace(collectionId)) query.Add($"collection_id={Uri.EscapeDataString(collectionId)}");
        if (!string.IsNullOrWhiteSpace(statusFilter)) query.Add($"status_filter={Uri.EscapeDataString(statusFilter)}");

        return SendAsync<PaginatedResponse<DocumentDto>>(
            CreateTenantRequest(HttpMethod.Get, $"documents?{string.Join('&', query)}"), ct);
    }

    public Task<ApiResult<IngestionJobDto>> GetStatusAsync(string documentId, CancellationToken ct = default) =>
        SendAsync<IngestionJobDto>(CreateTenantRequest(HttpMethod.Get, $"documents/{documentId}/status"), ct);

    public Task<ApiResult> DeleteAsync(string documentId, CancellationToken ct = default) =>
        SendAsync(CreateTenantRequest(HttpMethod.Delete, $"documents/{documentId}"), ct);
}
