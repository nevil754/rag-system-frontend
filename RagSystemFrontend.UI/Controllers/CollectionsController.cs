using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;

namespace RagSystemFrontend.UI.Controllers;

[Authorize(Policy = "TenantAuth")]
public class CollectionsController(ICollectionsApiClient collectionsApiClient) : BaseController
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await collectionsApiClient.GetCollectionsAsync(page, pageSize);
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index), "Home");
        }

        return View(new CollectionsIndexViewModel { Collections = result.Data ?? new(), Page = page, PageSize = pageSize });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCollectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Nome collezione non valido.";
            return RedirectToAction(nameof(Index));
        }

        var result = await collectionsApiClient.CreateAsync(new CreateCollectionRequest { Name = model.Name, Description = model.Description });
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = $"Collezione \"{result.Data?.Name}\" creata.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await collectionsApiClient.DeleteAsync(id);
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = "Collezione eliminata.";
        return RedirectToAction(nameof(Index));
    }
}
