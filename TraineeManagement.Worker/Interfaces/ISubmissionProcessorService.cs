using TraineeManagement.Shared.Contracts;
using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Worker.Interfaces
{
    public interface ISubmissionProcessorService
    {
        Task<ProcessingResultStatus> ProcessAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken);
    }
}