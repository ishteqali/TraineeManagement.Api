using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITaskAssignmentService
    {
        Task<TaskAssignmentResponse> CreateAssignmentAsync(CreateTaskAssignmentRequest request);
        Task<IEnumerable<TaskAssignmentResponse>> GetAllAssignmentAsnyc();
        Task<TaskAssignmentResponse?> GetAssignmentByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
    }
}