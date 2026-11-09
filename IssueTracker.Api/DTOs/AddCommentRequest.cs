using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class AddCommentRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "userId este obligatoriu si trebuie sa fie pozitiv.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Textul comentariului este obligatoriu.")]
        [StringLength(1000, ErrorMessage = "Comentariul poate avea maximum 1000 de caractere.")]
        public string Text { get; set; } = string.Empty;
    }
}
