using TraineeManagement.Api.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace TraineeManagement.Api.Models
{
    public class LearningTask
    {
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