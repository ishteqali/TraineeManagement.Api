using TraineeManagement.Api.DTOs;
using TraineeManagement.Shared.Enums;
using TraineeManagement.Shared.Models;
using TraineeManagement.Shared.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Helpers;

namespace TraineeManagement.Api.Services
{
    public class TraineeService : ITraineeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TraineeService> _logger;
        private readonly IDistributedCache _cache;
        public TraineeService(AppDbContext context, ILogger<TraineeService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
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

        private IQueryable<Trainee> SearchFilterQuery(IQueryable<Trainee> query, string? searchTerm)
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

        private IQueryable<Trainee> StatusFilterQuery(IQueryable<Trainee> query, string? status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse(status, true, out TraineeStatus parsedStatus))
                {
                    query = query.Where(trainee => trainee.Status == parsedStatus);
                }
                else
                {
                    query = query.Where(trainee => false);
                }
            }
            return query;
        }

        private IQueryable<Trainee> AllFilterQueries(string? searchTerm, string? status)
        {
            IQueryable<Trainee> query = _context.Trainees.AsQueryable();
            query = SearchFilterQuery(query, searchTerm);
            query = StatusFilterQuery(query, status);
            return query;
        }

        public async Task<PagedResponse<TraineeResponse>> GetTraineesAsync(int pageNumber, int pageSize, string? searchTerm, string? status)
        {
            // Preventing bad input for pageNumber and pageSize 
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            IQueryable<Trainee> query = AllFilterQueries(searchTerm, status);

            int totalRecords = await query.CountAsync();

            List<Trainee> pagedTrainees = await query.OrderBy(trainee => trainee.Id)
                           .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            List<TraineeResponse> traineeResponses = pagedTrainees.Select(MapToResponse).ToList();
            return new PagedResponse<TraineeResponse>(pageNumber, pageSize, totalRecords, traineeResponses);
        }

        public async Task<TraineeResponse?> GetTraineeByIdAsync(int id)
        {
            string cacheKey = CacheKeys.Trainee(id);
            try
            {
                string? cachedData = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<TraineeResponse>(cachedData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis cache unavailable. Falling back to MySQL.");
            }

            Trainee? trainee = await _context.Trainees.FindAsync(id);
            if (trainee is null) return null;
            TraineeResponse response = MapToResponse(trainee);
            DistributedCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            string? serializedResponse = JsonSerializer.Serialize(response);

            try
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    serializedResponse,
                    cacheOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to write data to Redis cache.");
            }
            return response;
        }

        public async Task<TraineeResponse> AddTraineeAsync(CreateTraineeRequest request)
        {
            Trainee newTrainee = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = EnumHelper.ParseOrThrow<TraineeStatus>(request.Status, nameof(request.Status)),
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
            if (trainee is null) return null;

            trainee.FirstName = request.FirstName;
            trainee.LastName = request.LastName;
            trainee.Email = request.Email;
            trainee.TechStack = request.TechStack;
            trainee.Status = EnumHelper.ParseOrThrow<TraineeStatus>(request.Status, nameof(request.Status));
            trainee.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Trainee with ID: {trainee.Id} was successfully updated at Timestamp: {trainee.UpdatedDate} UTC");
            try
            {
                await _cache.RemoveAsync(CacheKeys.Trainee(id));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to invalidate Redis cache for trainee {Id}.", id);
            }
            return MapToResponse(trainee);
        }

        public async Task<bool> DeleteTraineeAsync(int id)
        {
            Trainee? trainee = await _context.Trainees.FindAsync(id);
            if (trainee is null) return false;

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Trianee with ID: {trainee.Id} was successfully deleted at Timestamp: {DateTime.UtcNow} UTC");
            try
            {
                await _cache.RemoveAsync(CacheKeys.Trainee(id));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to invalidate Redis cache for trainee {Id}.", id);
            }
            return true;
        }
    }
}