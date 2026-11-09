namespace Issue_Tracker
{
    
    internal class IssueReportService
    {
        private readonly IssueTrackerService _tracker;

        // Primeste serviciul principal prin constructor (dependency injection)
        // ca sa aiba acces la issue-uri
        public IssueReportService(IssueTrackerService tracker)
        {
            _tracker = tracker;
        }

        //  Numarul de issues per status
        public Dictionary<Status, int> GetIssueCountByStatus()
        {
            return _tracker.Issues
                .GroupBy(i => i.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        //  Numarul de issues per developer 
        public Dictionary<User, int> GetIssueCountByDeveloper()
        {
            return _tracker.Issues
                .Where(i => i.AssignedTo != null)
                .GroupBy(i => i.AssignedTo!)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        //  Issues overdue
        public List<Issue> GetOverdueIssues()
        {
            return _tracker.GetOverdueIssues();
        }

        //  Top 3 utilizatori cu cele mai multe issues rezolvate
        public List<(User User, int ResolvedCount)> GetTopResolvers(int count = 3)
        {
            return _tracker.Issues
                .Where(i => i.ResolvedAt != null && i.AssignedTo != null)
                .GroupBy(i => i.AssignedTo!)
                .Select(g => (User: g.Key, ResolvedCount: g.Count()))
                .OrderByDescending(x => x.ResolvedCount)
                .Take(count)
                .ToList();
        }

        // 5. Timpul mediu de rezolvare
        public TimeSpan? GetAverageResolutionTime()
        {
            var resolvedIssues = _tracker.Issues
                .Where(i => i.ResolvedAt != null)
                .ToList();

            if (resolvedIssues.Count == 0)
                return null;

            double averageHours = resolvedIssues
                .Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalHours);

            return TimeSpan.FromHours(averageHours);
        }

        // 6. Issues grupate dupa prioritate
        public Dictionary<Priority, List<Issue>> GetIssuesGroupedByPriority()
        {
            return _tracker.Issues
                .GroupBy(i => i.Priority)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
