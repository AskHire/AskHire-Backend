using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AskHire_Backend.Data.Entities
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Application> Applies { get; set; }
        public DbSet<JobRole> JobRoles { get; set; }

        public DbSet<Reminder> Reminder { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Applications)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<JobRole>()
                .HasMany(j => j.Vacancies)
                .WithOne(v => v.JobRole)
                .HasForeignKey(v => v.JobId);

            modelBuilder.Entity<JobRole>()
                .HasMany(j => j.Questions)
                .WithOne(q => q.JobRole)
                .HasForeignKey(q => q.JobId);

            modelBuilder.Entity<Vacancy>()
                .HasMany(v => v.Applies)
                .WithOne(a => a.Vacancy)
                .HasForeignKey(a => a.VacancyId);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Interview)
                .WithOne(i => i.Application)
                .HasForeignKey<Interview>(i => i.ApplicationId);
        }
    }
}

