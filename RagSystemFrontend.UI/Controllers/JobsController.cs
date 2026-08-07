using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;

namespace RagSystemFrontend.UI.Controllers;

[Authorize(Policy = "TenantAuth")]
public class JobsController(IJobsApiClient jobsApiClient) : BaseController
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? status = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await jobsApiClient.GetJobsAsync(page, pageSize, status);
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index), "Home");
        }

        return View(new JobsIndexViewModel { Jobs = result.Data ?? new(), Page = page, PageSize = pageSize, Status = status });
    }

    public async Task<IActionResult> Details(string id)
    {
        var result = await jobsApiClient.GetJobAsync(id);
        if (!result.Success || result.Data is null)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> Cancel(string id)
    {
        var result = await jobsApiClient.CancelAsync(id);
        if (!result.Success)
        {
            return await HandleFailureAsync(result, nameof(Index));
        }

        TempData["Success"] = "Job cancellato.";
        return RedirectToAction(nameof(Index));
    }
}
