using System.ComponentModel.DataAnnotations;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class CreateTenantViewModel
{
    [Required(ErrorMessage = "Lo slug è obbligatorio.")]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Usa solo lettere minuscole, numeri e trattini.")]
    public string Slug { get; set; } = "";

    [Required(ErrorMessage = "Il nome è obbligatorio.")]
    [Display(Name = "Nome visualizzato")]
    public string DisplayName { get; set; } = "";

    public string Plan { get; set; } = "starter";

    [Display(Name = "Email admin (opzionale)")]
    public string? AdminEmail { get; set; }

    [Display(Name = "Password admin (opzionale)")]
    [DataType(DataType.Password)]
    public string? AdminPassword { get; set; }
}
