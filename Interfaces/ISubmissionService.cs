using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ISubmissionService
    {
        Task<SubmissionResponse> CreateSubmissionAsync(CreateSubmissionRequest request);
        Task<IEnumerable<SubmissionResponse>> GetAllSubmissionsAsync();
        Task<SubmissionResponse?> GetSubmissionByIdAsync(int id);
    }

}