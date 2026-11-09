namespace Issue_Tracker
{
    internal class IssueTrackerService
    {
        // DI prin constructor
        private readonly INotificationService _notificationService;

        private readonly List<User> _users = new List<User>();
        private readonly List<Project> _projects = new List<Project>();

        private int _nextUserId = 1;
        private int _nextProjectId = 1;
        private int _nextIssueId = 1;

        public IssueTrackerService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Acces doar pentru citire din afara
        public IReadOnlyList<User> Users
        {
            get { return _users; }
        }

        public IReadOnlyList<Project> Projects
        {
            get { return _projects; }
        }

        // Toate issue-urile 
        public IReadOnlyList<Issue> Issues
        {
            get { return _projects.SelectMany(p => p.Issues).ToList(); }
        }

        //  USERI 

        public User AddUser(string name, string email, Role role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException("Numele userului nu poate fi gol.");

            if (_users.Any(u => u.Email == email))
                throw new BusinessRuleException($"Exista deja un user cu emailul {email}.");

            var user = new User
            {
                Id = _nextUserId++,
                Name = name,
                Email = email,
                Role = role
            };

            _users.Add(user);
            return user;
        }

        //  PROIECTe

        public Project CreateProject(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException("Numele proiectului nu poate fi gol.");

            var project = new Project
            {
                Id = _nextProjectId++,
                Name = name,
                Description = description
            };

            _projects.Add(project);
            return project;
        }

        public void AddMemberToProject(int projectId, int userId)
        {
            var project = GetProject(projectId);
            var user = GetUser(userId);

            if (project.Members.Contains(user))
                throw new BusinessRuleException($"{user.Name} este deja membru in proiectul {project.Name}.");

            project.Members.Add(user);
            _notificationService.Notify(user, $"Ai fost adaugat in proiectul {project.Name}.");
        }

        // ISSUES 

        
        public Issue CreateIssue(int projectId, int createdByUserId, string title, string description,
                                 Priority priority, DateTime dueDate, DateTime? createdAt = null)
        {
            var project = GetProject(projectId);
            var creator = GetUser(createdByUserId);

            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleException("Titlul issue-ului nu poate fi gol.");

            var created = createdAt ?? DateTime.Now;

           
            if (dueDate.Date < created.Date)
                throw new BusinessRuleException("DueDate nu poate fi inainte de data crearii.");

            var issue = new Issue
            {
                Id = _nextIssueId++,
                ProjectId = project.Id,
                Title = title,
                Description = description,
                Status = Status.Open,
                Priority = priority,
                CreatedAt = created,
                DueDate = dueDate,
                CreatedBy = creator
            };

            project.Issues.Add(issue);
            return issue;
        }

        public void AssignIssue(int issueId, int userId)
        {
            var issue = GetIssue(issueId);
            var user = GetUser(userId);
            var project = GetProject(issue.ProjectId);

            EnsureNotClosed(issue);

            if (!project.Members.Contains(user))
                throw new BusinessRuleException(
                    $"{user.Name} nu face parte din proiectul {project.Name}, deci nu poate primi issue-ul #{issue.Id}.");

            issue.AssignedTo = user;
            _notificationService.Notify(user, $"Ti-a fost atribuit issue-ul #{issue.Id} \"{issue.Title}\".");
        }

        // STATUS 

        public void ChangeIssueStatus(int issueId, Status newStatus, int changedByUserId)
        {
            var issue = GetIssue(issueId);
            var changedBy = GetUser(changedByUserId);

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

            if (issue.AssignedTo != null)
                _notificationService.Notify(issue.AssignedTo,
                    $"Issue-ul #{issue.Id} \"{issue.Title}\" a trecut din {oldStatus} in {newStatus}.");
        }

        // COMENTARII 

        public Comment AddComment(int issueId, int userId, string text)
        {
            var issue = GetIssue(issueId);
            var user = GetUser(userId);

            EnsureNotClosed(issue);

            if (string.IsNullOrWhiteSpace(text))
                throw new BusinessRuleException("Textul comentariului nu poate fi gol.");

            var comment = new Comment
            {
                User = user,
                Text = text,
                CreatedAt = DateTime.Now
            };

            issue.Comments.Add(comment);

            if (issue.AssignedTo != null && issue.AssignedTo != user)
                _notificationService.Notify(issue.AssignedTo,
                    $"{user.Name} a comentat la issue-ul #{issue.Id}: \"{text}\"");

            return comment;
        }

        // CAUTARI 

        public List<Issue> GetIssuesForUser(int userId)
        {
            var user = GetUser(userId);
            return Issues.Where(i => i.AssignedTo == user).ToList();
        }

        public List<Issue> GetIssuesForProject(int projectId)
        {
            var project = GetProject(projectId);
            return project.Issues.ToList();
        }

        public List<Issue> GetIssuesByPriority(Priority priority)
        {
            return Issues.Where(i => i.Priority == priority).ToList();
        }

        // Overdue 
        public List<Issue> GetOverdueIssues()
        {
            return Issues
                .Where(i => i.DueDate < DateTime.Now
                         && i.Status != Status.Resolved
                         && i.Status != Status.Closed)
                .ToList();
        }

        private User GetUser(int userId)
        {
            return _users.FirstOrDefault(u => u.Id == userId)
                ?? throw new NotFoundException($"Userul cu Id {userId} nu exista.");
        }

        private Project GetProject(int projectId)
        {
            return _projects.FirstOrDefault(p => p.Id == projectId)
                ?? throw new NotFoundException($"Proiectul cu Id {projectId} nu exista.");
        }

        private Issue GetIssue(int issueId)
        {
            return Issues.FirstOrDefault(i => i.Id == issueId)
                ?? throw new NotFoundException($"Issue-ul cu Id {issueId} nu exista.");
        }

        // un issue Closed nu mai poate fi modificat
        private void EnsureNotClosed(Issue issue)
        {
            if (issue.Status == Status.Closed)
                throw new BusinessRuleException($"Issue-ul #{issue.Id} este Closed si nu mai poate fi modificat.");
        }
    }
}
