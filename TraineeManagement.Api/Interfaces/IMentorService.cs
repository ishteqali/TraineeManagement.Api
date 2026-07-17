using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface IMentorService
    {
        Task<PagedResponse<MentorResponse>> GetMentorsAsync(int pageNumber, int pageSize, string? search, string? status);
        Task<MentorResponse?> GetMentorByIdAsync(int id);
        Task<MentorResponse> AddMentorAsync(CreateMentorRequest request);
        Task<MentorResponse?> UpdateMentorAsync(int id, UpdateMentorRequest request);
        Task<bool> DeleteMentorAsync(int id);
    }
}