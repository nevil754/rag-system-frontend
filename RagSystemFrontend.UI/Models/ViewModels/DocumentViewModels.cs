using System.ComponentModel.DataAnnotations;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class DocumentsIndexViewModel
{
    public PaginatedResponse<DocumentDto> Documents { get; set; } = new();
    public List<CollectionDto> Collections { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CollectionId { get; set; }
    public string? StatusFilter { get; set; }
}

public class DocumentUploadViewModel
{
    [Required(ErrorMessage = "Seleziona un file da caricare.")]
    public IFormFile? File { get; set; }

    public string? CollectionId { get; set; }
}
