using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Numele proiectului este obligatoriu.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Numele trebuie sa aiba intre 3 si 100 de caractere.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrierea poate avea maximum 500 de caractere.")]
        public string Description { get; set; } = string.Empty;
    }
}
