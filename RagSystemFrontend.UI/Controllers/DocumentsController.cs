using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;

namespace RagSystemFrontend.UI.Controllers;

[Authorize(Policy = "TenantAuth")]
public class DocumentsController(
    IDocumentsApiClient documentsApiClient,
    ICollectionsApiClient collectionsApiClient) : BaseController
{
    private const int MaxUploadBytes = 110_000_000;    //ingestion_max_file_mb è 100MB nel backend

    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? collectionId = null, string? statusFilter = null)
    {

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var documentsTask = documentsApiClient.GetDocumentsAsync(page, pageSize, collectionId, statusFilter);
        var collectionsTask = collectionsApiClient.GetCollectionsAsync(1, 100);
        await Task.WhenAll(documentsTask, collectionsTask);

        var documentsResult = await documentsTask;
        if (!documentsResult.Success)
        {
            return await HandleFailureAsync(documentsResult, nameof(Index), "Home");
        }

        var collectionsResult = await collectionsTask;
        if (!collectionsResult.Success && collectionsResult.IsUnauthorized)
        {
            return await HandleFailureAsync(collectionsResult, nameof(Index), "Home");
        }

        return View(new DocumentsIndexViewModel
        {
            Documents = documentsResult.Data ?? new(),
            Collections = collectionsResult.Success ? collectionsResult.Data?.Items ?? [] : [],
            Page = page,
            PageSize = pageSize,
            CollectionId = collectionId,
            StatusFilter = statusFilter,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxUploadBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes)]
    public async Task<IActionResult> Upload(DocumentUploadViewModel model)
    {
        if (model.File is null || model.File.Length == 0)
        {
            TempData["Error"] = "Seleziona un file da caricare.";
            return RedirectToAction(nameof(Index));
        }

        await using var stream = model.File.OpenReadStream();
        var result = await documentsApiClient.UploadAsync(stream, model.File.FileName, model.File.ContentType, model.CollectionId);
        if (!result.Success || result.Data is null)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = $"\"{model.File.FileName}\" caricato: in coda per l'elaborazione (job {result.Data.JobId}).";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Status(string id)
    {
        var result = await documentsApiClient.GetStatusAsync(id);
        if (!result.Success || result.Data is null)
        {
            return StatusCode(result.StatusCode == 0 ? 502 : result.StatusCode, new { error = result.ErrorMessage });
        }

        return Json(result.Data);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await documentsApiClient.DeleteAsync(id);
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = "Documento eliminato.";
        return RedirectToAction(nameof(Index));
    }

}
