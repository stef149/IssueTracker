namespace IssueTracker.Api
{
    public class IssueResponse
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public bool IsOverdue { get; set; }
        public UserSummaryResponse CreatedBy { get; set; } = null!;
        public UserSummaryResponse? AssignedTo { get; set; }
        public List<CommentResponse> Comments { get; set; } = new List<CommentResponse>();

        public static IssueResponse FromIssue(Issue issue)
        {
            return new IssueResponse
            {
                Id = issue.Id,
                ProjectId = issue.ProjectId,
                Title = issue.Title,
                Description = issue.Description,
                Status = issue.Status,
                Priority = issue.Priority,
                CreatedAt = issue.CreatedAt,
                DueDate = issue.DueDate,
                ResolvedAt = issue.ResolvedAt,
                IsOverdue = issue.DueDate < DateTime.Now
                            && issue.Status != Status.Resolved
                            && issue.Status != Status.Closed,
                CreatedBy = UserSummaryResponse.FromUser(issue.CreatedBy),
                AssignedTo = issue.AssignedTo == null ? null : UserSummaryResponse.FromUser(issue.AssignedTo),
                Comments = issue.Comments.Select(CommentResponse.FromComment).ToList()
            };
        }
    }
}
