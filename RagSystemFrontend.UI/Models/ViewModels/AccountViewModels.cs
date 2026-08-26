using System.ComponentModel.DataAnnotations;

namespace RagSystemFrontend.UI.Models.ViewModels;

public class LoginTenantViewModel
{
    [Required(ErrorMessage = "L'email è obbligatoria.")]
    [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La password è obbligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Lo slug dell'ufficio è obbligatorio.")]
    [Display(Name = "Ufficio (slug)")]
    public string TenantSlug { get; set; } = "";
}

public class LoginPlatformViewModel
{
    [Required(ErrorMessage = "L'email è obbligatoria.")]
    [EmailAddress(ErrorMessage = "Inserisci un'email valida.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La password è obbligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class AccountLoginViewModel
{
    public LoginTenantViewModel Tenant { get; set; } = new();
    public LoginPlatformViewModel Platform { get; set; } = new();
    public bool Expired { get; set; }
}
