using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class LearningTaskResponse
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ExpectedTechStack { get; set; }
        public DateTime DueDate { get; set; }
        public required string Status { get; set; }
    }
}