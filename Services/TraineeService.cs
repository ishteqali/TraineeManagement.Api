using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Enums;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Services
{
    public class TraineeService : ITraineeService
    {
        private readonly AppDbContext _context;
        public TraineeService(AppDbContext context)
        {
            _context = context;
        }

        private TraineeResponse MapToResponse(Trainee trainee)
        {
            return new TraineeResponse
            {
                Id = trainee.Id,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                TechStack = trainee.TechStack,
                Status = trainee.Status.ToString()
            };
        }

        public IQueryable<Trainee> SearchQuery(string? searchTerm)
        {
            IQueryable<Trainee>? query = _context.Trainees.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                query = query.Where(trainee =>
                    trainee.FirstName.ToLower().Contains(searchTermLower) ||
                    trainee.LastName.ToLower().Contains(searchTermLower) ||
                    trainee.Email.ToLower().Contains(searchTermLower) ||
                    trainee.TechStack.ToLower().Contains(searchTermLower)
                );
            }
            return query;
        }

        public async Task<List<TraineeResponse>> GetAllTraineesAsync(string? search = null)
        {
            IQueryable<Trainee>? query = SearchQuery(search);
            List<Trainee> trainees = await query.ToListAsync();
            return trainees.Select(MapToResponse).ToList();
        }

        public async Task<TraineeResponse?> GetTraineeByIdAsync(int id)
        {
            Trainee? trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return null;
            return MapToResponse(trainee);
        }

        public async Task<TraineeResponse> AddTraineeAsync(CreateTraineeRequest request)
        {
            Trainee newTrainee = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = Enum.Parse<TraineeStatus>(request.Status!.ToString(), ignoreCase: true),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _context.Trainees.AddAsync(newTrainee);
            await _context.SaveChangesAsync();
            return MapToResponse(newTrainee);
        }

        public async Task<bool> UpdateTraineeAsync(int id, UpdateTraineeRequest request)
        {
            Trainee? trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return false;

            trainee.FirstName = request.FirstName;
            trainee.LastName = request.LastName;
            trainee.Email = request.Email;
            trainee.TechStack = request.TechStack;
            trainee.Status = Enum.Parse<TraineeStatus>(request.Status!.ToString(), ignoreCase: true);
            trainee.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTraineeAsync(int id)
        {
            Trainee? trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return false;

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}