using Microsoft.EntityFrameworkCore;
namespace IssueTracker.Api
{
    public class IssueTrackerDbContext : DbContext
    {
        public IssueTrackerDbContext(DbContextOptions<IssueTrackerDbContext> options) : base(options)   
        {

        }

        // DbSet-urile

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Issue> Issues { get; set; } = null!;

        // OnModelCreating 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relatia Issue(M) - Project(1)

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Project)                 //un issue are un proiect
                .WithMany(p => p.Issues)                //un proiect are multe issues
                .HasForeignKey(i => i.ProjectId)        //legatura se face prin coloana ProjectId
                .OnDelete(DeleteBehavior.Cascade);      //daca se sterge proiectul se sterg si issue-urile asignate proiectului

            // relatia Issue(M) - User(1)  CreatedById

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.CreatedBy)
                .WithMany()
                .HasForeignKey(i => i.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // relatia Issue(M) - User(1)  AssignedToId

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.AssignedTo)
                .WithMany()
                .HasForeignKey(i => i.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            // relatia Comments(M) - Issue(1)

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Issue)
                .WithMany(i => i.Comments)              
                .HasForeignKey(c => c.IssueId)          
                .OnDelete(DeleteBehavior.Cascade);

            // relatia Comment(M) - User(1)

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)          
                .OnDelete(DeleteBehavior.Restrict);

            // relatia Project(M) - User(M) - prin tabelul intermediar ProjectMembers
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Members)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectMembers",

                    // User: ProjectMembers.UserId 

                    r => r
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),

                    // Project: ProjectMembers.ProjectId 

                    l => l
                        .HasOne<Project>()
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade),

                    //PK
                    j => j.HasKey("ProjectId", "UserId"));



            // Users
            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Projects
            modelBuilder.Entity<Project>()
                .Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Project>()
                .Property(p => p.Description)
                .HasMaxLength(500);

            // Issues
            modelBuilder.Entity<Issue>()
                .Property(i => i.Title)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Issue>()
                .Property(i => i.Description)
                .HasMaxLength(2000);

            // Comments
            modelBuilder.Entity<Comment>()
                .Property(c => c.Text)
                .HasMaxLength(1000)
                .IsRequired();
        }

    }
}
