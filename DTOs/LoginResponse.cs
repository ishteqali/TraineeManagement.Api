namespace TraineeManagement.Api.DTOs
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public int ExpiresIn { get; set; }
        public required UserResponse User { get; set; }
    }
}