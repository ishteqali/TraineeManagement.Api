using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public UserRole Role { get; set; }
    }
}