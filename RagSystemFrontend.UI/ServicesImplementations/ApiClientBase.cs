using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.UI.Security;

namespace RagSystemFrontend.UI.ServicesImplementations;


public abstract class ApiClientBase(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
{
    protected readonly HttpClient HttpClient = httpClient;

    private string? GetClaim(string type) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(type)?.Value;

    protected string? TenantToken => GetClaim(AuthClaimTypes.TenantToken);

    protected string? PlatformToken => GetClaim(AuthClaimTypes.PlatformToken);

    protected HttpRequestMessage CreateTenantRequest(HttpMethod method, string relativeUrl, object? body = null) =>
        CreateRequest(method, relativeUrl, TenantToken, body);

    protected HttpRequestMessage CreatePlatformRequest(HttpMethod method, string relativeUrl, object? body = null) =>
        CreateRequest(method, relativeUrl, PlatformToken, body);

    private static HttpRequestMessage CreateRequest(HttpMethod method, string relativeUrl, string? token, object? body)
    {
        var request = new HttpRequestMessage(method, relativeUrl);
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, body.GetType(), RagApiJsonOptions.Default);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    protected async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken ct)
    {
        try
        {
            using var response = await HttpClient.SendAsync(request, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return ApiResult<T>.Fail((int)response.StatusCode, "Risposta vuota inattesa dal backend.");
                }

                var data = JsonSerializer.Deserialize<T>(content, RagApiJsonOptions.Default);
                return data is null
                    ? ApiResult<T>.Fail((int)response.StatusCode, "Impossibile leggere la risposta del backend.")
                    : ApiResult<T>.Ok(data, (int)response.StatusCode);
            }

            var (message, retryAfter) = ApiErrorParser.Parse((int)response.StatusCode, content);
            return ApiResult<T>.Fail((int)response.StatusCode, message, retryAfter);
        }
        catch (HttpRequestException ex)
        {
            return ApiResult<T>.Fail(0, $"Impossibile contattare il backend: {ex.Message}");
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return ApiResult<T>.Fail(0, "Timeout durante la chiamata al backend.");
        }
    }

    protected async Task<ApiResult> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        try
        {
            using var response = await HttpClient.SendAsync(request, ct);
            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Ok((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var (message, retryAfter) = ApiErrorParser.Parse((int)response.StatusCode, content);
            return ApiResult.Fail((int)response.StatusCode, message, retryAfter);
        }
        catch (HttpRequestException ex)
        {
            return ApiResult.Fail(0, $"Impossibile contattare il backend: {ex.Message}");
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return ApiResult.Fail(0, "Timeout durante la chiamata al backend.");
        }
    }
}
