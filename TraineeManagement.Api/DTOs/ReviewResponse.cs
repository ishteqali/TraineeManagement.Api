using System;
 
namespace TraineeManagement.Api.DTOs
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public required string SubmissionUrl { get; set; }
        public int MentorId { get; set; }
        public required string MentorName { get; set; }
        public required string Feedback { get; set; }
        public int? Score { get; set; }
        public required string ReviewStatus { get; set; }
        public DateTime ReviewedDate { get; set; }
    }
}