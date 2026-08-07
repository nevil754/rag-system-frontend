using System.ComponentModel.DataAnnotations;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class CreateSpaceViewModel
{
    [Required(ErrorMessage = "Il nome è obbligatorio.")]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = "";
}

public class RenameSpaceViewModel
{
    [Required]
    public string Id { get; set; } = "";

    [Required(ErrorMessage = "Il nome è obbligatorio.")]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = "";
}
