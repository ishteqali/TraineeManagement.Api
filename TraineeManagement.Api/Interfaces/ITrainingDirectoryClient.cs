using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITrainingDirectoryClient
    {
        Task<TraineeProfileResponse?> GetTraineeAsync(int traineeId, string correlationId, CancellationToken cancellationToken);
    }
}

