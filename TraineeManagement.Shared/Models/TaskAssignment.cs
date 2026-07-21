using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Shared.Models
{
    public class TaskAssignment
    {
        public int Id { get; set; }

        public int TraineeId { get; set; }
        public required Trainee Trainee { get; set; }

        public int MentorId { get; set; }
        public required Mentor Mentor { get; set; }

        public int LearningTaskId { get; set; }
        public required LearningTask LearningTask { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public TaskAssignmentStatus Status { get; set; }
        public string? Remarks {get; set;}
    }
}