namespace TraineeManagement.Api.DTOs
{
    public class ProcessingJobResponse
    {
        public int Id { get; set; }

        public int SubmissionId { get; set; }

        public int FileId { get; set; }

        public string Status { get; set; } = string.Empty;

        public int Attempts { get; set; }

        public string? ErrorSummary { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
