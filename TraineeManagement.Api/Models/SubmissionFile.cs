namespace TraineeManagement.Api.Models
{
    public class SubmissionFile
    {
        public int Id { get; set; }

        public int SubmissionId { get; set; }

        public Submission Submission { get; set; } = null!;

        public string OriginalFileName { get; set; } = string.Empty;

        public string StorageFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string? Checksum { get; set; }

        public int UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}