using TraineeManagement.Shared.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
namespace TraineeManagement.Shared.Models
{
    public class Mentor
    {
        public Mentor()
        {
        }
        
        [SetsRequiredMembers]
        public Mentor(string firstName, string lastName, string email, string expertise, MentorStatus status)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Expertise = expertise;
            Status = status;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }
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