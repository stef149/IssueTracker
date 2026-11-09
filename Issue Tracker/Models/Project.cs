using System;
using System.Collections.Generic;
using System.Text;

namespace Issue_Tracker
{
    internal class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<User> Members { get; set; } = new List<User>();
        public List<Issue> Issues { get; set; } = new List<Issue>();
    }
}
