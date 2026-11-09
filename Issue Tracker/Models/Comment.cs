using System;
using System.Collections.Generic;
using System.Text;

namespace Issue_Tracker
{
    internal class Comment
    {
        public User User { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
