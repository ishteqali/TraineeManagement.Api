using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITraineeService
    {
        Task<PagedResponse<TraineeResponse>> GetTraineesAsync(int pageNumber, int pageSize, string? search, string? status);
        Task<TraineeResponse?> GetTraineeByIdAsync(int id);
        Task<TraineeResponse> AddTraineeAsync(CreateTraineeRequest request);
        Task<TraineeResponse?> UpdateTraineeAsync(int id, UpdateTraineeRequest request);
        Task<bool> DeleteTraineeAsync(int id);
    }
}