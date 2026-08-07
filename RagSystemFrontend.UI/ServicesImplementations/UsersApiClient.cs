using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;


public class UsersApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IUsersApiClient
{
    public Task<ApiResult<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default) =>
        SendAsync<UserDto>(CreateTenantRequest(HttpMethod.Post, "users", request), ct);

    public Task<ApiResult<List<UserDto>>> GetUsersAsync(CancellationToken ct = default) =>
        SendAsync<List<UserDto>>(CreateTenantRequest(HttpMethod.Get, "users"), ct);

    public Task<ApiResult> DeleteAsync(string userId, CancellationToken ct = default) =>
        SendAsync(CreateTenantRequest(HttpMethod.Delete, $"users/{userId}"), ct);
}

