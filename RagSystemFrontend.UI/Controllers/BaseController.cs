using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.UI.Controllers;


public abstract class BaseController : Controller
{

    protected async Task<IActionResult?> RedirectIfUnauthorizedAsync(ApiResult result)
    {
        if (!result.IsUnauthorized)
        {
            return null;
        }
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Error"] = "Sessione scaduta o non valida. Effettua nuovamente l'accesso.";
        return RedirectToAction("Login", "Account");
    }


    protected IActionResult ErrorRedirect(ApiResult result, string action, string? controller = null, object? routeValues = null)
    {
        TempData["Error"] = result.ErrorMessage;
        return controller is null
            ? RedirectToAction(action, routeValues)
            : RedirectToAction(action, controller, routeValues);
    }

    protected async Task<IActionResult> HandleFailureAsync(ApiResult result, string action, string? controller = null, object? routeValues = null) =>
        await RedirectIfUnauthorizedAsync(result) ?? ErrorRedirect(result, action, controller, routeValues);


}
