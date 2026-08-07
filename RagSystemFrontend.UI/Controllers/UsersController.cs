using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;

namespace RagSystemFrontend.UI.Controllers;

[Authorize(Policy = "TenantAdmin")]
public class UsersController(IUsersApiClient usersApiClient) : BaseController
{
    public async Task<IActionResult> Index()
    {
        var result = await usersApiClient.GetUsersAsync();
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index), "Home");
        }

        return View(result.Data ?? []);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }

        var result = await usersApiClient.CreateAsync(new CreateUserRequest
        {
            Email = model.Email,
            FullName = model.FullName,
            Password = model.Password,
            Role = model.Role,
        });

        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = $"Utente \"{result.Data?.Email}\" creato.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await usersApiClient.DeleteAsync(id);
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = "Utente disattivato.";
        return RedirectToAction(nameof(Index));
    }
}
