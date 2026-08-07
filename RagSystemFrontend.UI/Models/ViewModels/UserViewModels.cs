using System.ComponentModel.DataAnnotations;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "L'email è obbligatoria.")]
    [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
    public string Email { get; set; } = "";

    [Display(Name = "Nome completo")]
    public string? FullName { get; set; }

    // Il backend richiede minimo 12 caratteri (password_min_length) -> 400 altrimenti.
    [Required(ErrorMessage = "La password è obbligatoria.")]
    [MinLength(12, ErrorMessage = "La password deve avere almeno 12 caratteri.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public string Role { get; set; } = "user";
}
