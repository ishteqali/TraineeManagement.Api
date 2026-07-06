using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services
{
    public class TraineeService : ITraineeService
    {
        private static List<Trainee> _trainees = new List<Trainee>
        {
            new Trainee
            {
                Id = 1,
                FirstName = "Ishteqali",
                LastName = "Khan",
                Email = "ishteqali.khan@zeuslearning.com",
                TechStack = ".NET",
                Status = "Active",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            },
            new Trainee
            {
                Id = 2,
                FirstName = "Jowin",
                LastName = "Paulose",
                Email = "jowin.paulose@zeuslearning.com",
                TechStack = ".NET",
                Status = "Active",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }
        };
        private static int _nextId = _trainees.Count;
        
        private TraineeResponse MapToResponse(Trainee trainee)
        {
            return new TraineeResponse
            {
                Id = trainee.Id,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                TechStack = trainee.TechStack,
                Status = trainee.Status
            };
        }

        public List<TraineeResponse> GetAllTrainees()
        {
            return _trainees.Select(MapToResponse).ToList();
        }
        
        public TraineeResponse GetTraineeById(int id)
        {
            var trainee = _trainees.FirstOrDefault(t => t.Id == id);
            if(trainee == null) return null;
            return MapToResponse(trainee);
        }

        public TraineeResponse AddTrainee(CreateTraineeRequest request)
        {
            var newTrainee = new Trainee
            {
                Id = _nextId++,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = request.Status,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _trainees.Add(newTrainee);
            return MapToResponse(newTrainee);
        }

        public bool UpdateTrainee(int id, UpdateTraineeRequest request)
        {
            var trainee = _trainees.FirstOrDefault(t => t.Id == id);
            if(trainee == null) return false;

            trainee.FirstName = request.FirstName;
            trainee.LastName = request.LastName;
            trainee.Email = request.Email;
            trainee.TechStack = request.TechStack;
            trainee.Status = request.Status;
            trainee.UpdatedDate = DateTime.UtcNow;

            return true;
        }

        public bool DeleteTrainee(int id)
        {
            var trainee = _trainees.FirstOrDefault(t => t.Id == id);
            if(trainee == null) return false;
            _trainees.Remove(trainee);
            return true;
        }

    }
}