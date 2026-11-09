namespace IssueTracker.Api
{
    public class CommentResponse
    {
        public UserSummaryResponse User { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public static CommentResponse FromComment(Comment comment)
        {
            return new CommentResponse
            {
                User = UserSummaryResponse.FromUser(comment.User),
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
