using System;
using System.Collections.Generic;
using System.Text;

namespace IssueTracker.Api
{
    public class Comment
    {
        public int Id { get; set; }                     //PK
        public int UserId { get; set; }                 //FK
        public User User { get; set; } = null!;
        public int IssueId { get; set; }                //FK
        public Issue Issue { get; set; } = null!;

        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}
