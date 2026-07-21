using System;
using System.Text.Json.Serialization;
using TraineeManagement.Shared.Enums;
 
namespace TraineeManagement.Shared.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int SubmissionId { get; set; }
        [JsonIgnore]
        public required Submission Submission { get; set; }

        public int MentorId { get; set; }
        [JsonIgnore]
        public required Mentor Mentor { get; set; }
 
        public required string Feedback { get; set; }
        public int? Score { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
        public DateTime ReviewedDate { get; set; }
    }
}