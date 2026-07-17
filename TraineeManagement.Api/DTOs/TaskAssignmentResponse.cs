namespace TraineeManagement.Api.DTOs
{
    public class TaskAssignmentResponse
    {
        public int Id { get; set; }

        public int TraineeId { get; set; }
        public string? TraineeName { get; set; }

        public int MentorId { get; set; }
        public string? MentorName { get; set; }

        public int LearningTaskId { get; set; }
        public string? TaskTitle { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public required string Status { get; set; }
        public string? Remarks { get; set; }
    }
}