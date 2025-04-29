using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Data.Entities
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Application> Applies { get; set; }
        public DbSet<JobRole> JobRoles { get; set; }
        public DbSet<Reminder> Reminder { get; set; }
    }
}
