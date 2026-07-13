using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Enums;
using BCrypt.Net;

namespace TraineeManagement.Api.Data
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(user => user.Role).HasConversion<string>(); // Store UserRole enum as string in the database
            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique(); // for unique username
            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique(); // for unique email

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                Role = UserRole.Admin,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            });


        }
    }
}