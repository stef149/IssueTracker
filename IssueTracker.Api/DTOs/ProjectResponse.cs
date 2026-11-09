namespace IssueTracker.Api
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<UserSummaryResponse> Members { get; set; } = new List<UserSummaryResponse>();
        public int IssueCount { get; set; }

        public static ProjectResponse FromProject(Project project)
        {
            return new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Members = project.Members.Select(UserSummaryResponse.FromUser).ToList(),
                IssueCount = project.Issues.Count
            };
        }
    }
}
