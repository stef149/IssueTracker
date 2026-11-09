using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Api
{
    public class AssignIssueRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "userId este obligatoriu si trebuie sa fie pozitiv.")]
        public int UserId { get; set; }
    }
}
