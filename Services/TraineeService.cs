using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Enums;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<TraineeResponse>> GetAllTraineesAsync(string? search = null)
        {
            var query = _context.Trainees.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                string searchTermLower = search.ToLower();
                query = query.Where(t =>
                    t.FirstName.ToLower().Contains(searchTermLower) ||
                    t.LastName.ToLower().Contains(searchTermLower) ||
                    t.Email.ToLower().Contains(searchTermLower) ||
                    t.TechStack.ToLower().Contains(searchTermLower)
                );
            }
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
            Trainee newTrainee = new Trainee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = request.Status,
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
            trainee.Status = request.Status;
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