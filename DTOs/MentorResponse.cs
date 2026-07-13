namespace TraineeManagement.Api.DTOs
{
    public class MentorResponse
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Expertise { get; set; }
        public required string Status { get; set; }
    }
}