using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Api
{
    // Rapoartele calculate direct in baza de date (GROUP BY, COUNT)
    public class IssueReportService
    {
        private readonly IssueTrackerDbContext _context;

        public IssueReportService(IssueTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<Status, int>> GetIssueCountByStatusAsync()
        {
            return await _context.Issues
                .GroupBy(i => i.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<Dictionary<int, int>> GetIssueCountByAssignedUserAsync()
        {
            return await _context.Issues
                .Where(i => i.AssignedToId != null)
                .GroupBy(i => i.AssignedToId!.Value)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);
        }

        public async Task<TimeSpan?> GetAverageResolutionTimeAsync()
        {
            var resolved = await _context.Issues
                .Where(i => i.ResolvedAt != null)
                .Select(i => new { i.CreatedAt, i.ResolvedAt })
                .ToListAsync();

            if (resolved.Count == 0)
                return null;

            double averageHours = resolved.Average(i => (i.ResolvedAt!.Value - i.CreatedAt).TotalHours);
            return TimeSpan.FromHours(averageHours);
        }
    }
}
