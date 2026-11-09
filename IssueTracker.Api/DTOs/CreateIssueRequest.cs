using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class CreateIssueRequest
    {
        [Required(ErrorMessage = "Titlul este obligatoriu.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Titlul trebuie sa aiba intre 3 si 200 de caractere.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Descrierea poate avea maximum 2000 de caractere.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prioritatea este obligatorie.")]
        [EnumDataType(typeof(Priority), ErrorMessage = "Prioritate invalida. Valori permise: Low, Medium, High, Critical.")]
        public Priority? Priority { get; set; }

        [Required(ErrorMessage = "DueDate este obligatoriu.")]
        public DateTime? DueDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "createdByUserId este obligatoriu si trebuie sa fie pozitiv.")]
        public int CreatedByUserId { get; set; }
    }
}
