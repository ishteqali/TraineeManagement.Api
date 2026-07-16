using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Enums;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TraineeManagement.Api.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubmissionService> _logger;
        private readonly IDistributedCache _cache;

        public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        private SubmissionResponse MapToResponse(Submission submission)
        {
            return new SubmissionResponse
            {
                Id = submission.Id,
                TaskAssignmentId = submission.TaskAssignmentId,
                TaskTitle = submission.TaskAssignment.LearningTask.Title,
                SubmissionUrl = submission.SubmissionUrl,
                Notes = submission.Notes,
                SubmittedDate = submission.SubmittedDate,
                Status = submission.Status.ToString(),
            };
        }

        public async Task<SubmissionResponse> CreateSubmissionAsync(CreateSubmissionRequest request)
        {
            TaskAssignment? taskAssignment = await _context.TaskAssignments
                                    .Include(taskAssignment => taskAssignment.LearningTask)
                                    .FirstOrDefaultAsync(taskAssignment => taskAssignment.Id == request.TaskAssignmentId);
            if (taskAssignment == null) throw new NotFoundException(ExceptionMessages.TaskAssignmentNotFound(request.TaskAssignmentId));

            Submission submission = new Submission
            {
                TaskAssignmentId = request.TaskAssignmentId,
                SubmissionUrl = request.SubmissionUrl,
                Notes = request.Notes,
                SubmittedDate = DateTime.UtcNow,
                Status = Enum.Parse<SubmissionStatus>(request.Status!.ToString(), ignoreCase: true),
                TaskAssignment = taskAssignment
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Submission Created Successfully with ID: {submission.Id} at Timestamp: {DateTime.UtcNow}");

            return MapToResponse(submission);
        }

        public async Task<IEnumerable<SubmissionResponse>> GetAllSubmissionsAsync()
        {
            IEnumerable<Submission> submissions = await _context.Submissions
                .Include(submission => submission.TaskAssignment)
                .ThenInclude(taskAssignment => taskAssignment.LearningTask)
                .ToListAsync();

            return submissions.Select(MapToResponse).ToList();
        }

        public async Task<SubmissionResponse?> GetSubmissionByIdAsync(int id)
        {
            string cacheKey = CacheKeys.Submission(id);
            try
            {
                string? cachedData = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<SubmissionResponse>(cachedData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis cache unavailable. Falling back to MySQL.");
            }

            Submission? submission = await _context.Submissions
                .Include(s => s.TaskAssignment)
                .ThenInclude(ta => ta.LearningTask)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null)
            {
                return null;
            }
            SubmissionResponse response = MapToResponse(submission);
            
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
            return MapToResponse(submission);
        }
    }
}