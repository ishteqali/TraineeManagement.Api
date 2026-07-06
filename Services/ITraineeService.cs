using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Services
{
    public interface ITraineeService
    {
        List<TraineeResponse> GetAllTrainees();
        TraineeResponse GetTraineeById(int id);
        TraineeResponse AddTrainee(CreateTraineeRequest request);
        bool UpdateTrainee(int id, UpdateTraineeRequest request);
        bool DeleteTrainee(int id);
    }
}