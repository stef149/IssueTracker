using System;
using System.Collections.Generic;
using System.Text;

namespace Issue_Tracker
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}
