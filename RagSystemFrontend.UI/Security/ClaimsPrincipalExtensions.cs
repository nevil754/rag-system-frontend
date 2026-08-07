using System.Security.Claims;

namespace RagSystemFrontend.UI.Security;

//methods usati nel code: x checkare se true/false cose settate nel cookie di autenticazione (claims) / estrarre info come email tenant slug role
public static class ClaimsPrincipalExtensions
{
    public static bool IsTenantAuthenticated(this ClaimsPrincipal user) =>
        user.HasClaim(c => c.Type == AuthClaimTypes.TenantToken);

    public static bool IsPlatformAuthenticated(this ClaimsPrincipal user) =>
        user.HasClaim(c => c.Type == AuthClaimTypes.PlatformToken);

    public static bool IsTenantAdmin(this ClaimsPrincipal user) =>
        user.IsTenantAuthenticated() && user.IsInRole("admin");

    public static string? TenantSlug(this ClaimsPrincipal user) =>
        user.FindFirst(AuthClaimTypes.TenantSlug)?.Value;

    public static string? Email(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Email)?.Value;

    public static string? TenantRole(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Role)?.Value;
}
