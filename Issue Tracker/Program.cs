using Issue_Tracker;

INotificationService notifier = new ConsoleNotificationService();

var service = new IssueTrackerService(notifier);
var report = new IssueReportService(service);

Titlu("CREARE DATE DE TEST");

// ---- 6 useri cu roluri diferite ----
var ana    = service.AddUser("Ana Popescu",    "ana@netrom.ro",    Role.Developer);
var mihai  = service.AddUser("Mihai Ionescu",  "mihai@netrom.ro",  Role.Developer);
var elena  = service.AddUser("Elena Dumitru",  "elena@netrom.ro",  Role.Developer);
var andrei = service.AddUser("Andrei Stan",    "andrei@netrom.ro", Role.Developer);
var ioana  = service.AddUser("Ioana Marin",    "ioana@netrom.ro",  Role.Manager);
var radu   = service.AddUser("Radu Georgescu", "radu@netrom.ro",   Role.Admin);

// ---- 2 proiecte ----
var web    = service.CreateProject("Website Magazin",  "Magazinul online al firmei");
var mobile = service.CreateProject("Aplicatie Mobila", "Aplicatia pentru Android si iOS");

// ---- membri (Ana NU e in proiectul mobil, Andrei NU e in proiectul web) ----
foreach (var u in new[] { ana, mihai, elena, ioana, radu })
    service.AddMemberToProject(web.Id, u.Id);

foreach (var u in new[] { andrei, elena, mihai, ioana, radu })
    service.AddMemberToProject(mobile.Id, u.Id);

// ---- 15 issues----
//                     proiect  creator  titlu                                   prioritate          
var i1  = NewIssue(web,    ioana,  "Login nu functioneaza",                Priority.Critical, 20, -5);
var i2  = NewIssue(web,    ioana,  "Cosul se goleste dupa refresh",        Priority.High,     15,  5);
var i3  = NewIssue(web,    ana,    "Filtrare produse dupa pret",           Priority.Medium,   12, 10);
var i4  = NewIssue(web,    ioana,  "Pagina de contact",                    Priority.Low,      10, 20);
var i5  = NewIssue(web,    radu,   "Imagini lipsa la produse",             Priority.High,      8, -1);
var i6  = NewIssue(web,    ioana,  "Optimizare viteza homepage",           Priority.Medium,    6, 14);
var i7  = NewIssue(web,    mihai,  "Footer afisat gresit pe mobil",        Priority.Low,       5,  7);
var i8  = NewIssue(web,    ioana,  "Newsletter nu trimite confirmare",     Priority.Medium,    3, 12);
var i9  = NewIssue(mobile, ioana,  "Crash la pornire pe Android",          Priority.Critical, 18, -3);
var i10 = NewIssue(mobile, radu,   "Notificari push",                      Priority.High,     14,  6);
var i11 = NewIssue(mobile, elena,  "Dark mode",                            Priority.Low,      11, 30);
var i12 = NewIssue(mobile, ioana,  "Sincronizare offline",                 Priority.High,      9,  9);
var i13 = NewIssue(mobile, ioana,  "Ecran de profil",                      Priority.Medium,    7, 15);
var i14 = NewIssue(mobile, andrei, "Traduceri lipsa in romana",            Priority.Low,       4, 10);
var i15 = NewIssue(mobile, radu,   "Login cu Google",                      Priority.Critical,  2,  8);

// ---- asignari (i15 ramane neasignat) ----
service.AssignIssue(i1.Id,  ana.Id);
service.AssignIssue(i2.Id,  mihai.Id);
service.AssignIssue(i3.Id,  elena.Id);
service.AssignIssue(i4.Id,  ana.Id);
service.AssignIssue(i5.Id,  mihai.Id);
service.AssignIssue(i6.Id,  elena.Id);
service.AssignIssue(i7.Id,  ana.Id);
service.AssignIssue(i8.Id,  mihai.Id);
service.AssignIssue(i9.Id,  andrei.Id);
service.AssignIssue(i10.Id, elena.Id);
service.AssignIssue(i11.Id, mihai.Id);
service.AssignIssue(i12.Id, andrei.Id);
service.AssignIssue(i13.Id, andrei.Id);
service.AssignIssue(i14.Id, elena.Id);

// ---- 20 comentarii (adaugate INAINTE de inchiderea issue-urilor) ----
var comments = new (Issue Issue, User User, string Text)[]
{
    (i1,  ana,    "Reproduc problema doar in Chrome."),
    (i1,  ioana,  "Este prioritar, clientii nu se pot loga."),
    (i1,  ana,    "Am gasit cauza: token-ul expira prea repede."),
    (i2,  mihai,  "Problema apare la sesiunile anonime."),
    (i2,  elena,  "Confirm si eu bug-ul."),
    (i3,  elena,  "Am adaugat filtrul min/max."),
    (i3,  ioana,  "Arata bine, il inchidem dupa review."),
    (i4,  ana,    "Formularul trimite acum email."),
    (i5,  mihai,  "Lipsesc imaginile pentru 30 de produse."),
    (i5,  radu,   "Verifica si CDN-ul."),
    (i6,  elena,  "Am redus timpul de incarcare la 1.2s."),
    (i7,  ana,    "Am corectat CSS-ul pentru ecrane mici."),
    (i9,  andrei, "Crash-ul apare doar pe Android 12."),
    (i9,  ioana,  "Termenul a trecut, te rog un update."),
    (i9,  andrei, "Lucrez la fix, mai am nevoie de o zi."),
    (i10, elena,  "Notificarile merg pe ambele platforme."),
    (i11, mihai,  "Dark mode respecta setarea sistemului."),
    (i12, andrei, "Datele se sincronizeaza la reconectare."),
    (i13, andrei, "Ecranul de profil e gata."),
    (i14, elena,  "Am tradus 80% din texte."),
};

foreach (var c in comments)
    service.AddComment(c.Issue.Id, c.User.Id, c.Text);

// ---- statusuri diferite ----
MoveTo(i1,  Status.InProgress, ana);
MoveTo(i2,  Status.Resolved,   mihai);
MoveTo(i3,  Status.Closed,     ioana);   // Manager -> poate inchide
MoveTo(i4,  Status.Closed,     radu);    // Admin   -> poate inchide
MoveTo(i6,  Status.Resolved,   elena);
MoveTo(i7,  Status.Resolved,   ana);
MoveTo(i9,  Status.InProgress, andrei);
MoveTo(i10, Status.Closed,     ioana);
MoveTo(i11, Status.Resolved,   mihai);
MoveTo(i12, Status.Resolved,   andrei);
MoveTo(i13, Status.Closed,     radu);
MoveTo(i14, Status.InProgress, elena);
// i5, i8, i15 raman Open

// ---- o reasignare valida ----
service.AssignIssue(i8.Id, elena.Id);

Console.WriteLine();
Console.WriteLine($"Creat: {service.Users.Count} useri, {service.Projects.Count} proiecte, " +
                  $"{service.Issues.Count} issues, {service.Issues.Sum(i => i.Comments.Count)} comentarii.");

// Reguli de business

Titlu("REGULI DE BUSINESS");

Incearca("Asignare catre un user care nu e in proiect",
    () => service.AssignIssue(i8.Id, andrei.Id));

Incearca("Trecere directa din Open in Closed",
    () => service.ChangeIssueStatus(i15.Id, Status.Closed, ioana.Id));

Incearca("Intoarcere la un status anterior (Resolved -> InProgress)",
    () => service.ChangeIssueStatus(i2.Id, Status.InProgress, mihai.Id));

Incearca("Un Developer incearca sa inchida un issue",
    () => service.ChangeIssueStatus(i2.Id, Status.Closed, mihai.Id));

Incearca("Comentariu pe un issue Closed",
    () => service.AddComment(i3.Id, elena.Id, "Inca o modificare..."));

Incearca("Reasignare pe un issue Closed",
    () => service.AssignIssue(i4.Id, mihai.Id));

Incearca("DueDate inainte de CreatedAt",
    () => service.CreateIssue(web.Id, ioana.Id, "Test", "", Priority.Low, DateTime.Now.AddDays(-2)));

Incearca("Issue fara titlu",
    () => service.CreateIssue(web.Id, ioana.Id, "   ", "", Priority.Low, DateTime.Now.AddDays(5)));

Incearca("Acelasi user adaugat de doua ori in proiect",
    () => service.AddMemberToProject(web.Id, ana.Id));

Incearca("Issue inexistent",
    () => service.AssignIssue(999, ana.Id));

//  CAUTARI
Titlu("CAUTARI");

Console.WriteLine($"Issues asignate lui {ana.Name}:");
foreach (var issue in service.GetIssuesForUser(ana.Id))
    Console.WriteLine("   " + Descriere(issue));

Console.WriteLine($"\nIssues din proiectul {mobile.Name}:");
foreach (var issue in service.GetIssuesForProject(mobile.Id))
    Console.WriteLine("   " + Descriere(issue));

Console.WriteLine("\nIssues cu prioritate Critical:");
foreach (var issue in service.GetIssuesByPriority(Priority.Critical))
    Console.WriteLine("   " + Descriere(issue));

// 5. RAPOARTE

Titlu("RAPOARTE");

Console.WriteLine("Numar de issues per status:");
foreach (var pair in report.GetIssueCountByStatus())
    Console.WriteLine($"   {pair.Key}: {pair.Value}");

Console.WriteLine("\nNumar de issues per developer:");
foreach (var pair in report.GetIssueCountByDeveloper())
    Console.WriteLine($"   {pair.Key.Name}: {pair.Value}");

Console.WriteLine("\nIssues overdue:");
foreach (var issue in report.GetOverdueIssues())
    Console.WriteLine($"   {Descriere(issue)} (termen: {issue.DueDate:dd.MM.yyyy})");

Console.WriteLine("\nTop 3 utilizatori dupa issues rezolvate:");
foreach (var entry in report.GetTopResolvers())
    Console.WriteLine($"   {entry.User.Name}: {entry.ResolvedCount}");

var average = report.GetAverageResolutionTime();
Console.WriteLine(average == null
    ? "\nTimp mediu de rezolvare: nu exista issues rezolvate."
    : $"\nTimp mediu de rezolvare: {average.Value.Days} zile si {average.Value.Hours} ore");

Console.WriteLine("\nIssues grupate dupa prioritate:");
foreach (var group in report.GetIssuesGroupedByPriority())
{
    Console.WriteLine($"   {group.Key}:");
    foreach (var issue in group.Value)
        Console.WriteLine("      " + Descriere(issue));
}

// FUNCTII AJUTATOARE (local functions)

Issue NewIssue(Project project, User creator, string title, Priority priority, int createdDaysAgo, int dueInDays)
{
    return service.CreateIssue(project.Id, creator.Id, title, $"Descriere: {title}", priority,
                               DateTime.Now.AddDays(dueInDays), DateTime.Now.AddDays(-createdDaysAgo));
}

// Muta un issue pas cu pas pana la statusul dorit (respectand fluxul)
void MoveTo(Issue issue, Status target, User changedBy)
{
    while (issue.Status < target)
        service.ChangeIssueStatus(issue.Id, issue.Status + 1, changedBy.Id);
}

// Ruleaza o operatie si afiseaza daca a fost refuzata si de ce
void Incearca(string descriere, Action actiune)
{
    Console.WriteLine($"\n- {descriere}");
    try
    {
        actiune();
        Console.WriteLine("   PERMIS (nu ar fi trebuit!)");
    }
    catch (NotFoundException ex)
    {
        Console.WriteLine($"   [NU EXISTA] {ex.Message}");
    }
    catch (BusinessRuleException ex)
    {
        Console.WriteLine($"   [REGULA INCALCATA] {ex.Message}");
    }
}

string Descriere(Issue issue)
{
    return $"#{issue.Id} [{issue.Status}, {issue.Priority}] {issue.Title} - {issue.AssignedTo?.Name ?? "neasignat"}";
}

void Titlu(string text)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 60));
    Console.WriteLine(" " + text);
    Console.WriteLine(new string('=', 60));
}
