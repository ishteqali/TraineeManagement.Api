using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ILearningTaskService
    {
        Task<PagedResponse<LearningTaskResponse>> GetLearningTasksAsync(int pageNumber, int pageSize, string? search, string? status);
        Task<LearningTaskResponse?> GetLearningTaskByIdAsync(int id);
        Task<LearningTaskResponse> AddLearningTaskAsync(CreateLearningTaskRequest request);
        Task<LearningTaskResponse?> UpdateLearningTaskAsync(int id, UpdateLearningTaskRequest request);
        Task<bool> DeleteLearningTaskAsync(int id);
    }
}