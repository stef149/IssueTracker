using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Api
{
    public class IssueTrackerService
    {
        // datele se citesc si salveaza prin DBContext
        private readonly IssueTrackerDbContext _context;
        private readonly INotificationService _notificationService;

        public IssueTrackerService(IssueTrackerDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }


        // PROIECTII (entitate -> DTO, traduse de EF in SQL)


        private static readonly Expression<Func<Issue, IssueResponse>> ToIssueResponse = i => new IssueResponse
        {
            Id = i.Id,
            ProjectId = i.ProjectId,
            Title = i.Title,
            Description = i.Description,
            Status = i.Status,
            Priority = i.Priority,
            CreatedAt = i.CreatedAt,
            DueDate = i.DueDate,
            ResolvedAt = i.ResolvedAt,
            IsOverdue = i.DueDate < DateTime.Now
                        && i.Status != Status.Resolved
                        && i.Status != Status.Closed,
            CreatedBy = new UserSummaryResponse
            {
                Id = i.CreatedBy.Id,
                Name = i.CreatedBy.Name,
                Role = i.CreatedBy.Role
            },
            AssignedTo = i.AssignedTo == null
                ? null
                : new UserSummaryResponse
                {
                    Id = i.AssignedTo.Id,
                    Name = i.AssignedTo.Name,
                    Role = i.AssignedTo.Role
                },
            Comments = i.Comments
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponse
                {
                    User = new UserSummaryResponse
                    {
                        Id = c.User.Id,
                        Name = c.User.Name,
                        Role = c.User.Role
                    },
                    Text = c.Text,
                    CreatedAt = c.CreatedAt
                })
                .ToList()
        };

        private static readonly Expression<Func<Project, ProjectResponse>> ToProjectResponse = p => new ProjectResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Members = p.Members
                .OrderBy(m => m.Id)
                .Select(m => new UserSummaryResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Role = m.Role
                })
                .ToList(),
            IssueCount = p.Issues.Count()
        };


        // CITIRI (read-only- nu se modifica nimic)


        public async Task<List<UserResponse>> GetUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<List<ProjectResponse>> GetProjectsAsync()
        {
            return await _context.Projects
                .OrderBy(p => p.Id)
                .Select(ToProjectResponse)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<ProjectResponse> GetProjectByIdAsync(int projectId)
        {
            return await _context.Projects
                .Where(p => p.Id == projectId)
                .Select(ToProjectResponse)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException($"Proiectul cu Id {projectId} nu exista.");
        }

        public async Task<IssueResponse> GetIssueByIdAsync(int issueId)
        {
            return await _context.Issues
                .Where(i => i.Id == issueId)
                .Select(ToIssueResponse)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException($"Issue-ul cu Id {issueId} nu exista.");
        }

        public async Task<List<IssueResponse>> GetIssuesForProjectAsync(int projectId, Status? status, Priority? priority, int? assignedUserId)
        {
            if (!await _context.Projects.AnyAsync(p => p.Id == projectId))
                throw new NotFoundException($"Proiectul cu Id {projectId} nu exista.");

            IQueryable<Issue> query = _context.Issues.Where(i => i.ProjectId == projectId);

            if (status != null)
                query = query.Where(i => i.Status == status);

            if (priority != null)
                query = query.Where(i => i.Priority == priority);

            if (assignedUserId != null)
                query = query.Where(i => i.AssignedToId == assignedUserId);

            return await query
                .OrderBy(i => i.Id)
                .Select(ToIssueResponse)
                .ToListAsync();
        }

        public async Task<List<IssueResponse>> GetIssuesForUserAsync(int userId)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
                throw new NotFoundException($"Userul cu Id {userId} nu exista.");

            return await _context.Issues
                .Where(i => i.AssignedToId == userId)
                .OrderBy(i => i.Id)
                .Select(ToIssueResponse)
                .ToListAsync();
        }

        public async Task<List<IssueResponse>> GetOverdueIssuesAsync()
        {
            var now = DateTime.Now;

            return await _context.Issues
                .Where(i => i.DueDate < now
                         && i.Status != Status.Resolved
                         && i.Status != Status.Closed)
                .OrderBy(i => i.DueDate)
                .Select(ToIssueResponse)
                .ToListAsync();
        }

        public async Task<PagedResponse<IssueResponse>> GetIssuesAsync(IssueQuery filter)
        {
            IQueryable<Issue> query = _context.Issues;

            if (filter.ProjectId != null)
                query = query.Where(i => i.ProjectId == filter.ProjectId);

            if (filter.Status != null)
                query = query.Where(i => i.Status == filter.Status);

            if (filter.Priority != null)
                query = query.Where(i => i.Priority == filter.Priority);

            if (filter.AssignedUserId != null)
                query = query.Where(i => i.AssignedToId == filter.AssignedUserId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(i => i.Id)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(ToIssueResponse)
                .ToListAsync();

            return new PagedResponse<IssueResponse>
            {
                Items = items,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }


        // USERI


        public async Task<UserResponse> AddUserAsync(string name, string email, Role role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException("Numele userului nu poate fi gol.");

            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new BusinessRuleException($"Exista deja un user cu emailul {email}.");

            var user = new User
            {
                Name = name,
                Email = email,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();  // aici SQL Server genereaza Id-ul

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }


        // PROIECTE


        public async Task<ProjectResponse> CreateProjectAsync(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException("Numele proiectului nu poate fi gol. ");

            var project = new Project
            {
                Name = name,
                Description = description
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return await GetProjectByIdAsync(project.Id);
        }

        public async Task AddMemberToProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId)
                ?? throw new NotFoundException($"Proiectul cu Id {projectId} nu exista.");

            var user = await GetUserAsync(userId);

            if (project.Members.Any(m => m.Id == userId))
                throw new BusinessRuleException($"{user.Name} este deja membru in proiectul {project.Name}.");

            project.Members.Add(user);

            await _context.SaveChangesAsync();

            _notificationService.Notify(user, $"Ai fost adaugat in proiectul {project.Name}.");
        }


        // ISSUES


        public async Task<IssueResponse> CreateIssueAsync(int projectId, int createdByUserId, string title, string description,
                                                          Priority priority, DateTime dueDate)
        {
            var project = await GetProjectAsync(projectId);
            var creator = await GetUserAsync(createdByUserId);

            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleException("Titlul issue-ului nu poate fi gol.");

            var created = DateTime.Now;

            if (dueDate.Date < created.Date)
                throw new BusinessRuleException("DueDate nu poate fi inainte de data crearii.");

            var issue = new Issue
            {
                ProjectId = project.Id,
                CreatedBy = creator,
                Title = title,
                Description = description,
                Status = Status.Open,
                Priority = priority,
                CreatedAt = created,
                DueDate = dueDate
            };

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            return await GetIssueByIdAsync(issue.Id);
        }

        public async Task<IssueResponse> AssignIssueAsync(int issueId, int userId)
        {
            var issue = await GetIssueForUpdateAsync(issueId);
            var user = await GetUserAsync(userId);

            EnsureNotClosed(issue);

            var isMember = await _context.Projects
                .AnyAsync(p => p.Id == issue.ProjectId && p.Members.Any(m => m.Id == userId));

            if (!isMember)
                throw new BusinessRuleException(
                    $"{user.Name} nu face parte din proiectul acestui issue, deci nu poate primi issue-ul #{issue.Id}.");

            issue.AssignedTo = user;

            await _context.SaveChangesAsync();

            _notificationService.Notify(user, $"Ti-a fost atribuit issue-ul #{issue.Id} \"{issue.Title}\".");

            return await GetIssueByIdAsync(issue.Id);
        }

        public async Task<IssueResponse> ChangeIssueStatusAsync(int issueId, Status newStatus, int changedByUserId)
        {
            var issue = await GetIssueForUpdateAsync(issueId);
            var changedBy = await GetUserAsync(changedByUserId);

            EnsureNotClosed(issue);

            if (newStatus != issue.Status + 1)
                throw new BusinessRuleException(
                    $"Tranzitie invalida pentru issue-ul #{issue.Id}: din {issue.Status} nu se poate trece in {newStatus}. " +
                    $"Urmatorul status permis este {issue.Status + 1}.");

            if (newStatus == Status.Closed && changedBy.Role != Role.Manager && changedBy.Role != Role.Admin)
                throw new BusinessRuleException(
                    $"{changedBy.Name} are rolul {changedBy.Role}. Doar un Manager sau Admin poate inchide un issue.");

            var oldStatus = issue.Status;
            issue.Status = newStatus;

            if (newStatus == Status.Resolved)
                issue.ResolvedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            if (issue.AssignedTo != null)
                _notificationService.Notify(issue.AssignedTo,
                    $"Issue-ul #{issue.Id} \"{issue.Title}\" a trecut din {oldStatus} in {newStatus}.");

            return await GetIssueByIdAsync(issue.Id);
        }


        // COMENTARII


        public async Task<CommentResponse> AddCommentAsync(int issueId, int userId, string text)
        {
            var issue = await GetIssueForUpdateAsync(issueId);
            var user = await GetUserAsync(userId);

            EnsureNotClosed(issue);

            if (string.IsNullOrWhiteSpace(text))
                throw new BusinessRuleException("Textul comentariului nu poate fi gol.");

            var comment = new Comment
            {
                IssueId = issue.Id,
                User = user,
                Text = text,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            if (issue.AssignedTo != null && issue.AssignedTo.Id != user.Id)
                _notificationService.Notify(issue.AssignedTo,
                    $"{user.Name} a comentat la issue-ul #{issue.Id}: \"{text}\"");

            return new CommentResponse
            {
                User = new UserSummaryResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.Role
                },
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };
        }


        // METODE AJUTATOARE


        private async Task<Issue> GetIssueForUpdateAsync(int issueId)
        {
            return await _context.Issues
                .Include(i => i.AssignedTo)
                .FirstOrDefaultAsync(i => i.Id == issueId)
                ?? throw new NotFoundException($"Issue-ul cu Id {issueId} nu exista.");
        }

        private async Task<User> GetUserAsync(int userId)
        {
            return await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException($"Userul cu Id {userId} nu exista.");
        }

        private async Task<Project> GetProjectAsync(int projectId)
        {
            return await _context.Projects.FindAsync(projectId)
                ?? throw new NotFoundException($"Proiectul cu Id {projectId} nu exista.");
        }

        private void EnsureNotClosed(Issue issue)
        {
            if (issue.Status == Status.Closed)
                throw new BusinessRuleException($"Issue-ul #{issue.Id} este Closed si nu mai poate fi modificat.");
        }
    }
}
