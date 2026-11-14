using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class IssueQuery
    {
        [EnumDataType(typeof(Status), ErrorMessage = "Status invalid. Valori permise: Open, InProgress, Resolved, Closed.")]
        public Status? Status { get; set; }

        [EnumDataType(typeof(Priority), ErrorMessage = "Prioritate invalida. Valori permise: Low, Medium, High, Critical.")]
        public Priority? Priority { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "assignedUserId trebuie sa fie pozitiv.")]
        public int? AssignedUserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ProjectId trebuie sa fie pozitiv.")]
        public int? ProjectId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "page trebuie sa fie pozitiv.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "pageSize trebuie sa fie intre 1 si 100.")]
        public int PageSize { get; set; } = 10;

    }
}
