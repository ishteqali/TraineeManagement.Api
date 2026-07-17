namespace TraineeManagement.Api.DTOs
{
    public class SubmissionAcceptedResponse
    {
        public Guid TrackingId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}

