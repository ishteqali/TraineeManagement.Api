using System.Diagnostics.CodeAnalysis;
using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Shared.Models
{
    public class TaskAssignment
    {
        public TaskAssignment()
        {
        }

        [SetsRequiredMembers]
        public TaskAssignment(int traineeId, Trainee trainee, int mentorId, Mentor mentor, int learningTaskId, LearningTask learningTask, DateTime assignedDate, DateTime dueDate, TaskAssignmentStatus status, string? remarks)
        {
            TraineeId = traineeId;
            Trainee = trainee;
            MentorId = mentorId;
            Mentor = mentor;
            LearningTaskId = learningTaskId;
            LearningTask = learningTask;
            AssignedDate = assignedDate;
            DueDate = dueDate;
            Status = status;
            Remarks = remarks;
        }
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