using System.ComponentModel.DataAnnotations;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class CollectionsIndexViewModel
{
    public PaginatedResponse<CollectionDto> Collections { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CreateCollectionViewModel
{
    [Required(ErrorMessage = "Il nome è obbligatorio.")]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }
}
