using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class ChangeIssueStatusRequest
    {
        [Required(ErrorMessage = "Statusul este obligatoriu.")]
        [EnumDataType(typeof(Status), ErrorMessage = "Status invalid. Valori permise: Open, InProgress, Resolved, Closed.")]
        public Status? Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "changedByUserId este obligatoriu si trebuie sa fie pozitiv.")]
        public int ChangedByUserId { get; set; }
    }
}
