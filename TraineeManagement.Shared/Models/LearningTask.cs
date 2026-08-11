using TraineeManagement.Shared.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
namespace TraineeManagement.Shared.Models
{
    public class LearningTask
    {
        public LearningTask()
        {
        }

        [SetsRequiredMembers]
        public LearningTask(string title, string description, string expectedTechStack, DateTime dueDate, LearningTaskStatus status)
        {
            Title = title;
            Description = description;
            ExpectedTechStack = expectedTechStack;
            DueDate = dueDate;
            Status = status;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ExpectedTechStack { get; set; }
        public DateTime DueDate { get; set; }
        public LearningTaskStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [JsonIgnore]
        public ICollection<TaskAssignment> TaskAssingments { get; set; } = new List<TaskAssignment>();
    }
}