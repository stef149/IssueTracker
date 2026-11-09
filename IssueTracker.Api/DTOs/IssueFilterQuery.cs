using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class IssueFilterQuery
    {
        [EnumDataType(typeof(Status), ErrorMessage = "Status invalid. Valori permise: Open, InProgress, Resolved, Closed.")]
        public Status? Status { get; set; }

        [EnumDataType(typeof(Priority), ErrorMessage = "Prioritate invalida. Valori permise: Low, Medium, High, Critical.")]
        public Priority? Priority { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "assignedUserId trebuie sa fie pozitiv.")]
        public int? AssignedUserId { get; set; }
    }
}
