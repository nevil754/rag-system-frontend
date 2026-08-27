using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;
using RagSystemFrontend.UI.Security;

namespace RagSystemFrontend.UI.Controllers;


[Authorize(Policy = "PlatformAuth")]
public class SpacesController(ISpacesApiClient spacesApiClient) : BaseController
{
    public async Task<IActionResult> Index()
    {
        var result = await spacesApiClient.GetSpacesAsync();
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index), "Home");
        }

        return View(result.Data ?? []);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSpaceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }

        var result = await spacesApiClient.CreateSpaceAsync(new CreateSpaceRequest
        {
            Name = model.Name,
            Plan = model.Plan,
            AdminEmail = model.CustomCredentials ? model.AdminEmail : null,
            AdminPassword = model.CustomCredentials ? model.AdminPassword : null,
        });
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = $"Space \"{result.Data?.DisplayName}\" creato.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rename(RenameSpaceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Nome Space non valido.";
            return RedirectToAction(nameof(Index));
        }

        var result = await spacesApiClient.RenameSpaceAsync(model.Id, new RenameSpaceRequest(model.Name));
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = "Space rinominato.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disable(string id)
    {
        var result = await spacesApiClient.DisableSpaceAsync(id);
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = "Space disabilitato.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Select(string id)
    {
        var result = await spacesApiClient.SelectSpaceAsync(id);
        if (!result.Success || result.Data is null)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        await AuthSignInHelper.SignInTenantAsync(HttpContext, result.Data, User.Email() ?? "", keepPlatformClaims: true);
        TempData["Success"] = $"Spazio \"{result.Data.TenantSlug}\" selezionato.";
        return RedirectToAction("Index", "Home");
    }


}
