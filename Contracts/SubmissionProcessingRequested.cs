namespace TraineeManagement.Api.Contracts
{
    public class SubmissionProcessingRequested
    {
        public Guid MessageId { get; init; }
        public Guid CorrelationId { get; init; }
        public int SubmissionId { get; init; }
        public int FileId { get; init; }
        public DateTime RequestedAt { get; init; }
        public string ContractVersion { get; init; } = "1.0";
    }
}

