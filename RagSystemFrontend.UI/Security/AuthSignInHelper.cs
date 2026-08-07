using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RagSystemFrontend.Core.DTOs;

namespace RagSystemFrontend.UI.Security;

//claims = info associata all’utente autenticato, metadati contenuti nel payload del JWT (che è un HEADER.PAYLOAD.SIGNATURE)
//backend fastapi restituisce un token -> frontend ASP.NET crea delle claim -> le claim vengono inserite in un cookie autenticato -> alle richieste successive ASP.NET ricostruisce HttpContext.User. e.g. un utente platform che seleziona uno Space mantiene la claim platform_token, per poter tornare alla lista degli Space o cambiarne uno senza rifare il login (vedi SpacesController.Select)

public static class AuthSignInHelper
{
    //x auth l’utente nel contesto di un platform 
    public static async Task SignInPlatformAsync(HttpContext httpContext, PlatformTokenResponse token)
    {
        //HttpContext rappresenta la re http corrente, PlatformTokenResponse è la risposta del backend dopo il successful login
        var claims = new List<Claim>   //lista x contenere tutte le info da salvare nell'identita authenticata
        {
            new(ClaimTypes.NameIdentifier, token.PlatformUserId),   //NameIdentifier = claim standard x identificare univocamente l’utente
            new(ClaimTypes.Email, token.Email),
            new(AuthClaimTypes.PlatformToken, token.AccessToken),   //questo verra quindi usato sempre x fare req http al backend
            new(AuthClaimTypes.PlatformUserId, token.PlatformUserId),
        };
        await SignInAsync(httpContext, claims, token.ExpiresIn);  //usa tua custom funct
    }

    //x auth l’utente nel contesto di un tenant-scope 
    public static async Task SignInTenantAsync(HttpContext httpContext, TokenResponse token, string email, bool keepPlatformClaims)
    {
        //email è l’email dell’utente, keepPlatformClaims indica se mantenere le claim del platform gia esistenti (per poter tornare alla lista degli Space o cambiarne uno senza rifare il login)
        var claims = new List<Claim>();
        if (keepPlatformClaims)   //conservazione delle claims platform gia esistenti
        {
            var platformToken = httpContext.User.FindFirst(AuthClaimTypes.PlatformToken);
            var platformUserId = httpContext.User.FindFirst(AuthClaimTypes.PlatformUserId);
            if (platformToken is not null) claims.Add(new Claim(AuthClaimTypes.PlatformToken, platformToken.Value));
            if (platformUserId is not null) claims.Add(new Claim(AuthClaimTypes.PlatformUserId, platformUserId.Value));
        }
        claims.Add(new Claim(ClaimTypes.NameIdentifier, token.UserId));
        claims.Add(new Claim(ClaimTypes.Email, email));
        claims.Add(new Claim(ClaimTypes.Role, token.UserRole));
        claims.Add(new Claim(AuthClaimTypes.TenantToken, token.AccessToken));
        claims.Add(new Claim(AuthClaimTypes.TenantSlug, token.TenantSlug));
        claims.Add(new Claim(AuthClaimTypes.UserId, token.UserId));
        await SignInAsync(httpContext, claims, token.ExpiresIn);  //usa tua custom funct
    }

    
    private static async Task SignInAsync(HttpContext httpContext, List<Claim> claims, int expiresInSeconds)
    {
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);  //ClaimsIdentity rappresenta una singola identità autenticata, l'altro è lo schema di autenticazione (cookie)
        var principal = new ClaimsPrincipal(identity);   //ClaimsPrincipal rappresenta l’utente completo
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            //qua viene realmente generato il cookie
            IsPersistent = false,   //cookie non sopravvive alla chiusura del browser
            ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds),  //60min di scadenza, allienato a quello del backend
        });
    }


}
