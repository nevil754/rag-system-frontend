using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>POST/GET /users, DELETE /users/{id}. Auth: admin sul tenant corrente.</summary>
public interface IUsersApiClient
{
    Task<ApiResult<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

    Task<ApiResult<List<UserDto>>> GetUsersAsync(CancellationToken ct = default);

    Task<ApiResult> DeleteAsync(string userId, CancellationToken ct = default);
}
