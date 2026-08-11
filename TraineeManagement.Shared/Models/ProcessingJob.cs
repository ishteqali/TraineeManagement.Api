using TraineeManagement.Shared.Enums;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
namespace TraineeManagement.Shared.Models
{
    public class ProcessingJob
    {
        public ProcessingJob()
        {
        }

        [SetsRequiredMembers]
        public ProcessingJob(int submissionId, Submission submission, int submissionFileId, SubmissionFile submissionFile, Guid messageId, Guid correlationId, ProcessingStatus status)
        {
            SubmissionId = submissionId;
            Submission = submission;
            SubmissionFileId = submissionFileId;
            SubmissionFile = submissionFile;
            MessageId = messageId;
            CorrelationId = correlationId;
            Status = status;
            Attempts = 0;
            CreatedAt = DateTime.UtcNow;
        }
        public int Id { get; set; }

        public int SubmissionId { get; set; }
        [JsonIgnore]
        public Submission Submission { get; set; } = null!;
        public int SubmissionFileId { get; set; }
        [JsonIgnore]
        public SubmissionFile SubmissionFile { get; set; } = null!;
        public Guid MessageId { get; set; }

        public Guid CorrelationId { get; set; }

        public ProcessingStatus Status { get; set; }

        public int Attempts { get; set; }

        public string? ErrorSummary { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    internal class setsRequiredMembersAttribute : Attribute
    {
    }
}

