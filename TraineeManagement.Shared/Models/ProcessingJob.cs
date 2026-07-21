using TraineeManagement.Shared.Enums;
using System.Text.Json.Serialization;
namespace TraineeManagement.Shared.Models
{
    public class ProcessingJob
    {
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
}

