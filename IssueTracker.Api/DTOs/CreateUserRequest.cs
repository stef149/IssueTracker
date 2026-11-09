using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Numele este obligatoriu.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie sa aiba intre 2 si 100 de caractere.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Emailul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Emailul nu are o forma valida.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rolul este obligatoriu.")]
        [EnumDataType(typeof(Role), ErrorMessage = "Rol invalid. Valori permise: Developer, Manager, Admin.")]
        public Role? Role { get; set; }
    }
}
