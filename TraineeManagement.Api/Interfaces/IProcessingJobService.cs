using TraineeManagement.Api.DTOs;
namespace TraineeManagement.Api.Interfaces
{
    public interface IProcessingJobService
    {
        Task<ProcessingJobResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
