using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;
using RagSystemFrontend.UI.Security;

namespace RagSystemFrontend.UI.Controllers;

[AllowAnonymous]
public class AccountController(
    ITenantAuthApiClient tenantAuthApiClient,
    IPlatformAuthApiClient platformAuthApiClient) : Controller
{
    [HttpGet]
    public IActionResult Login(bool expired = false)
    {
        if (User.IsTenantAuthenticated() || User.IsPlatformAuthenticated())
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new AccountLoginViewModel { Expired = expired });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginTenant(LoginTenantViewModel tenant)
    {
        if (!ModelState.IsValid)
        {
            return View("Login", new AccountLoginViewModel { Tenant = tenant });
        }
        var result = await tenantAuthApiClient.LoginAsync(new TenantLoginRequest(tenant.Email, tenant.Password, tenant.TenantSlug));
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Accesso non riuscito.");
            return View("Login", new AccountLoginViewModel { Tenant = tenant });
        }
        await AuthSignInHelper.SignInTenantAsync(HttpContext, result.Data, tenant.Email, keepPlatformClaims: false);
        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginPlatform(LoginPlatformViewModel platform)
    {
        if (!ModelState.IsValid)
        {
            return View("Login", new AccountLoginViewModel { Platform = platform });
        }
        var result = await platformAuthApiClient.LoginAsync(new PlatformLoginRequest(platform.Email, platform.Password));
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Accesso non riuscito.");
            return View("Login", new AccountLoginViewModel { Platform = platform });
        }
        await AuthSignInHelper.SignInPlatformAsync(HttpContext, result.Data);
        return RedirectToAction("Index", "Spaces");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Invalida anche le sessioni di conversazione Redis lato backend (best-effort: il JWT
        // resta comunque valido fino a scadenza naturale, vedi README4.md §2 /auth/logout).
        if (User.IsTenantAuthenticated())
        {
            try
            {
                await tenantAuthApiClient.LogoutAsync();
            }
            catch
            {
                // logout locale non deve fallire per un errore del backend
            }
        }
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        return View();
    }


}
