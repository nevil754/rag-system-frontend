namespace RagSystemFrontend.Core.DTOs;

/// <summary>Provisioning "senza owner" riservato ai superadmin platform. POST/GET /tenants.</summary>
public sealed record CreateTenantRequest
{
    public string DisplayName { get; init; } = "";
    public string Plan { get; init; } = "starter";
    public string? AdminEmail { get; init; }
    public string? AdminPassword { get; init; }
}

public sealed record CreateTenantResponse
{
    public string TenantId { get; init; } = "";
    public string Slug { get; init; } = "";
    public string Plan { get; init; } = "";
    public string? AdminUserId { get; init; }
}

public sealed record TenantDto
{
    public string Id { get; init; } = "";
    public string Slug { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Plan { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record DisableTenantResponse
{
    public string Message { get; init; } = "";
}
