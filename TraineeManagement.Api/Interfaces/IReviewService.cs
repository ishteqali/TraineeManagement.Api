using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeManagement.Api.DTOs;
 
namespace TraineeManagement.Api.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateReviewAsync(CreateReviewRequest request);
        Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync();
        Task<ReviewResponse?> GetReviewByIdAsync(int id);
    }
}