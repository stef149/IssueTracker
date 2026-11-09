namespace IssueTracker.Api
{
    public class UserSummaryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Role Role { get; set; }

        public static UserSummaryResponse FromUser(User user)
        {
            return new UserSummaryResponse
            {
                Id = user.Id,
                Name = user.Name,
                Role = user.Role
            };
        }
    }
}
