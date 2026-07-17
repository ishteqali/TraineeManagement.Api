using TraineeManagement.Api.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace TraineeManagement.Api.Models
{
    public class Mentor
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Expertise { get; set; }
        public MentorStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [JsonIgnore]
        public ICollection<TaskAssignment> TaskAssingments { get; set; } = new List<TaskAssignment>();
    }
}