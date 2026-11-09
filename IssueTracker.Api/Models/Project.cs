using System;
using System.Collections.Generic;
using System.Text;

namespace IssueTracker.Api
{
    public class Project
    {
        public int Id { get; set; }                             //PK
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<User> Members { get; set; } = new List<User>();
        public List<Issue> Issues { get; set; } = new List<Issue>();
    }
}
