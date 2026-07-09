using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITraineeService
    {
        Task<List<TraineeResponse>> GetAllTraineesAsync(string? search = null);
        Task<TraineeResponse?> GetTraineeByIdAsync(int id);
        Task<TraineeResponse> AddTraineeAsync(CreateTraineeRequest request);
        Task<bool> UpdateTraineeAsync(int id, UpdateTraineeRequest request);
        Task<bool> DeleteTraineeAsync(int id);
    }
}