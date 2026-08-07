using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;

namespace RagSystemFrontend.UI.Controllers;


[Authorize(Policy = "PlatformAuth")]
public class AdminTenantsController(ITenantsApiClient tenantsApiClient) : BaseController
{
    public async Task<IActionResult> Index()
    {
        var result = await tenantsApiClient.GetTenantsAsync();
        if (!result.Success)
        {
            if (result.IsForbidden)
            {
                return View("AccessDenied");
            }
            return await HandleFailureAsync(result, nameof(Index), "Home");
        }
        return View(result.Data ?? []);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTenantViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }
        var result = await tenantsApiClient.CreateAsync(new CreateTenantRequest
        {
            Slug = model.Slug,
            DisplayName = model.DisplayName,
            Plan = model.Plan,
            AdminEmail = string.IsNullOrWhiteSpace(model.AdminEmail) ? null : model.AdminEmail,
            AdminPassword = string.IsNullOrWhiteSpace(model.AdminPassword) ? null : model.AdminPassword,
        });
        if (!result.Success)
        {
            if (result.IsForbidden)
            {
                return View("AccessDenied");
            }

            return await HandleFailureAsync(result, nameof(Index));
        }
        TempData["Success"] = $"Tenant \"{result.Data?.Slug}\" creato.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disable(string slug)
    {
        var result = await tenantsApiClient.DisableAsync(slug);
        if (!result.Success)
        {
            if (result.IsForbidden)
            {
                return View("AccessDenied");
            }

            return await HandleFailureAsync(result, nameof(Index));
        }
        TempData["Success"] = result.Data?.Message ?? "Tenant disabilitato.";
        return RedirectToAction(nameof(Index));
    }



}
