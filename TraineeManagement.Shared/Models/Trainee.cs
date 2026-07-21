using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TraineeManagement.Shared.Enums;

namespace TraineeManagement.Shared.Models
{
    public class Trainee
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string TechStack { get; set; }
        public required TraineeStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [JsonIgnore]
        public ICollection<TaskAssignment> TaskAssingments { get; set; } = new List<TaskAssignment>();
    }
}
