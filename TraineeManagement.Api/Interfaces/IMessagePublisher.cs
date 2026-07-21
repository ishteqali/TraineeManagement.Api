using TraineeManagement.Shared.Contracts;

namespace TraineeManagement.Api.Interfaces
{
    public interface IMessagePublisher
    {
        Task<bool> PublishSubmissionProcessingAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken = default);
    }
}

