namespace IssueTracker.Api
{
    public class Issue
    {
        public int Id { get; set; }                         //PK
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public int ProjectId { get; set; }                  //FK
        public Project Project { get; set; } = null!;

        public int CreatedById { get; set; }                //FK
        public User CreatedBy { get; set; } = null!;

        public int? AssignedToId { get; set; }              //FK
        public User? AssignedTo { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
