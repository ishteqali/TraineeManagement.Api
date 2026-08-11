using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace TraineeManagement.Shared.Models
{
    public class SubmissionFile
    {
        public SubmissionFile()
        {
        }


        [SetsRequiredMembers]
        public SubmissionFile(int submissionId, Submission submission, string originalFileName, string storageFileName, string contentType, long fileSize, string? checksum, int uploadedBy, DateTime uploadedDate)
        {
            SubmissionId = submissionId;
            Submission = submission;
            OriginalFileName = originalFileName;
            StorageFileName = storageFileName;
            ContentType = contentType;
            FileSize = fileSize;
            Checksum = checksum;
            UploadedBy = uploadedBy;
            UploadedDate = uploadedDate;
        }
        public int Id { get; set; }

        public int SubmissionId { get; set; }

        public Submission Submission { get; set; } = null!;

        public string OriginalFileName { get; set; } = string.Empty;

        public string StorageFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string? Checksum { get; set; }

        public int UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        [JsonIgnore]
        public ProcessingJob? ProcessingJob { get; set; }

    }
}