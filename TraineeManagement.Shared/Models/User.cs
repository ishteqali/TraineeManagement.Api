using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }

        public required string PasswordHash { get; set; }
        public required UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}