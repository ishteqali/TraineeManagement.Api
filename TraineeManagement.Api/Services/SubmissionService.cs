using TraineeManagement.Api.DTOs;
using TraineeManagement.Shared.Enums;
using TraineeManagement.Shared.Models;
using TraineeManagement.Shared.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Helpers;
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
                                    .FirstOrDefaultAsync(taskAssignment => taskAssignment.Id == request.TaskAssignmentId) ?? throw new NotFoundException(ExceptionMessages.TaskAssignmentNotFound(request.TaskAssignmentId));
            Submission submission = new Submission
            {
                TaskAssignmentId = request.TaskAssignmentId,
                SubmissionUrl = request.SubmissionUrl,
                Notes = request.Notes,
                SubmittedDate = DateTime.UtcNow,
                Status = EnumHelper.ParseOrThrow<SubmissionStatus>(request.Status, nameof(request.Status)),
                TaskAssignment = taskAssignment
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Submission Created Successfully with ID: {SubmissionId} at Timestamp: {Timestamp}",
                submission.Id, DateTime.UtcNow);


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
            Func<Task<SubmissionResponse?>> retrieveFromDb = async () =>
            {
                Submission? submission = await _context.Submissions
                    .Include(currentSubmission => currentSubmission.TaskAssignment)
                    .ThenInclude(ta => ta.LearningTask)
                    .FirstOrDefaultAsync(s => s.Id == id);
                return submission is null ? null : MapToResponse(submission);
            };
            return await _cache.GetOrSetAsync(cacheKey, retrieveFromDb, _logger
            );
        }
    }
}