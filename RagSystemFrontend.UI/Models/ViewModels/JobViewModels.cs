using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class JobsIndexViewModel
{
    public PaginatedResponse<IngestionJobDto> Jobs { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
}
