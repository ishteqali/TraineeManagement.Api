namespace TraineeManagement.Api.DTOs
{
    public class SubmissionFileResponse
    {
        public int Id { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedDate { get; set; }
    }
}

