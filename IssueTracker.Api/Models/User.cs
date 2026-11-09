using System;
using System.Collections.Generic;
using System.Text;

namespace IssueTracker.Api
{
    public class User
    {
        public int Id { get; set; }                         //PK
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}
