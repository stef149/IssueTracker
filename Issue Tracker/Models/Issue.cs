using System;
using System.Collections.Generic;
using System.Text;

namespace Issue_Tracker
{
    internal class Issue
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public User CreatedBy { get; set; } = null!;
        public User? AssignedTo { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public DateTime? ResolvedAt { get; set; }
    }
}
