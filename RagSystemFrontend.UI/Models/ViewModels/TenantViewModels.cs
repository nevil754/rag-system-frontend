using System.ComponentModel.DataAnnotations;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class CreateTenantViewModel
{
    // Lo slug non si scrive più a mano: viene generato dal backend a partire da questo
    // nome, esattamente come per la creazione self-service da "I miei Space".
    [Required(ErrorMessage = "Il nome è obbligatorio.")]
    [StringLength(255, MinimumLength = 1)]
    [Display(Name = "Nome ufficio")]
    public string DisplayName { get; set; } = "";

    public string Plan { get; set; } = "starter";

    [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
    [Display(Name = "Email admin (opzionale)")]
    public string? AdminEmail { get; set; }

    // Il backend richiede minimo 12 caratteri (password_min_length) -> 400 altrimenti.
    [MinLength(12, ErrorMessage = "La password deve avere almeno 12 caratteri.")]
    [Display(Name = "Password admin (opzionale)")]
    [DataType(DataType.Password)]
    public string? AdminPassword { get; set; }
}
