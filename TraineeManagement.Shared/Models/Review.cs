using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TraineeManagement.Shared.Enums;

namespace TraineeManagement.Shared.Models
{
    public class Review
    {
        public Review()
        {
        }

        [SetsRequiredMembers]
        public Review(int submissionId, Submission submission, int mentorId, Mentor mentor, string feedback, int? score, ReviewStatus reviewStatus, DateTime reviewedDate)
        {
            SubmissionId = submissionId;
            Submission = submission;
            MentorId = mentorId;
            Mentor = mentor;
            Feedback = feedback;
            Score = score;
            ReviewStatus = reviewStatus;
            ReviewedDate = reviewedDate;
        }

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