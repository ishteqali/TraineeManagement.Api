using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Enums;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TraineeManagement.Api.Services
{
    public class TraineeService : ITraineeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TraineeService> _logger;
        public TraineeService(AppDbContext context, ILogger<TraineeService> logger)
        {
            _context = context;
            _logger = logger;
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

        public IQueryable<Trainee> SearchFilterQuery(IQueryable<Trainee> query, string? searchTerm)
        {
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

        public IQueryable<Trainee> StatusFilterQuery(IQueryable<Trainee> query, string? status)
        {
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TraineeStatus>(status, true, out TraineeStatus parsedStatus))
            {
                query = query.Where(trainee => trainee.Status == parsedStatus);
            }
            return query;
        }

        public IQueryable<Trainee> AllFilterQueries(string? searchTerm, string? status)
        {
            IQueryable<Trainee> query = _context.Trainees.AsQueryable();
            query = SearchFilterQuery(query, searchTerm);
            query = StatusFilterQuery(query, status);
            return query;
        }

        public async Task<PagedResponse<TraineeResponse>> GetTraineesAsync(int pageNumber, int pageSize, string? searchTerm, string? status)
        {
            IQueryable<Trainee> query = AllFilterQueries(searchTerm, status);

            int totalRecords = await query.CountAsync();

            List<Trainee> pagedTrainees = await query.OrderBy(trainee => trainee.Id)
                           .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            List<TraineeResponse> traineeResponses = pagedTrainees.Select(MapToResponse).ToList();
            return new PagedResponse<TraineeResponse>(pageNumber, pageSize, totalRecords, traineeResponses);
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
            _logger.LogInformation($"Trainee Created Successfully with ID: {newTrainee.Id} at Timestamp: {newTrainee.CreatedDate}");
            return MapToResponse(newTrainee);
        }

        public async Task<TraineeResponse?> UpdateTraineeAsync(int id, UpdateTraineeRequest request)
        {
            Trainee? trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return null;

            trainee.FirstName = request.FirstName;
            trainee.LastName = request.LastName;
            trainee.Email = request.Email;
            trainee.TechStack = request.TechStack;
            trainee.Status = Enum.Parse<TraineeStatus>(request.Status!.ToString(), ignoreCase: true);
            trainee.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Trainee with ID: {trainee.Id} was successfully updated at Timestamp: {trainee.UpdatedDate} UTC");
            return MapToResponse(trainee);
        }

        public async Task<bool> DeleteTraineeAsync(int id)
        {
            Trainee? trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return false;

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Trianee with ID: {trainee.Id} was successfully deleted at Timestamp: {DateTime.UtcNow} UTC");
            return true;
        }
    }
}