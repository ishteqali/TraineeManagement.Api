using Microsoft.EntityFrameworkCore;
using TraineeManagement.Shared.Models;
using TraineeManagement.Shared.Enums;
using BCrypt.Net;

namespace TraineeManagement.Shared.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<LearningTask> LearningTasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<SubmissionFile> SubmissionFiles { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(user => user.Role).HasConversion<string>(); // Store UserRole enum as string in the database
            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique(); // for unique username
            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique(); // for unique email

            string adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "admin";
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@gmail.com";
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin@123";

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminPassword);


            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = adminUsername,
                Email = adminEmail,
                PasswordHash = hashedPassword,
                Role = UserRole.Admin,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            });


        }
    }
}