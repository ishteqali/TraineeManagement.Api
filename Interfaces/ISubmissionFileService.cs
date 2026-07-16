using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ISubmissionFileService
    {
        Task<SubmissionFileResponse> UploadAsync(int submissionId, UploadSubmissionFileRequest request, int uploadedBy,
            CancellationToken cancellationToken = default);

        Task<SubmissionFileResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<DownloadSubmissionFileResponse> DownloadAsync(int id, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

