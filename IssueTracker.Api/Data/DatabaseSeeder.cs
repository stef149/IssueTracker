using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Api
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IssueTrackerDbContext context)
        {
            // daca exista deja useri, baza a fost populata -> nu mai adaugam nimic
            if (await context.Users.AnyAsync())
                return;

            var now = DateTime.Now;

            // ---------------- USERI ----------------

            var ana    = new User { Name = "Ana Popescu",    Email = "ana@netrom.ro",    Role = Role.Developer };
            var mihai  = new User { Name = "Mihai Ionescu",  Email = "mihai@netrom.ro",  Role = Role.Developer };
            var elena  = new User { Name = "Elena Dumitru",  Email = "elena@netrom.ro",  Role = Role.Developer };
            var andrei = new User { Name = "Andrei Stan",    Email = "andrei@netrom.ro", Role = Role.Developer };
            var ioana  = new User { Name = "Ioana Marin",    Email = "ioana@netrom.ro",  Role = Role.Manager };
            var radu   = new User { Name = "Radu Georgescu", Email = "radu@netrom.ro",   Role = Role.Admin };

            // ---------------- PROIECTE ----------------
            // Members = randurile din tabelul intermediar ProjectMembers

            var web = new Project
            {
                Name = "Website Magazin",
                Description = "Magazinul online al firmei",
                Members = new List<User> { ana, mihai, elena, ioana, radu }
            };

            var mobile = new Project
            {
                Name = "Aplicatie Mobila",
                Description = "Aplicatia pentru Android si iOS",
                Members = new List<User> { andrei, elena, mihai, ioana, radu }
            };

            var admin = new Project
            {
                Name = "Panou Administrare",
                Description = "Aplicatia interna pentru administrarea comenzilor",
                Members = new List<User> { ana, andrei, ioana, radu }
            };

            // ---------------- ISSUES ----------------
            //                proiect  creator  asignat  titlu                                prioritate         status             creat acum X zile  termen peste Y zile

            var i1  = NewIssue(web,    ioana,  ana,    "Login nu functioneaza",            Priority.Critical, Status.InProgress, 20, -5);
            var i2  = NewIssue(web,    ioana,  mihai,  "Cosul se goleste dupa refresh",    Priority.High,     Status.Resolved,   15,  5);
            var i3  = NewIssue(web,    ana,    elena,  "Filtrare produse dupa pret",       Priority.Medium,   Status.Closed,     12, 10);
            var i4  = NewIssue(web,    ioana,  ana,    "Pagina de contact",                Priority.Low,      Status.Closed,     10, 20);
            var i5  = NewIssue(web,    radu,   mihai,  "Imagini lipsa la produse",         Priority.High,     Status.Open,        8, -1);
            var i6  = NewIssue(web,    ioana,  elena,  "Optimizare viteza homepage",       Priority.Medium,   Status.Resolved,    6, 14);
            var i7  = NewIssue(web,    mihai,  ana,    "Footer afisat gresit pe mobil",    Priority.Low,      Status.Resolved,    5,  7);
            var i8  = NewIssue(web,    ioana,  elena,  "Newsletter nu trimite confirmare", Priority.Medium,   Status.Open,        3, 12);
            var i9  = NewIssue(mobile, ioana,  andrei, "Crash la pornire pe Android",      Priority.Critical, Status.InProgress, 18, -3);
            var i10 = NewIssue(mobile, radu,   elena,  "Notificari push",                  Priority.High,     Status.Closed,     14,  6);
            var i11 = NewIssue(mobile, elena,  mihai,  "Dark mode",                        Priority.Low,      Status.Resolved,   11, 30);
            var i12 = NewIssue(mobile, ioana,  andrei, "Sincronizare offline",             Priority.High,     Status.Resolved,    9,  9);
            var i13 = NewIssue(mobile, ioana,  andrei, "Ecran de profil",                  Priority.Medium,   Status.Closed,      7, 15);
            var i14 = NewIssue(mobile, andrei, elena,  "Traduceri lipsa in romana",        Priority.Low,      Status.InProgress,  4, 10);
            var i15 = NewIssue(mobile, radu,   null,   "Login cu Google",                  Priority.Critical, Status.Open,        2,  8);

            var i16 = NewIssue(admin,  radu,   andrei, "Export comenzi in Excel",          Priority.High,     Status.InProgress, 16, -4);
            var i17 = NewIssue(admin,  ioana,  ana,    "Raport vanzari lunare",            Priority.Medium,   Status.Open,       10, -2);
            var i18 = NewIssue(admin,  ioana,  null,   "Roluri si permisiuni",             Priority.Critical, Status.Open,        6,  9);
            var i19 = NewIssue(admin,  ana,    andrei, "Cautare dupa client",              Priority.Low,      Status.Resolved,   13,  3);
            var i20 = NewIssue(admin,  radu,   ana,    "Istoric modificari comanda",       Priority.Medium,   Status.Closed,     21,  2);
            var i21 = NewIssue(admin,  andrei, null,   "Paginare in lista de comenzi",     Priority.Low,      Status.Open,        1, 20);

            // ---------------- COMENTARII ----------------

            AddComment(i1,  ana,    "Reproduc problema doar in Chrome.");
            AddComment(i1,  ioana,  "Este prioritar, clientii nu se pot loga.");
            AddComment(i1,  ana,    "Am gasit cauza: token-ul expira prea repede.");
            AddComment(i2,  mihai,  "Problema apare la sesiunile anonime.");
            AddComment(i3,  ioana,  "Arata bine, il inchidem dupa review.");
            AddComment(i5,  radu,   "Verifica si CDN-ul.");
            AddComment(i9,  andrei, "Crash-ul apare doar pe Android 12.");
            AddComment(i9,  ioana,  "Termenul a trecut, te rog un update.");
            AddComment(i10, elena,  "Notificarile merg pe ambele platforme.");
            AddComment(i14, elena,  "Am tradus 80% din texte.");

            AddComment(i16, andrei, "Exportul merge, mai lipseste formatarea datelor.");
            AddComment(i16, radu,   "Termenul a trecut, avem nevoie de el pentru contabilitate.");
            AddComment(i18, ioana,  "Trebuie discutat intai cu echipa de securitate.");
            AddComment(i20, ana,    "Istoricul se salveaza la fiecare schimbare de status.");

            // ---------------- SALVARE ----------------
            // Adaugam proiectele: EF adauga automat si tot ce e legat de ele
            // (userii din Members, issue-urile din Issues, comentariile din Comments).
            // Un singur SaveChangesAsync pentru tot.

            context.Projects.AddRange(web, mobile, admin);
            await context.SaveChangesAsync();

            // ---------------- FUNCTII AJUTATOARE ----------------

            Issue NewIssue(Project project, User creator, User? assignee, string title,
                           Priority priority, Status status, int createdDaysAgo, int dueInDays)
            {
                var createdAt = now.AddDays(-createdDaysAgo);

                var issue = new Issue
                {
                    Project = project,
                    CreatedBy = creator,
                    AssignedTo = assignee,
                    Title = title,
                    Description = $"Descriere: {title}",
                    Priority = priority,
                    Status = status,
                    CreatedAt = createdAt,
                    DueDate = now.AddDays(dueInDays),
                    // issue-urile Resolved si Closed au fost rezolvate la jumatatea perioadei
                    ResolvedAt = status == Status.Resolved || status == Status.Closed
                        ? createdAt.AddDays(createdDaysAgo / 2.0)
                        : null
                };

                project.Issues.Add(issue);
                return issue;
            }

            void AddComment(Issue issue, User user, string text)
            {
                issue.Comments.Add(new Comment
                {
                    User = user,
                    Text = text,
                    CreatedAt = now
                });
            }
        }
    }
}
