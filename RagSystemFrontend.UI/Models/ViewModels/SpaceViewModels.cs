using System.ComponentModel.DataAnnotations;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class CreateSpaceViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Il nome è obbligatorio.")]
    [StringLength(255, MinimumLength = 1)]
    [Display(Name = "Nome ufficio")]
    public string Name { get; set; } = "";

    // Visibile/modificabile solo per i superadmin (vedi Spaces/Index.cshtml); per tutti gli
    // altri resta "starter" e il backend rifiuta comunque un valore diverso.
    public string Plan { get; set; } = "starter";

    [Display(Name = "Credenziali admin personalizzate")]
    public bool CustomCredentials { get; set; }

    [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
    [Display(Name = "Email admin")]
    public string? AdminEmail { get; set; }

    // Il backend richiede minimo 12 caratteri (password_min_length) -> 400 altrimenti.
    [MinLength(12, ErrorMessage = "La password deve avere almeno 12 caratteri.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password admin")]
    public string? AdminPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!CustomCredentials) yield break;
        if (string.IsNullOrWhiteSpace(AdminEmail))
            yield return new ValidationResult(
                "Email admin obbligatoria con credenziali personalizzate.", new[] { nameof(AdminEmail) });
        if (string.IsNullOrWhiteSpace(AdminPassword))
            yield return new ValidationResult(
                "Password admin obbligatoria con credenziali personalizzate.", new[] { nameof(AdminPassword) });
    }
}

public class RenameSpaceViewModel
{
    [Required]
    public string Id { get; set; } = "";

    [Required(ErrorMessage = "Il nome è obbligatorio.")]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = "";
}
